using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepAltGuid<TEntidade> : IRepConGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        void Inserir(TEntidade obj);
        void Inserir(List<TEntidade> objs);
        void Remover(TEntidade obj);
        void Remover(Guid id);
        void Remover(List<TEntidade> objs);
    }
}
