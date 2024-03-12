using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoString
    {
        public static string uRemoveAcentuacao(this string str)
        {
            if (str == null)
                return str;

            StringBuilder sbReturn = new StringBuilder();
            var arrayText = str.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }

        public static string uSomenteNumeros(this string str)
        {
            return str.uSomenteNumeros(string.Empty);
        }

        public static string uSomenteNumeros(this string str, string substituir)
        {
            if (str == null)
                return str;

            return Regex.Replace(str, @"[^\d]", substituir);
        }

        public static bool uTemSomenteNumeros(this string str)
        {
            if (str == null)
                return false;

            return Regex.IsMatch(str, @"^\d+$");
        }

        public static string uSomenteLetras(this string str)
        {
            return str.uSomenteLetras(string.Empty);
        }

        public static string uSomenteLetras(this string str, string substituir)
        {
            if (str == null)
                return str;

            return Regex.Replace(str, @"[^a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]+", substituir);
        }

        public static string uSomenteLetrasNumeros(this string str)
        {
            return str.uSomenteLetrasNumeros(string.Empty);
        }

        public static string uSomenteLetrasNumeros(this string str, string substituir)
        {
            if (str == null)
                return str;

            return Regex.Replace(str, @"[^a-zA-Z0-9áàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ]+", substituir);
        }

        public static bool uVazio(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Método Core.
        /// Funciona parecido com o método padrão "String.Substring"
        /// com a diferença que não da exceção quando o tamanho passado por parâmetro é maior que o tamanho da string.        
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string uCopia(this string str, int length)
        {
            return str.uCopia(0, length);
        }

        /// <summary>
        /// Método Core.
        /// Funciona parecido com o método padrão "String.Substring"
        /// com a diferença que não da exceção quando o tamanho passado por parâmetro é maior que o tamanho da string.        
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string uCopia(this string str, int index, int length)
        {
            if (str == null)
                return null;

            if (index > str.Length - 1)
                return null;

            var tamanho = index + length;

            if (tamanho > str.Length)
                length -= tamanho - str.Length;

            return str.Substring(index, length);
        }

        public static Stream uToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static int uConvStrToInt(this string value)
        {
            var numeros = value.uGetSoNumeros();
            return int.TryParse(numeros, out var resultado) ? resultado : 0;
        }

        public static string uGetSoNumeros(this string value)
        {
            return value == null ? null : string.Join("", Regex.Split(value, @"[^\d]"));
        }

        public static string uFmt(this string value, params object[] args)
        {
            return string.Format(value, args);
        }
    }
}
