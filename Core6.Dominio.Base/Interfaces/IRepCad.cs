using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepCad<TEntidade> : IRepAlt<TEntidade> where TEntidade : Identificador
    {
        void GeraId(TEntidade obj);
        void GeraIds(List<TEntidade> objs);
        void GeraIdSequenceTenant(TEntidade obj);
    }
}
