using Core6.Repositorio.Base.Contextos;
using Microsoft.EntityFrameworkCore;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;

namespace Core6.Repositorio.Base.Repositorios
{
    public class RepositorioLeituraGuidBase<TEntidade> : RepConGuid<TEntidade>, IRepositorioLeituraGuidBase<TEntidade> where TEntidade : IdentificadorGuid
    {
        #region ctor
        public RepositorioLeituraGuidBase(Contexto contexto)
            : base(contexto)
        {
        }
        #endregion

        #region Recuperar level 2
        public virtual IQueryable<TEntidade> Recuperar(List<FiltroClass> filtros, params string[] includes)
        {
            return Recuperar(filtros, null, includes); // chama level 3
        }
        #endregion

        #region Recuperar level 3
        public virtual IQueryable<TEntidade> Recuperar(List<FiltroClass> filtros, List<OrdemClass> ordenacao, params string[] includes)
        {
            var grupoFiltro = new GrupoFiltroQuery() { Filtros = filtros };
            var listaGrupo = new List<GrupoFiltroQuery>();
            listaGrupo.Add(grupoFiltro);

            return Recuperar(listaGrupo, ordenacao, includes);// chama level boss
        }
        #endregion

        #region Recuperar level 4
        public virtual IQueryable<TEntidade> Recuperar(List<GrupoFiltroQuery> filtros, params string[] includes)
        {
            return Recuperar(filtros, null, includes);// chama level boss
        }
        #endregion

        #region Recuperar level boss
        public virtual IQueryable<TEntidade> Recuperar(List<GrupoFiltroQuery> filtros, List<OrdemClass> ordenacao, params string[] includes)
        {
            var query = DbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            var cons = new ConsultaQuery<TEntidade>();

            cons.Consulta(query, filtros, ordenacao); // Este é quem "suja as mão" e realmente faz a consulta!

            return cons.Query;
        }
        #endregion

        public IQueryable<TDest> Recuperar<TDest>(List<GrupoFiltroQuery> filtros, params string[] includes) where TDest : new()
        {
            return Recuperar(filtros, includes)
                                             .To()
                                             .New<TDest>();
        }
    }
}
