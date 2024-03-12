using Core6.Dominio.Base.Entidades;
using System.Linq.Expressions;

namespace Core6.Dominio.Base.Interfaces
{
    public interface IRepCon<TEntidade> : IRep<TEntidade> where TEntidade : Identificador
    {
        TEntidade Find(int id);
        List<TEntidade> Find(int? id1, int? id2, params int?[] id3);
        List<TEntidade> Find(IEnumerable<int?> ids, params string[] includes);
        List<TEntidade> Find(IEnumerable<int> ids, params string[] includes);
        List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, int?> exp, string include, params string[] includes);
        List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, int?> exp, params Func<T, int?>[] exps);
        TEntidade RecuperarPorId(int id, params string[] includes);
        TDest RecuperarPorId<TDest>(int id) where TDest : new();
        TCampos RecuperarPorId<TCampos>(int id, Expression<Func<TEntidade, TCampos>> campos, params string[] includes);


        TEntidade RecuperarPorIdObrigatorio(int id, string msg, params string[] includes);
        TEntidade RecuperarPorIdOriginal(int id, params string[] includes);
        IQueryable<TEntidade> RecuperarOriginal(params string[] includes);
    }
}