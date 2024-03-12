using Core6.Aplicacao.Base.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using System.Linq.Expressions;

namespace Core6.Aplicacao.Base.Base.Helpers.Consultas
{
    public class ConsultaGuidHelper
    {
        public RetConView Paginar<T, TView>(IQueryable<T> query, QueryParams queryParams)
            where T : IdentificadorGuid
            where TView : IdGuidView, new()
        {
            var queryView = Consultar(query, queryParams).To().New<TView>();
            var queryFinal = AplicaFields(queryView, queryParams);
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

            cons.Consulta(query, filtros, ordenacao);
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

        public IQueryable<object> AplicaFields<T>(IQueryable<T> query, QueryParams queryParams) where T : class
        {
            IQueryable<object> queryFinal = query;
            var fields = queryParams.GetFields();
            if (fields.Count > 0)
                queryFinal = query.SelectDynamic(fields) as IQueryable<object>;

            return queryFinal;
        }
    }
}
