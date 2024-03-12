using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;

namespace Core6.Aplicacao.Base.Base
{
    public class Aplic<TEntidade, TRep> : Aplic where TEntidade : Identificador
                                          where TRep : IRep<TEntidade>
    {
        protected TRep Rep { get; private set; }

        public Aplic(IUnitOfWork unitOfWork, TRep rep) :
            base(unitOfWork)
        {
            Rep = rep;
        }
    }

    public class Aplic : AplicacaoBase, IAplic
    {
        public Aplic(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}
