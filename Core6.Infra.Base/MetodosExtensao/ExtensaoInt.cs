namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoInt
    {
        public static bool uIsNullOrZero(this int? value)
        {
            return value == null || value == 0;
        }
        public static bool uMaiorQue(this int value, int valor)
        {
            return value > valor;
        }
        public static bool uMaiorOuIgual(this int value, int valor)
        {
            return value >= valor;
        }
        public static bool uMenorQue(this int value, int valor)
        {
            return value < valor;
        }
        public static bool uMenorOuIgual(this int value, int valor)
        {
            return value <= valor;
        }

    }
}
