using System.Globalization;

namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoConversao
    {
        public static int uParse(this string str, string msgErro)
        {
            int numeroConv;

            if (int.TryParse(str, out numeroConv))
                return numeroConv;

            throw new Exception(msgErro);
        }

        public static DateTime uParse(this string str, string formato, string msgErro)
        {
            DateTime dataConv;

            var formatInfo = new DateTimeFormatInfo()
            {
                FullDateTimePattern = formato
            };

            if (DateTime.TryParse(str, formatInfo, DateTimeStyles.None, out dataConv))
                return dataConv;

            throw new Exception(msgErro);
        }
    }
}
