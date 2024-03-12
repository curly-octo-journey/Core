using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public class ResolvedorBindingComplexType : IResolvedorBinding
    {
        public MemberAssignment Resolver(ResolvedorMemberInit resolvedorMemberInit, int nivel, Expression parentExp, PropertyInfo propSrc, PropertyInfo propDest)
        {
            var paramAssoc = Expression.Property(parentExp, propSrc);

            var memberInit = resolvedorMemberInit.Resolver(nivel,
                                                           paramAssoc,
                                                           propSrc.PropertyType,
                                                           propDest.PropertyType);

            return Expression.Bind(propDest, memberInit);
        }
    }
}
