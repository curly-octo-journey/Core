using Core6.Repositorio.Base.Contextos;
using Core6.Repositorio.Base.Infra;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.Auth;

namespace Core6.Repositorio.Base.Repositorios
{
    public class RepositorioCadBase<TEntidade> : RepCad<TEntidade>, IRepositorioCadBase<TEntidade> where TEntidade : Identificador
    {
        public RepositorioCadBase(Contexto contexto)
            : base(contexto)
        {
        }

        public override string Sequence()
        {
            return SequencesConfig.Recuperar(typeof(TEntidade).FullName).ToString();
        }

        public override string SequenceTenant()
        {
            return SequencesTenantConfig.Recuperar(typeof(TEntidade).FullName).ToString();
        }

        private int _proximoNumero(string sequence)
        {
            try
            {
                return Db.Executar(MontarSqlBusca(sequence)).First();
            }
            catch (Exception e)
            {
                Db.CriarSequence(sequence);
                return Db.Executar(MontarSqlBusca(sequence)).First();
            }
        }
        public int ProximoNumero()
        {
            var sequence = SequencesNumeroConfig.Recuperar(typeof(TEntidade).FullName).ToString();

            return _proximoNumero(sequence);
        }
        public int ProximoNumeroTenant()
        {
            var sequence = string.Format("{0}_{1}", SequencesNumeroConfig.Recuperar(typeof(TEntidade).FullName).ToString(), DadosTokenHelperBase.Dados().RecuperarTenant());
            return _proximoNumero(sequence);
        }
    }
}
