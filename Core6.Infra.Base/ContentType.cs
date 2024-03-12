using Microsoft.Win32;

namespace Core6.Infra.Base
{
    public static class ContentType
    {
        #region BuscarPorExtensao
        public static string BuscarPorExtensao(string extensao)
        {
            string contentType = "application/octet-stream";
            if (extensao == null || extensao.Trim() == "")
            {
                return contentType;
            }
            if (extensao[0] != '.')
            {
                extensao = "." + extensao;
            }
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(extensao);
                contentType = key.GetValue("Content Type").ToString();
            }
            catch (Exception)
            {
            }

            return contentType;
        }
        #endregion

        #region BuscarPorContentType
        public static string BuscarPorContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return null;
            }
            var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + contentType, false);
            var value = key != null ? key.GetValue("Extension", null) : null;
            return value != null ? value.ToString().Replace(".", string.Empty) : string.Empty;
        }
        #endregion
    }
}
