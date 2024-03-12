using Core6.Infra.Base.MetodosExtensao.NewSelects.Annotations;
using System.Reflection;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects.Resolvedores.Bindings
{
    public class FabricaResolvedorBinding
    {
        private readonly IDictionary<Type, IResolvedorBinding> _resolvedores;

        public FabricaResolvedorBinding()
        {
            _resolvedores = new Dictionary<Type, IResolvedorBinding>();
        }

        private IResolvedorBinding GetResolvedor(Type type)
        {
            IResolvedorBinding resolvedor;
            if (_resolvedores.TryGetValue(type, out resolvedor))
                return resolvedor;

            return _resolvedores[type] = (IResolvedorBinding)Activator.CreateInstance(type);
        }

        public IResolvedorBinding GetInstance(PropertyInfo propSrc, PropertyInfo propDest)
        {
            /*Propriedades do mesmo tipo utilizam o bind padrão*/
            if (propSrc.PropertyType == propDest.PropertyType)
                return GetResolvedor(typeof(ResolvedorBindingDefault));

            /*Propriedades de lista utilizam o resolvedor de lista para criar um sub-select*/
            if (propSrc.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(propSrc.PropertyType))
                return GetResolvedor(typeof(ResolvedorBindingListAssociation));

            /*Propriedades que são associações e tipos diferente utilizam binds que instanciam o DTO da classe de destino*/
            if (propDest.PropertyType.IsClass)
            {
                if (Attribute.IsDefined(propSrc, typeof(ComplexTypeNewSelectAttribute)) || Attribute.IsDefined(propDest, typeof(ComplexTypeNewSelectAttribute)))
                    return GetResolvedor(typeof(ResolvedorBindingComplexType));

                return GetResolvedor(typeof(ResolvedorBindingAssociation));
            }

            /*Propriedades que são de tipos diferente utilizam binds que tentam converter a propriedade origem para destino*/
            return GetResolvedor(typeof(ResolvedorBindingConvert));
        }
    }
}
