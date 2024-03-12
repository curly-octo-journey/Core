using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepAlt<TEntidade> : IRepCon<TEntidade> where TEntidade : Identificador
    {
        void Inserir(TEntidade obj);
        void Inserir(List<TEntidade> objs);
        void Remover(TEntidade obj);
        void Remover(int id);
        void Remover(List<TEntidade> objs);
    }
}
