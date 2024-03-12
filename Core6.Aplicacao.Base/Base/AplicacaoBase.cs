using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;

namespace Core6.Aplicacao.Base.Base
{
    public class AplicacaoBase
    {
        private IUnitOfWork Transacao { get; set; }

        protected AplicacaoBase(IUnitOfWork _Transacao)
        {
            Transacao = _Transacao;
        }

        protected void PersistirTransacao()
        {
            Transacao.Persistir();
        }

        protected void RejectChanges()
        {
            Transacao.RejectChanges();
        }

        protected void PersonalizacaoInserir<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            Transacao.PersonalizacaoInserir<TEntidade>(id, obj);
        }

        protected void PersonalizacaoAlterar<TEntidade>(Guid id, Dictionary<string, object> obj) where TEntidade : IdentificadorGuid, new()
        {
            Transacao.PersonalizacaoAlterar<TEntidade>(id, obj);
        }

        protected void PersonalizacaoInserir<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            Transacao.PersonalizacaoInserir<TEntidade>(id, obj);
        }

        protected void PersonalizacaoAlterar<TEntidade>(int id, Dictionary<string, object> obj) where TEntidade : Identificador, new()
        {
            Transacao.PersonalizacaoAlterar<TEntidade>(id, obj);
        }
    }
}
