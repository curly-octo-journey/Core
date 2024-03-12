using Core6.Repositorio.Base.Contextos;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;

namespace Core6.Repositorio.Base.Repositorios
{
    public class RepositorioBase<TEntidade> : RepCon<TEntidade>, IRepositorioBase<TEntidade> where TEntidade : Identificador
    {
        public RepositorioBase(Contexto contexto)
            : base(contexto)
        {
        }
    }
}
