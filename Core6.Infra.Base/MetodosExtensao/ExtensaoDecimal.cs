namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoDecimal
    {
        /// <summary>
        ///     Método para realiza arredondamento
        /// </summary>
        /// <param name="value">Valor a ser arredondado</param>
        /// <param name="casasDecimais">Quantidade de casas decimais</param>
        /// <returns></returns>
        public static decimal uArredonda(this decimal value, int casasDecimais)
        {
            return Math.Round(value, casasDecimais, MidpointRounding.AwayFromZero);
        }
        /// <summary>
        ///     Método para realiza arredondamento de quantidade
        /// </summary>
        /// <param name="value">Valor a ser arredondado</param>
        /// <returns>Retorno um decimal com 4 casas decimais.</returns>
        public static decimal uArredondaQuantidade(this decimal value)
        {
            return value.uArredonda(4);
        }
        /// <summary>
        ///     Método para realiza arredondamento de valores monetários
        /// </summary>
        /// <param name="value">Valor a ser arredondado</param>
        /// <returns>Retorno um decimal com 2 casas decimais.</returns>
        public static decimal uArredondaMonetario(this decimal value)
        {
            return value.uArredonda(2);
        }
        /// <summary>
        ///     Método para realiza arredondamento de peso
        /// </summary>
        /// <param name="value">Valor a ser arredondado</param>
        /// <returns>Retorno um decimal com 4 casas decimais.</returns>
        public static decimal uArredondaPeso(this decimal value)
        {
            return value.uArredonda(4);
        }
        public static decimal uPower(this decimal values, decimal pow)
        {
            return (decimal)Math.Pow(double.Parse(values.ToString()), double.Parse(pow.ToString()));
        }
        public static decimal uTruncate(this decimal value)
        {
            return decimal.Truncate(value);
        }

        public static decimal uTruncate(this decimal value, int precision)
        {
            var step = (decimal)Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        public static bool uMaiorQue(this decimal value, decimal valor)
        {
            return value > valor;
        }
        public static bool uMaiorOuIgual(this decimal value, decimal valor)
        {
            return value >= valor;
        }
        public static bool uMenorQue(this decimal value, decimal valor)
        {
            return value < valor;
        }
        public static bool uMenorOuIgual(this decimal value, decimal valor)
        {
            return value <= valor;
        }

        public static decimal uDividir(this decimal value, decimal divisor)
        {
            return value.uDividir(divisor, 0);
        }

        public static decimal uDividir(this decimal value, decimal divisor, decimal resultDivisorZero)
        {
            return divisor == 0 ? resultDivisorZero : value / divisor;
        }
    }
}
