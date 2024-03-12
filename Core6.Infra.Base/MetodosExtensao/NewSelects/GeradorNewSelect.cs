using System.Linq.Expressions;
using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects
{
    public class GeradorNewSelect
    {
        public Expression<Func<TSource, TDest>> Gerar<TSource, TDest>() where TDest : new()
        {
            var typeSrc = typeof(TSource);
            var param = Expression.Parameter(typeSrc, "p");
            var memberInit = new ResolvedorMemberInit().Resolver(0, param, typeSrc, typeof(TDest));

            return Expression.Lambda<Func<TSource, TDest>>(memberInit, param);
        }

        public Expression<Func<TSource, dynamic>> Gerar<TSource>(Type typeResult)
        {
            var typeSrc = typeof(TSource);
            var param = Expression.Parameter(typeSrc, "p");
            var memberInit = new ResolvedorMemberInit().Resolver(0, param, typeSrc, typeResult);

            return Expression.Lambda<Func<TSource, dynamic>>(memberInit, param);
        }
    }
}