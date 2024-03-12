namespace Core6.Infra.Base.MetodosExtensao
{
    public static class ExtensaoLinq
    {
        public static IEnumerable<T> uDistinct<T, TProp>(this IEnumerable<T> lista, Func<T, TProp> prop)
        {
            var props = new HashSet<TProp>();
            foreach (var obj in lista)
            {
                if (props.Add(prop(obj)))
                    yield return obj;
            }
        }
    }
}
