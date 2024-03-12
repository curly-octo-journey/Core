
using Core6.Repositorio.Base.Contextos;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;

namespace Core6.Repositorio.Base.Repositorios
{
    public abstract class RepGuid<TEntidade> : IRepGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        protected readonly DbContext Db;
        protected readonly DbSet<TEntidade> DbSet;

        public RepGuid(Contexto contexto)
        {
            Db = contexto;
            DbSet = Db.Set<TEntidade>();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public IQueryable<TEntidade> Recuperar(params string[] includes)
        {
            if (includes.uVazio())
                return DbSet;

            var query = DbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public IQueryable<TEntidade> Where(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.Where(exp);
        }

        public TEntidade Single()
        {
            return DbSet.Single();
        }

        public TEntidade Single(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.Single(exp);
        }

        public TEntidade SingleOrDefault()
        {
            return DbSet.SingleOrDefault();
        }

        public TEntidade SingleOrDefault(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.SingleOrDefault(exp);
        }

        public TEntidade First()
        {
            return DbSet.First();
        }

        public TEntidade First(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.First(exp);
        }

        public TEntidade FirstOrDefault()
        {
            return DbSet.FirstOrDefault();
        }

        public TEntidade FirstOrDefault(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.FirstOrDefault(exp);
        }

        public IQueryable<TCampos> Select<TCampos>(Expression<Func<TEntidade, TCampos>> campos)
        {
            return DbSet.Select(campos);
        }

        public IOrderedQueryable<TEntidade> OrderBy<TCampos>(Expression<Func<TEntidade, TCampos>> campos)
        {
            return DbSet.OrderBy(campos);
        }

        public IOrderedQueryable<TEntidade> OrderByDescending<TCampos>(Expression<Func<TEntidade, TCampos>> campos)
        {
            return DbSet.OrderByDescending(campos);
        }

        public bool Any()
        {
            return DbSet.Any();
        }

        public bool Any(Expression<Func<TEntidade, bool>> exp)
        {
            return DbSet.Any(exp);
        }

        #region Recuperar level 1
        public virtual IQueryable<TEntidade> Recuperar(QueryParams queryParams)
        {
            var filtros = queryParams.GetFiltros();
            var includes = queryParams.GetIncludes();
            var ordem = queryParams.GetOrdem() ?? OrdenacaoPadrao();

            return Recuperar(filtros, ordem, includes.ToArray());
        }
        #endregion

        private List<OrdemClass> OrdenacaoPadrao()
        {
            return new List<OrdemClass> { new OrdemClass { property = "Id", direction = "asc" } };
        }

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
