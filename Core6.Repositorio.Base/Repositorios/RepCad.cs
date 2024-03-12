using Core6.Repositorio.Base;
using Core6.Repositorio.Base.Contextos;
using Microsoft.EntityFrameworkCore;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.Auth;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Repositorio.Base.Repositorios
{
    public abstract class RepCad<TEntidade> : RepAlt<TEntidade>, IRepCad<TEntidade> where TEntidade : Identificador
    {
        public abstract string Sequence();

        public virtual string SequenceTenant()
        {
            return null;
        }

        protected RepCad(Contexto contexto)
            : base(contexto)
        {
        }

        public override void Inserir(TEntidade obj)
        {
            Inserir(new List<TEntidade> { obj });
        }

        public override void Inserir(List<TEntidade> objs)
        {
            if (objs.uVazio())
                return;

            if (!string.IsNullOrEmpty(Sequence()))
            {
                GeraIds(objs);
            }

            DbSet.AddRange(objs);
        }

        public void GeraId(TEntidade obj)
        {
            GeraIds(new List<TEntidade> { obj });
        }

        public void GeraIds(List<TEntidade> objs)
        {
            if (objs.uVazio())
                return;

            var id = ReservarIds(objs.Count);

            for (var i = objs.Count - 1; i >= 0; i--)
            {
                GeraIdSequenceTenant(objs[i]);
                objs[i].Id = id[i];
            }
        }

        public virtual void GeraIdSequenceTenant(TEntidade obj)
        {
            if (string.IsNullOrEmpty(SequenceTenant()))
            {
                return;
            }
            GerarSequenciaTenant();
        }

        public int GerarSequenciaTenant()
        {
            return Db.GerarSequenciaTenant(SequenceTenant(), DadosTokenHelperBase.Dados().RecuperarTenant());
        }

        private List<int> ReservarIds(int quant = 1)
        {
            var listaId = new List<int>();
            var resp = Db.Executar(MontarSqlBusca(Sequence(), quant)).ToList();
            foreach (var item in resp)
            {
                listaId.Add((int)item.id);
            }

            listaId = listaId.OrderByDescending(p => p).ToList();
            return listaId;
        }

        protected string MontarSqlBusca(string sequence, int quant = 1)
        {
            if (Db.Database.IsOracle())
            {
                return $"select {sequence}.nextval as \"id\" from (select level as seq from dual connect by level <= {quant} order by 1 asc)";
            }
            if (Db.Database.IsPostgreSql())
            {
                return $"select nextval('{sequence}') as id from (SELECT * FROM GENERATE_SERIES(1, {quant}) as seq) as GERADOR order by GERADOR.seq";
            }
            throw new Exception("Não foi possível reservar ids. Tipo de banco de dados não detectado.");
        }
    }
}