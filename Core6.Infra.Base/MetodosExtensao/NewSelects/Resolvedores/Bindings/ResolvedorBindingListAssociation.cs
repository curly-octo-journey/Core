using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public class ResolvedorBindingListAssociation : IResolvedorBinding
    {
        private readonly MethodInfo _selectMethod;
        private readonly MethodInfo _toListMethod;

        public ResolvedorBindingListAssociation()
        {
            _selectMethod = typeof(Enumerable).GetMethods().FirstOrDefault(p => p.Name == "Select");
            _toListMethod = typeof(Enumerable).GetMethods().FirstOrDefault(p => p.Name == "ToList");
        }

        public MemberAssignment Resolver(ResolvedorMemberInit resolvedorMemberInit, int nivel, Expression parentExp, PropertyInfo propSrc, PropertyInfo propDest)
        {
            var genericTypeSrc = propSrc.PropertyType.GetGenericArguments().First();
            var genericTypeDest = propDest.PropertyType.GetGenericArguments().First();

            var paramSubSelect = Expression.Parameter(genericTypeSrc, string.Format("p{0}", nivel));
            var memberInitSubSelect = resolvedorMemberInit.Resolver(nivel, paramSubSelect, genericTypeSrc, genericTypeDest);

            var lambda = Expression.Lambda(memberInitSubSelect, paramSubSelect);

            var selectMethodTyped = _selectMethod.MakeGenericMethod(genericTypeSrc, genericTypeDest);
            var paramAssoc = Expression.Property(parentExp, propSrc);
            var select = Expression.Call(selectMethodTyped, new Expression[]
            {
                paramAssoc,
                lambda
            });

            var toListMethodTyped = _toListMethod.MakeGenericMethod(genericTypeDest);
            var toList = Expression.Call(toListMethodTyped, select);

            return Expression.Bind(propDest, toList);
        }
    }
}
