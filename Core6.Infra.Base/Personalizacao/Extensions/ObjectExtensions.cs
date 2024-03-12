using Core6.Infra.Base.Personalizacao.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Core6.Infra.Base.Personalizacao.Extensions
{
    public static class ObjectExtensions
    {
        public static T ToObject<T>(this Dictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                         .GetProperty(item.Key)
                         .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static Dictionary<string, object> ToDictionary(this object source)
        {
            return source.GetType().GetProperties().ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
