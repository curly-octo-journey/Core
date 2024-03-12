using Core6.Infra.Base.Queries;

namespace Core6.Infra.Base.Queries
{
    public class ConsultaQuery<T>
    {
        public IQueryable<T> Query { get; set; }

        public void Consulta(IQueryable<T> query, List<GrupoFiltroQuery> filtros, List<OrdemClass> ordenacao)
        {
            var where = new WhereQuery();
            query = where.MontaWhere(query, filtros);

            var ordem = new OrderByQuery();
            query = ordem.MontaOrderBy(query, ordenacao);

            Query = query;
        }
    }
}