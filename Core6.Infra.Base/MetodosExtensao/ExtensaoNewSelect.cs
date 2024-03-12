using Core6.Infra.Base.MetodosExtensao.NewSelects;

namespace Core6.Infra.Base.MetodosExtensao
{
    public static class NewSelectExtension
    {
        [Obsolete("Use o método 'Qry'. Exp.: query.Qry.New<View>()")]
        public static NewSelect<TSource> To<TSource>(this IEnumerable<TSource> query) where TSource : class
        {
            return query.Qry();
        }

        [Obsolete("Use o método 'Obj'. Exp.: source.Obj.New<View>()")]
        public static NewSelect<TSource, TSource> NewSelect<TSource>(this TSource source) where TSource : class
        {
            return source.Obj();
        }

        public static NewSelect<TSource> Qry<TSource>(this IEnumerable<TSource> query) where TSource : class
        {
            return new NewSelect<TSource>(query.AsQueryable());
        }

        public static NewSelect<TSource, TSource> Obj<TSource>(this TSource source) where TSource : class
        {
            if (source == null)
                return null;

            var query = new List<TSource>() { source }.AsQueryable();
            return new NewSelect<TSource, TSource>(query);
        }
    }
}