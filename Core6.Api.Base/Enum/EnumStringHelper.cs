using Core6.Infra.Base;
using System.ComponentModel;
using System.Reflection;

namespace Core6.Api.Base.Enum
{
    #region DTOs
    public class RetornoEnumStringDTO
    {
        public object Nome { get; set; }
        public List<EnumStringDTO> Itens { get; set; }
    }

    public class EnumStringDTO : ListEnumString
    {
        public string Cor { get; set; }
        public string Identificador { get; set; }
    }
    #endregion

    public class EnumStringHelper
    {
        #region RecuperarTodos
        public static List<RetornoEnumStringDTO> RecuperarTodos(string namespaceBase, params string[] namespaceExcludes)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains(namespaceBase)).ToList();
            var types = assemblies.SelectMany(x => x.GetTypes().Where(y => y.IsEnum
                                                                        || y.Name.StartsWith("Enum"))).ToList();
            if (namespaceExcludes != null && namespaceExcludes.Any())
            {
                types.RemoveAll(x => namespaceExcludes.Any(y => (x.FullName ?? "").StartsWith(y)));
            }
            var retorno = (from t in types
                           select new RetornoEnumStringDTO
                           {
                               Nome = t.Name,
                               Itens = t.IsEnum ? (from f in t.GetFields()
                                                   where !f.Name.Equals("value__")
                                                   let descricaoAttrib = f.GetCustomAttribute(typeof(DescricaoAttribute)) as DescricaoAttribute
                                                   let descriptAttrib = f.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute
                                                   select new EnumStringDTO
                                                   {
                                                       Identificador = f.Name,
                                                       Text = descricaoAttrib != null ? descricaoAttrib.Description :
                                                              descriptAttrib != null ? descriptAttrib.Description :
                                                              f.Name,
                                                       Cor = descricaoAttrib != null ? descricaoAttrib.Cor : "",
                                                       Value = f.GetRawConstantValue().ToString()
                                                   }).ToList() :
                                       t.IsClass ? (from f in t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                                    where !f.Name.Equals("value__")
                                                    let descricaoAttrib = f.GetCustomAttribute(typeof(DescricaoAttribute)) as DescricaoAttribute
                                                    let descriptAttrib = f.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute
                                                    select new EnumStringDTO
                                                    {
                                                        Identificador = f.Name,
                                                        Text = descricaoAttrib != null ? descricaoAttrib.Description :
                                                             descriptAttrib != null ? descriptAttrib.Description :
                                                             f.Name,
                                                        Cor = descricaoAttrib != null ? descricaoAttrib.Cor : "",
                                                        Value = f.GetRawConstantValue().ToString()
                                                    }).ToList() : null
                           }).ToList();
            return retorno;
        }
        #endregion
    }
}