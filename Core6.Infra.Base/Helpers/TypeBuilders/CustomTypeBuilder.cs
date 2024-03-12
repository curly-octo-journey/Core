using Core6.Infra.Base.MetodosExtensao;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Core6.Infra.Base.Helpers.TypeBuilders
{
    public static class CustomTypeBuilder
    {
        /*
         * Classe adaptada a partir do exemplo encontrado no post https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class no dia 18/09/200
         */

        private const string ASSEMBLY_NAME = "DynamicAssembly_9501CA994D9046D1B4C014F9BE21130B";
        private const string MODULE_NAME = "DynamicModule_9501CA994D9046D1B4C014F9BE21130B";

        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly object _lock = new object();
        private static readonly IDictionary<string, Type> _dicTypes = new Dictionary<string, Type>();

        static CustomTypeBuilder()
        {
            _moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ASSEMBLY_NAME), AssemblyBuilderAccess.Run).DefineDynamicModule(MODULE_NAME);
            //_moduleBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ASSEMBLY_NAME), AssemblyBuilderAccess.Run)
            //                                        .DefineDynamicModule(MODULE_NAME);
        }

        public static Type CreateType(Dictionary<string, Type> props)
        {
            var key = CreateKey(props);

            lock (_lock)
            {
                Type type;
                if (_dicTypes.TryGetValue(key, out type))
                    return type;

                var tb = GetTypeBuilder();
                tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

                foreach (var prop in props)
                    CreateProperty(tb, prop.Key, prop.Value);

                type = tb.CreateType();
                _dicTypes.Add(key, type);

                return type;
            }
        }

        private static TypeBuilder GetTypeBuilder()
        {
            return _moduleBuilder.DefineType(string.Format("DynamicClass_{0}", Guid.NewGuid().ToString("N")),
                                             TypeAttributes.Public |
                                             TypeAttributes.Class |
                                             TypeAttributes.AutoClass |
                                             TypeAttributes.AnsiClass |
                                             TypeAttributes.BeforeFieldInit |
                                             TypeAttributes.AutoLayout,
                                             null);
        }

        private static void CreateProperty(TypeBuilder tb, string propName, Type propType)
        {
            var fieldBuilder = tb.DefineField(string.Format("_{0}", propName), propType, FieldAttributes.Private);
            var propertyBuilder = tb.DefineProperty(propName, PropertyAttributes.HasDefault, propType, null);
            var getPropMthdBldr = tb.DefineMethod(string.Format("get_{0}", propName), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propType, Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = tb.DefineMethod(string.Format("set_{0}", propName), MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propType });

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static string CreateKey(Dictionary<string, Type> props)
        {
            var str = string.Join(";", props.OrderBy(p => p.Key).Select(p => string.Format("{0}|{1}", p.Key, p.Value.FullName)));

            return Encoding.ASCII.GetBytes(str).HashMd5();
        }

        private static string HashMd5(this byte[] bytes)
        {
            if (bytes.uVazio())
                return null;

            using (var md5 = MD5.Create())
            {
                var data = md5.ComputeHash(bytes);
                var sb = new StringBuilder();

                foreach (var c in data)
                    sb.Append(c.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}