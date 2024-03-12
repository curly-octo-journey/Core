using Core6.Aplicacao.Base.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Infra.Base.Queries;

namespace Core6.Aplicacao.Base.Base
{
    public interface IAplicListaGuidBase<TView> : IAplic
        where TView : IdGuidView, new()
    {
        RetConView Listar(QueryParams queryParams);
        object Listar(Guid id);
        object Listar(Guid id, List<string> fields);
    }
}
