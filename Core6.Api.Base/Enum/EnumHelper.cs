using Core6.Infra.Base;
using System.ComponentModel;
using System.Reflection;

namespace Core6.Api.Base.Enum
{
    #region DTOs
    public class RetornoEnumDTO
    {
        public object Nome { get; set; }
        public List<EnumDTO> Itens { get; set; }
    }

    public class EnumDTO : ListEnum
    {
        public string Cor { get; set; }
        public string Identificador { get; set; }
    }
    #endregion

    public class EnumHelper
    {
        #region RecuperarTodos
        public static List<RetornoEnumDTO> RecuperarTodos(string nomeDll, string namespaceBase, params string[] namespaceExcludes)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains(nomeDll)).ToList();
            var types = assemblies.SelectMany(x => x.GetTypes().Where(y => y.IsEnum
                                                                        || y.Name.StartsWith("Enum"))).ToList();

            if (!string.IsNullOrEmpty(namespaceBase))
            {
                types = types.Where(p => p.FullName.StartsWith(namespaceBase)).ToList();
            }

            if (namespaceExcludes != null && namespaceExcludes.Any())
            {
                types.RemoveAll(x => namespaceExcludes.Any(y => (x.FullName ?? "").StartsWith(y)));
            }
            var retorno = (from t in types
                           select new RetornoEnumDTO
                           {
                               Nome = t.Name,
                               Itens = t.IsEnum ? (from f in t.GetFields()
                                                   where !f.Name.Equals("value__")
                                                   let descricaoAttrib = f.GetCustomAttribute(typeof(DescricaoAttribute)) as DescricaoAttribute
                                                   let descriptAttrib = f.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute
                                                   select new EnumDTO
                                                   {
                                                       Identificador = f.Name,
                                                       Text = descricaoAttrib != null ? descricaoAttrib.Description :
                                                              descriptAttrib != null ? descriptAttrib.Description :
                                                              f.Name,
                                                       Cor = descricaoAttrib != null ? descricaoAttrib.Cor : "",
                                                       Value = Convert.ToInt32(f.GetRawConstantValue())
                                                   }).ToList() :
                                       t.IsClass ? (from f in t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                                                    where !f.Name.Equals("value__")
                                                    let descricaoAttrib = f.GetCustomAttribute(typeof(DescricaoAttribute)) as DescricaoAttribute
                                                    let descriptAttrib = f.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute
                                                    select new EnumDTO
                                                    {
                                                        Identificador = f.Name,
                                                        Text = descricaoAttrib != null ? descricaoAttrib.Description :
                                                             descriptAttrib != null ? descriptAttrib.Description :
                                                             f.Name,
                                                        Cor = descricaoAttrib != null ? descricaoAttrib.Cor : "",
                                                        Value = Convert.ToInt32(f.GetRawConstantValue())
                                                    }).ToList() : null
                           }).ToList();
            return retorno;
        }
        #endregion
    }
}