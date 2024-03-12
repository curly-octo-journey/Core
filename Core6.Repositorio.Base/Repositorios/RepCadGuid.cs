using Core6.Repositorio.Base;
using Core6.Repositorio.Base.Contextos;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.Auth;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Repositorio.Base.Repositorios
{
    public abstract class RepCadGuid<TEntidade> : RepAltGuid<TEntidade>, IRepCadGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        public virtual string SequenceTenant()
        {
            return null;
        }

        protected RepCadGuid(Contexto contexto)
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

            GeraIds(objs);

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
            for (var i = objs.Count - 1; i >= 0; i--)
            {
                if (objs[i].Id == Guid.Empty)
                {
                    objs[i].Id = GeraGuid();
                }
                GeraIdSequenceTenant(objs[i]);
            }
        }

        public virtual Guid GeraGuid()
        {
            return Guid.NewGuid();
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
    }
}