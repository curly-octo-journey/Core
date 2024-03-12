using Core6.Dominio.Base.Entidades;

namespace Core6.Repositorio.Base.Personalizacao
{
    public interface IContextoPersonalizacao
    {
        void Inserir<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new();
        void Inserir<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new();
        void Alterar<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new();
        void Alterar<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new();
        IList<object> MontarCamposPersonalizadosLista<TEntidade>(List<object> resultadoPaginado) where TEntidade : Identificador, new();
        IList<object> MontarCamposPersonalizadosListaGuid<TEntidade>(List<object> dadosPaginacao) where TEntidade : IdentificadorGuid, new();
        object MontarCamposPersonalizadosLista<TEntidade>(object resultado) where TEntidade : Identificador, new();
        object MontarCamposPersonalizadosListaGuid<TEntidade>(object resultado) where TEntidade : IdentificadorGuid, new();
    }
}
