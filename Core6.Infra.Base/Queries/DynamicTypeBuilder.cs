using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Core6.Infra.Base.Queries
{
    public static class DynamicTypeBuilder
    {
        private static AssemblyName assemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        private static ModuleBuilder moduleBuilder = null;
        private static Dictionary<string, Tuple<string, Type>> builtTypes = new Dictionary<string, Tuple<string, Type>>();

        static DynamicTypeBuilder()
        {
            //TODO
            //moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name);
            moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(assemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        {
            string key = string.Empty;
            foreach (var field in fields.OrderBy(v => v.Key).ThenBy(v => v.Value.Name))
                key += field.Key + ";" + field.Value.Name + ";";
            return key;
        }

        public static Type GetDynamicType(Dictionary<string, Type> fields, Type basetype, Type[] interfaces)
        {
            if (null == fields)
                throw new ArgumentNullException("fields");
            if (0 == fields.Count)
                throw new ArgumentOutOfRangeException("fields", "fields must have at least 1 field definition");

            try
            {
                Monitor.Enter(builtTypes);
                string typeKey = GetTypeKey(fields);

                //Comentei para os que cammpos que aceitam nulo funcionem
                if (builtTypes.ContainsKey(typeKey))
                    return builtTypes[typeKey].Item2;

                string typeName = "DynamicLinqType" + moduleBuilder.GetTypes().Count().ToString();
                TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, null, Type.EmptyTypes);

                // Agrupa os fields pela classe que contem eles
                var groupedFields = fields.GroupBy(x => x.Key.Split('.')[0]);

                // typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);

                var xx = moduleBuilder.GetTypes();

                foreach (var groupedItem in groupedFields)
                {
                    var firstField = groupedItem.FirstOrDefault();
                    //var isList2 = firstField.Value.GetGenericArguments().Count() > 0;
                    var isList = firstField.Value is IList;

                    // É uma propriedade da classe
                    if (firstField.Key.Split('.').Count() == 1 && !isList)
                    {
                        typeBuilder.DefineField(firstField.Key, firstField.Value, FieldAttributes.Public);
                    }

                    // É uma associação da classe
                    else
                    {
                        var memberName = groupedItem.Key;
                        var memberFields = groupedItem.Where(x => x.Key != memberName).ToDictionary(x => x.Key.Remove(0, x.Key.IndexOf(".") + 1), x => x.Value);
                        if (memberFields.Count > 0)
                        {
                            var memberType = GetDynamicType(memberFields, typeof(object), Type.EmptyTypes);
                            if (isList)
                            {
                                IList lst = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(memberType));
                                memberType = lst.GetType();
                            }

                            typeBuilder.DefineField(memberName, memberType, FieldAttributes.Public);
                        }
                    }

                }

                builtTypes[typeKey] = new Tuple<string, Type>(typeName, typeBuilder.CreateType());

                return builtTypes[typeKey].Item2;
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(builtTypes);
            }

        }

    }
}