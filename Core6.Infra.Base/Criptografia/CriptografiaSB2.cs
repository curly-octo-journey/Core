namespace Core6.Infra.Base.Criptografia
{
    /// <summary>
    /// A classe <c>UCriptografiaSB2</c> faz o gerenciamento da criptografia do setor SB2.
    /// </summary>
    public static class CriptografiaSB2
    {
        const string tabelaOriginal = " 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string tabelaPares = " 3BdVWXYpqrEFijklZnoOPQ45R12ICDxyzAe0JSTU67KLmNgh89abuvMcfwstGH";
        const string tabelaImpares = " oOPQ76TUrLMNgh89ab45eX2ICDxyzAR03BKEqFdVYpijklmnJSuvZcfwstGHW1";

        /// <summary>
        /// A função <c>Criptografar</c> criptografa a string do parâmetro.
        /// </summary>
        /// <example>O bloco de código seguinte é um exemplo
        /// de utilização da função <c>Criptografar</c>:
        /// <code>
        /// var retorno =  CriptografiaSB2.Criptografar("valor", 5);
        /// </code>
        /// <param name="texto">Texto a ser criptografado.</param>
        /// <param name="niveis">Níveis em que o texto será criptografado.</param>
        /// <returns>Uma string com o parametro <paramref name="texto"/> criptografado.</returns>
        /// </example>
        public static string Criptografar(string texto, int niveis)
        {
            if (texto == null)
            {
                return "";
            }
            int p;
            string result = "";

            for (int i = 0; i < texto.Length; i++)
            {
                p = tabelaOriginal.IndexOf(texto[i]);
                if (p == -1)
                {
                    result = result + texto[i];
                }
                else
                {
                    if ((i + 1) % 2 == 0)
                    {
                        result = result + tabelaPares.Substring(p, 1);
                    }
                    else
                    {
                        result = result + tabelaImpares.Substring(p, 1);
                    }
                }
            }
            if (niveis > 1)
            {
                result = Criptografar(result, niveis - 1);
            }
            return result;
        }

        /// <summary>
        /// A função <c>Descriptografar</c> descriptografa a string do parâmetro.
        /// </summary>
        /// <example>O bloco de código seguinte é um exemplo
        /// de utilização da função <c>Descriptografar</c>:
        /// <code>
        /// var retorno =  CriptografiaSB2.Descriptografar("valor", 5);
        /// </code>
        /// <param name="texto">Texto a ser descriptografado.</param>
        /// <param name="niveis">Níveis em que o texto será descriptografado.</param>
        /// <returns>Uma string com o parametro <paramref name="texto"/> descriptografado.</returns>
        /// </example>
        public static string Descriptografar(string texto, int niveis)
        {
            if (texto == null)
            {
                return "";
            }
            int p;
            string result = "";

            for (int i = 0; i < texto.Length; i++)
            {
                p = tabelaOriginal.IndexOf(texto[i]);
                if (p == -1)
                {
                    result = result + texto[i];
                }
                else
                {
                    if ((i + 1) % 2 == 0)
                    {
                        result = result + tabelaOriginal.Substring(tabelaPares.IndexOf(texto[i]), 1);
                    }
                    else
                    {
                        result = result + tabelaOriginal.Substring(tabelaImpares.IndexOf(texto[i]), 1);
                    }
                }
            }
            if (niveis > 1)
            {
                result = Descriptografar(result, niveis - 1);
            }

            return result;
        }
    }
}
