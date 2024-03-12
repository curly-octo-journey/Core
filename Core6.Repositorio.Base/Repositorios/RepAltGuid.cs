using Core6.Repositorio.Base.Contextos;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Repositorio.Base.Repositorios
{
    public class RepAltGuid<TEntidade> : RepConGuid<TEntidade>, IRepAltGuid<TEntidade> where TEntidade : IdentificadorGuid
    {
        protected RepAltGuid(Contexto contexto)
            : base(contexto)
        {
        }

        public virtual void Inserir(TEntidade obj)
        {
            Inserir(new List<TEntidade> { obj });
        }

        public virtual void Inserir(List<TEntidade> objs)
        {
            if (objs.uVazio())
                return;

            DbSet.AddRange(objs);
        }

        public void Remover(Guid id)
        {
            var obj = Find(id);
            Remover(obj);
        }

        public virtual void Remover(TEntidade obj)
        {
            DbSet.Remove(obj);
        }

        public virtual void Remover(List<TEntidade> objs)
        {
            DbSet.RemoveRange(objs);
        }
    }
}
