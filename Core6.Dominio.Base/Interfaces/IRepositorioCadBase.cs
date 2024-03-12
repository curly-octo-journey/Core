using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepositorioCadBase<TEntidade> : IRepCad<TEntidade>, IRepositorioBase<TEntidade> where TEntidade : Identificador
    {
        int ProximoNumero();
        int ProximoNumeroTenant();
    }
}
