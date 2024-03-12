using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Interfaces;

namespace Core6.Aplicacao.Base.Base
{
    public class AplicGuid<TEntidade, TRep> : Aplic where TEntidade : IdentificadorGuid
                                          where TRep : IRepGuid<TEntidade>
    {
        protected TRep Rep { get; private set; }

        public AplicGuid(IUnitOfWork unitOfWork, TRep rep) :
            base(unitOfWork)
        {
            Rep = rep;
        }
    }

    public class AplicGuid : AplicacaoBase, IAplic
    {
        public AplicGuid(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}
