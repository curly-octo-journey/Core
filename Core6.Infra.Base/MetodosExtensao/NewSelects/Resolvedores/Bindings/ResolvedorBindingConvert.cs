using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public class ResolvedorBindingConvert : IResolvedorBinding
    {
        public MemberAssignment Resolver(ResolvedorMemberInit resolvedorMemberInit, int nivel, Expression parentExp, PropertyInfo propSrc, PropertyInfo propDest)
        {
            var convert = Expression.Convert(Expression.Property(parentExp, propSrc), propDest.PropertyType);
            return Expression.Bind(propDest, convert);
        }
    }
}
