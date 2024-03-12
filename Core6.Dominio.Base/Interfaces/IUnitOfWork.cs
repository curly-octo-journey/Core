using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Persistir();
        void RejectChanges();
        void PersonalizacaoInserir<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new();
        void PersonalizacaoAlterar<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new();
        void PersonalizacaoInserir<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new();
        void PersonalizacaoAlterar<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new();
        object MontarCamposPersonalizadosLista<TEntidade>(object resultado) where TEntidade : Identificador, new();
        object MontarCamposPersonalizadosListaGuid<TEntidade>(object resultado) where TEntidade : IdentificadorGuid, new();
        IList<object> MontarCamposPersonalizadosLista<TEntidade>(List<object> dadosPaginacao) where TEntidade : Identificador, new();
        IList<object> MontarCamposPersonalizadosListaGuid<TEntidade>(List<object> dadosPaginacao) where TEntidade : IdentificadorGuid, new();
        IEnumerable<dynamic> Executar(string sql);
    }
}
