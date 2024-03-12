using Core6.Aplicacao.Base.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Infra.Base.Queries;

namespace Core6.Aplicacao.Base.Base
{
    public interface IAplicListaBase<TView> : IAplic
        where TView : IdView, new()
    {
        RetConView Listar(QueryParams queryParams);
        object Listar(int id);
        object Listar(int id, List<string> fields);
    }
}
