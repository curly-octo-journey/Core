using System.ComponentModel;

namespace Core6.Dominio.Base.Entidades
{
    public class Identificador
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }

        public string GetIdentificacaoPersonalizacao()
        {
            var origemAttr = GetType().GetCustomAttributes(typeof(DescriptionAttribute), true);

            if (origemAttr.Count() == 0)
            {
                return null;
            }

            try
            {
                return ((DescriptionAttribute)origemAttr[0]).Description;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
