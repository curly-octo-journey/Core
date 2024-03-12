using Core6.Infra.Base.MetodosExtensao;
using System.Reflection;

namespace Core6.Infra.Base.Helpers.TypeBuilders
{
    public static class TypeBuilderHelper
    {
        private class PropStringDto
        {
            public PropStringDto()
            {
                SubProps = new List<PropStringDto>();
            }

            public string Name { get; set; }
            public List<PropStringDto> SubProps { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public static Type CreateType(Dictionary<string, Type> props)
        {
            return CustomTypeBuilder.CreateType(props);
        }

        public static Type CreateType(Type type, IEnumerable<string> props)
        {
            var propsSplit = props.Select(p => p.Split('.')).ToList();
            return CreateTypeByString(type, CreateGraphPropString(propsSplit));
        }

        private static List<PropStringDto> CreateGraphPropString(List<string[]> props)
        {
            var graph = new List<PropStringDto>();
            foreach (var propsAg in props.Where(p => p.uAny()).GroupBy(p => p[0]))
            {
                var propStr = new PropStringDto();
                graph.Add(propStr);

                propStr.Name = propsAg.Key;

                if (propsAg.First().Count() > 1)
                    propStr.SubProps.AddRange(CreateGraphPropString(propsAg.Select(p => p.Skip(1).ToArray()).ToList()));
            }

            return graph;
        }

        private static Type CreateTypeByString(Type type, List<PropStringDto> props)
        {
            var dicType = new Dictionary<string, Type>();

            foreach (var prop in props)
            {
                var propInfo = type.GetProperty(prop.Name);
                if (propInfo == null)
                    continue;

                var tuple = CreateTypeItem(propInfo, prop);
                dicType.Add(tuple.Item1, tuple.Item2);
            }

            return CustomTypeBuilder.CreateType(dicType);
        }

        private static Type CreateTypeByPropInfo(Type type)
        {
            var dicType = type.GetProperties().Select(p => CreateTypeItem(p, null)).ToDictionary(p => p.Item1, p => p.Item2);

            return CustomTypeBuilder.CreateType(dicType);
        }

        private static Tuple<string, Type> CreateTypeItem(PropertyInfo propInfo, PropStringDto prop)
        {
            if (propInfo.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(propInfo.PropertyType))
                return CreateTypeList(propInfo, prop);

            if (propInfo.PropertyType.IsClass)
            {
                if (prop != null && prop.SubProps.Any())
                    return Tuple.Create(propInfo.Name, CreateTypeByString(propInfo.PropertyType, prop.SubProps));

                return Tuple.Create(propInfo.Name, propInfo.PropertyType);
            }

            return Tuple.Create(propInfo.Name, propInfo.PropertyType);
        }

        private static Tuple<string, Type> CreateTypeList(PropertyInfo propInfo, PropStringDto prop)
        {
            var genericTypes = CreateGenericTypesFromList(propInfo.PropertyType, prop);
            return Tuple.Create(propInfo.Name, typeof(List<>).MakeGenericType(genericTypes.ToArray()));
        }

        private static List<Type> CreateGenericTypesFromList(Type listType, PropStringDto prop)
        {
            var genericTypesDest = new List<Type>();

            foreach (var genericTypeSrc in listType.GetGenericArguments())
            {
                if (genericTypeSrc.IsPrimitive)
                {
                    genericTypesDest.Add(genericTypeSrc);
                    continue;
                }

                genericTypesDest.Add(prop != null && prop.SubProps.Any() ? CreateTypeByString(genericTypeSrc, prop.SubProps)
                                                                         : CreateTypeByPropInfo(genericTypeSrc));
            }

            return genericTypesDest;
        }
    }
}
