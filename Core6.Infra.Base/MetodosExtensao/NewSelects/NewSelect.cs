using System.Linq.Expressions;

namespace Core6.Infra.Base.MetodosExtensao.NewSelects
{
    public class NewSelect<TSource, TObj> : NewSelect<TSource> where TSource : class
    {
        public NewSelect(IQueryable<TSource> query)
            : base(query)
        {
        }

        public new TDest New<TDest>() where TDest : new()
        {
            var gerador = new GeradorNewSelect();
            var newSelect = gerador.Gerar<TSource, TDest>();

            return Query.Select(newSelect)
                .FirstOrDefault();
        }
    }

    public class NewSelect<TSource> where TSource : class
    {
        public NewSelect(IQueryable<TSource> query)
        {
            Query = query;
        }

        public IQueryable<TSource> Query { get; private set; }

        public IQueryable<TDest> New<TDest>() where TDest : new()
        {
            var gerador = new GeradorNewSelect();
            var newSelect = gerador.Gerar<TSource, TDest>();

            return Query.Select(newSelect);
        }

        public IQueryable<dynamic> New(Type typeResult)
        {
            var gerador = new GeradorNewSelect();
            var newSelect = gerador.Gerar<TSource>(typeResult);

            return Query.Select(newSelect);
        }

        public Expression<Func<TSource, TDest>> Expression<TDest>() where TDest : new()
        {
            var gerador = new GeradorNewSelect();
            return gerador.Gerar<TSource, TDest>();
        }

        public Func<TSource, TDest> Compile<TDest>() where TDest : new()
        {
            var gerador = new GeradorNewSelect();
            return gerador.Gerar<TSource, TDest>()
                .Compile();
        }
    }
}
