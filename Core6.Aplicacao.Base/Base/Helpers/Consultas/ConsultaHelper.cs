using Core6.Aplicacao.Base.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.Helpers.TypeBuilders;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using System.Linq.Expressions;
using Core6.Aplicacao.Base.Base.Helpers.Consultas.Per;

namespace Core6.Aplicacao.Base.Base.Helpers.Consultas
{
    public class ConsultaHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        public ConsultaHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RetConView Paginar<T, TView>(IQueryable<T> query, QueryParams queryParams)
            where T : Identificador
            where TView : IdView, new()
        {
            query = Consultar(query, queryParams);
            var queryFinal = AplicaFields<T, TView>(query, queryParams);
            var retorno = new RetConView();

            retorno.Total = queryFinal.Count();
            retorno.Conteudo = ExecutaPaginacao(queryFinal, queryParams);

            return retorno;
        }

        public RetConView Paginar<T>(IQueryable<T> query, QueryParams queryParams) where T : class
        {
            return Paginar(query, queryParams, null);
        }

        public RetConView Paginar<T>(IQueryable<T> query, QueryParams queryParams, Expression<Func<T, bool>> exp) where T : class
        {
            if (exp != null)
                query = query.Where(exp);

            query = Consultar(query, queryParams);
            var queryFinal = AplicaFields(query, queryParams);
            var retorno = new RetConView();
            retorno.Total = queryFinal.Count();
            retorno.Conteudo = ExecutaPaginacao(queryFinal, queryParams);

            return retorno;
        }

        public IQueryable<T> Consultar<T>(IQueryable<T> query, QueryParams queryParams) where T : class
        {
            var ordenacao = MontaOrderyBy<T>(queryParams);
            var cons = new ConsultaQuery<T>();
            var filtros = queryParams.GetFiltros();
            var expPer = new ConsultadorPersona(_unitOfWork).Aplicar<T>(filtros);

            cons.Consulta(query, filtros, ordenacao);

            if (expPer != null)
                cons.Query = cons.Query.Where(expPer);

            return cons.Query;
        }

        public List<T> ExecutaPaginacao<T>(IQueryable<T> query, QueryParams queryParams)
        {
            var page = queryParams.page ?? 0;
            var limit = queryParams.limit ?? 25;
            var content = query.Skip((page - 1) * limit).Take(limit).ToList();

            return content;
        }

        public IQueryable<T> OrderBy<T>(IQueryable<T> query, QueryParams queryParams) where T : class
        {
            var ordenacao = MontaOrderyBy<T>(queryParams);
            var cons = new OrderByQuery();
            return cons.MontaOrderBy(query, ordenacao);
        }

        public List<OrdemClass> MontaOrderyBy<T>(QueryParams queryParams)
        {
            var ordem = queryParams.GetOrdem();
            // Faz a atribuição do campo ID para ordenação default caso não tiver ordenação. 
            // Isso se faz necessário pois da erro ao paginar caso não seja ordenado.
            if (ordem != null)
                return ordem;

            if (typeof(T).GetProperty("Id") == null)
                return ordem;

            return new List<OrdemClass> { new OrdemClass { property = "Id", direction = "asc" } };
        }

        public IQueryable<object> AplicaFields<T, TView>(IQueryable<T> query, QueryField queryParams) where T : class
                                                                                                      where TView : new()
        {
            var fields = queryParams.GetFields();
            if (fields.uAny())
                return query.Qry().New(TypeBuilderHelper.CreateType(typeof(TView), fields));

            if (typeof(T) == typeof(TView))
                return query;

            return query.Qry().New<TView>() as IQueryable<object>;
        }

        public IQueryable<object> AplicaFields<T>(IQueryable<T> query, QueryField queryParams) where T : class
        {
            var fields = queryParams.GetFields();
            if (fields.uVazio())
                return query;

            return query.Qry().New(TypeBuilderHelper.CreateType(typeof(T), fields));
        }
    }
}
