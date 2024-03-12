using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public class ResolvedorBindingDefault : IResolvedorBinding
    {
        public MemberAssignment Resolver(ResolvedorMemberInit resolvedorMemberInit, int nivel, Expression parentExp, PropertyInfo propSrc, PropertyInfo propDest)
        {
            return Expression.Bind(propDest, Expression.Property(parentExp, propSrc));
        }
    }
}
