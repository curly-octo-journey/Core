using Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings;
using System.Linq.Expressions;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores
{
    public class ResolvedorMemberInit
    {
        private readonly FabricaResolvedorBinding _fabricaResolvedorBinding;

        public ResolvedorMemberInit()
        {
            _fabricaResolvedorBinding = new FabricaResolvedorBinding();
        }

        public MemberInitExpression Resolver(int nivel, Expression parentExp, Type typeSrc, Type typeDest)
        {
            nivel++;
            var listaPropDest = typeDest.GetProperties().Where(p => p.CanWrite
                                                                 && p.GetSetMethod() != null
                                                                 && p.GetSetMethod().IsPublic)
                                                        .ToList();

            var bindings = new List<MemberBinding>();

            foreach (var propDest in listaPropDest)
            {
                var propSrc = typeSrc.GetProperty(propDest.Name);

                if (propSrc == null)
                    continue;

                var resolvedor = _fabricaResolvedorBinding.GetInstance(propSrc, propDest);
                var binding = resolvedor.Resolver(this, nivel, parentExp, propSrc, propDest);

                bindings.Add(binding);
            }

            return Expression.MemberInit(Expression.New(typeDest), bindings);
        }
    }
}