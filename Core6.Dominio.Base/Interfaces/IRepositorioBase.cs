using Core6.Dominio.Base.Entidades;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepositorioBase<TEntidade> : IRepCon<TEntidade> where TEntidade : Identificador
    {
        TEntidade RecuperarPorIdObrigatorio(int id, string msg, params string[] includes);
        TEntidade RecuperarPorIdOriginal(int id, params string[] includes);
        IQueryable<TEntidade> RecuperarOriginal(params string[] includes);
    }
}
