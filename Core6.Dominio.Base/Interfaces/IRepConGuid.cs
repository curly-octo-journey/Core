using Core6.Dominio.Base.Entidades;
using System.Linq.Expressions;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepConGuid<TEntidade> : IRepGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        TEntidade Find(Guid id);
        List<TEntidade> Find(Guid? id1, Guid? id2, params Guid?[] id3);
        List<TEntidade> Find(IEnumerable<Guid?> ids, params string[] includes);
        List<TEntidade> Find(IEnumerable<Guid> ids, params string[] includes);
        List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, Guid?> exp, string include, params string[] includes);
        List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, Guid?> exp, params Func<T, Guid?>[] exps);
        TEntidade RecuperarPorId(Guid id, params string[] includes);
        TDest RecuperarPorId<TDest>(Guid id) where TDest : new();
        TCampos RecuperarPorId<TCampos>(Guid id, Expression<Func<TEntidade, TCampos>> campos, params string[] includes);


        TEntidade RecuperarPorIdObrigatorio(Guid id, string msg, params string[] includes);
        TEntidade RecuperarPorIdOriginal(Guid id, params string[] includes);
        IQueryable<TEntidade> RecuperarOriginal(params string[] includes);
    }
}