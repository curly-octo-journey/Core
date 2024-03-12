using Core6.Dominio.Base.Entidades;
using Core6.Infra.Base.Queries;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepositorioLeituraGuidBase<TEntidade> : IRepConGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        IQueryable<TEntidade> Recuperar(List<GrupoFiltroQuery> filtros, params string[] includes);
        IQueryable<TDest> Recuperar<TDest>(List<GrupoFiltroQuery> filtros, params string[] includes) where TDest : new();
    }
}
