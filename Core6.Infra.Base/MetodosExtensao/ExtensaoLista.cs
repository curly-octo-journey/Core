namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoLista
    {
        public static List<T> uNovaLista<T>(this T obj)
        {
            return new List<T>() { obj };
        }

        public static bool uVazio<T>(this IEnumerable<T> obj, Func<T, bool> exp)
        {
            return !obj.uAny(exp);
        }

        public static bool uVazio<T>(this IEnumerable<T> obj)
        {
            return obj.uVazio(p => true);
        }

        public static bool uAny<T>(this IEnumerable<T> obj, Func<T, bool> exp)
        {
            return obj != null && obj.Any(exp);
        }

        public static bool uAny<T>(this IEnumerable<T> obj)
        {
            return obj.uAny(p => true);
        }

        public static string uConcatena<T>(this IEnumerable<T> objs, string delimitador)
        {
            var retorno = objs.Aggregate<T, string>(null, (current, obj) => current + delimitador + obj.ToString());

            return objs.Any() ? retorno.Substring(retorno.StartsWith(",") ? 1 : 2) : null;
        }

        public static List<List<T>> uRepartidaListaPorNumeroMaxDeItens<T>(this List<T> lista, int numeroMaxItens)
        {
            var listaRepartida = new List<List<T>>();

            if (numeroMaxItens <= 1)
            {
                listaRepartida.Add(lista);

                return listaRepartida;
            }

            var numeroListas = Math.Ceiling((double)lista.Count / numeroMaxItens);

            if (numeroListas <= 0)
            {
                listaRepartida.Add(lista);

                return listaRepartida;
            }

            var numeroDeItensPorLista = (int)Math.Ceiling(lista.Count / numeroListas);

            for (var i = 0; i < numeroListas; i++)
            {
                listaRepartida.Add(lista.Skip(numeroDeItensPorLista * i)
                                        .Take(numeroDeItensPorLista)
                                        .ToList());
            }

            return listaRepartida;
        }
    }
}
