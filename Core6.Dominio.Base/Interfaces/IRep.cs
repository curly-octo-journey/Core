using Core6.Dominio.Base.Entidades;
using Core6.Infra.Base.Queries;
using System.Linq.Expressions;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRep<TEntidade> : IDisposable where TEntidade : Identificador
    {
        IQueryable<TEntidade> Recuperar(params string[] includes);
        IQueryable<TEntidade> Where(Expression<Func<TEntidade, bool>> exp);
        IQueryable<TCampos> Select<TCampos>(Expression<Func<TEntidade, TCampos>> campos);
        TEntidade Single();
        TEntidade Single(Expression<Func<TEntidade, bool>> exp);
        TEntidade SingleOrDefault();
        TEntidade SingleOrDefault(Expression<Func<TEntidade, bool>> exp);
        TEntidade First();
        TEntidade First(Expression<Func<TEntidade, bool>> exp);
        TEntidade FirstOrDefault();
        TEntidade FirstOrDefault(Expression<Func<TEntidade, bool>> exp);
        IOrderedQueryable<TEntidade> OrderBy<TCampos>(Expression<Func<TEntidade, TCampos>> campos);
        IOrderedQueryable<TEntidade> OrderByDescending<TCampos>(Expression<Func<TEntidade, TCampos>> campos);
        bool Any();
        bool Any(Expression<Func<TEntidade, bool>> exp);
        IQueryable<TEntidade> Recuperar(QueryParams queryParams);
        IQueryable<TEntidade> Recuperar(List<FiltroClass> filtros, params string[] includes);
        IQueryable<TEntidade> Recuperar(List<FiltroClass> filtros, List<OrdemClass> ordenacao, params string[] includes);
        IQueryable<TEntidade> Recuperar(List<GrupoFiltroQuery> filtros, params string[] includes);
        IQueryable<TEntidade> Recuperar(List<GrupoFiltroQuery> filtros, List<OrdemClass> ordenacao, params string[] includes);
        IQueryable<TDest> Recuperar<TDest>(List<GrupoFiltroQuery> filtros, params string[] includes) where TDest : new();
    }
}
