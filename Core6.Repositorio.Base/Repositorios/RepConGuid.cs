using Core6.Repositorio.Base.Contextos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Repositorio.Base.Repositorios
{
    public abstract class RepConGuid<TEntidade> : RepGuid<TEntidade>, IRepConGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        public RepConGuid(Contexto contexto)
            : base(contexto)
        {
        }

        public TEntidade Find(Guid id)
        {
            return DbSet.Find(id);
        }

        public List<TEntidade> Find(Guid? id1, Guid? id2, params Guid?[] id3)
        {
            var ids = new List<Guid?>();

            ids.Add(id1);
            ids.Add(id2);
            ids.AddRange(id3);

            return Find(ids);
        }

        public List<TEntidade> Find(IEnumerable<Guid?> ids, params string[] includes)
        {
            var values = ids.Where(p => p.HasValue)
                            .Select(p => p.Value)
                            .ToList();

            return Find(values, includes);
        }

        public List<TEntidade> Find(IEnumerable<Guid> ids, params string[] includes)
        {
            var values = ids.ToList();

            if (values.uVazio())
                return new List<TEntidade>();

            var entidades = DbSet.Local
                                 .Where(p => values.Contains(p.Id))
                                 .ToList();

            var listaDetached = values.Except(entidades.Select(p => p.Id))
                                      .ToList();

            if (listaDetached.uVazio())
                return entidades;

            foreach (var listaDetachedAg in listaDetached.uRepartidaListaPorNumeroMaxDeItens(1000))
            {
                entidades.AddRange(Recuperar(includes).Where(p => listaDetachedAg.Contains(p.Id)));
            }

            return entidades;
        }

        public List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, Guid?> exp, string include, params string[] includes)
        {
            var listaInclude = new List<string>(includes.Count() + 1);

            listaInclude.Add(include);
            listaInclude.AddRange(includes);

            return Find(lista.Select(exp), listaInclude.ToArray());
        }

        public List<TEntidade> Find<T>(IEnumerable<T> lista, Func<T, Guid?> exp, params Func<T, Guid?>[] exps)
        {
            var ids = new List<Guid?>();
            var regs = lista.ToList();

            ids.AddRange(regs.Select(exp).ToList());

            foreach (var item in exps)
            {
                ids.AddRange(regs.Select(item).ToList());
            }

            return Find(ids);
        }

        public TEntidade RecuperarPorId(Guid id, params string[] includes)
        {
            var query = Recuperar(includes);

            return query.SingleOrDefault(p => p.Id == id);
        }

        public TDest RecuperarPorId<TDest>(Guid id) where TDest : new()
        {
            return DbSet.Where(p => p.Id == id)
                        .To()
                        .New<TDest>()
                        .SingleOrDefault();
        }

        public TCampos RecuperarPorId<TCampos>(Guid id, Expression<Func<TEntidade, TCampos>> campos, params string[] includes)
        {
            return Recuperar(includes).Where(p => p.Id == id)
                                      .Select(campos)
                                      .ToList()
                                      .SingleOrDefault();
        }

        public TEntidade RecuperarPorIdObrigatorio(Guid id, string msg, params string[] includes)
        {
            var entidade = RecuperarPorId(id, includes);

            if (entidade == null)
                throw new Exception(msg);

            return entidade;
        }

        public TEntidade RecuperarPorIdOriginal(Guid id, params string[] includes)
        {
            var query = RecuperarOriginal(includes);

            try
            {
                return query.SingleOrDefault(p => p.Id == id);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Erro ao recuperar por ID. {0}", e.uTratar()));
            }
        }

        public IQueryable<TEntidade> RecuperarOriginal(params string[] includes)
        {
            var query = DbSet.AsQueryable()
                             .AsNoTracking();

            if (includes.uVazio())
                return query;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}