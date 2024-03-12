using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepCadGuid<TEntidade> : IRepAltGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        void GeraId(TEntidade obj);
        void GeraIds(List<TEntidade> objs);
        void GeraIdSequenceTenant(TEntidade obj);
        Guid GeraGuid();
    }
}
