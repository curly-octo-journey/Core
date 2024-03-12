using System.ComponentModel;
using System.Reflection;

namespace Core6.Infra.Base
{
    public class ListEnum
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class ListEnumString
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public static class EnumHelper
    {
        public static string MontaDescricaoEnum(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = null;
            DescricaoAttribute[] attributosDescricao = null;
            try
            {
                attributosDescricao = (DescricaoAttribute[])fi.GetCustomAttributes(typeof(DescricaoAttribute), false);

                if (attributosDescricao.Length > 0)
                {
                    if (attributosDescricao != null && attributosDescricao.Length > 0)
                        return attributosDescricao[0].Description;
                    else
                        return "Descrição para " + value.ToString() + " não especificado";
                }
                else
                {
                    attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                        return attributes[0].Description;
                    else
                        return "Descrição para " + value.ToString() + " não especificado";
                }


            }
            catch (Exception)
            {
                return value.ToString() + " não é um valor válido para o Enum";
            }
        }

        public static string MontaDescricaoEnum<TEnum>(int value)
        {
            return MontaDescricaoEnum((Enum)(object)(TEnum)(object)value);
        }

        //public static List<SelectListItem> MontaListaEnum<TEnum>()
        //{

        //    var selectList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(v => new SelectListItem
        //    {
        //        Text = MontaDescricaoEnum((Enum)(object)((TEnum)(object)v)),
        //        Value = v.ToString()
        //    });

        //    return selectList.ToList();
        //}

        public static List<ListEnum> MontaListEnum<TEnum>()
        {

            var selectList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(v => new ListEnum
            {
                Text = MontaDescricaoEnum((Enum)(object)(TEnum)(object)v),
                Value = (int)(v as object)
            });

            return selectList.ToList();
        }

        //public static List<SelectListItem> MontaListaIntEnum<TEnum>()
        //{

        //    var selectList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(v => new SelectListItem
        //    {
        //        Text = MontaDescricaoEnum((Enum)(object)((TEnum)(object)v)),
        //        Value = ((int)(v as object)).ToString()
        //    });

        //    return selectList.ToList();
        //}
    }
}
