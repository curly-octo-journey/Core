using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public interface IResolvedorBinding
    {
        MemberAssignment Resolver(ResolvedorMemberInit resolvedorMemberInit, int nivel, Expression parentExp, PropertyInfo propSrc, PropertyInfo propDest);
    }
}
