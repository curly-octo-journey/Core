using Core6.Aplicacao.Base.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Entidades;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Dominio.Base.Interfaces;
using Core6.Infra.Base.Helpers.TypeBuilders;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using Core6.Aplicacao.Base.Base.Helpers.Consultas;

namespace Core6.Aplicacao.Base.Base
{
    public abstract class AplicListaGuidBase<TEntidade, TRep, TView> : AplicGuid<TEntidade, TRep>, IAplicListaGuidBase<TView>
        where TEntidade : IdentificadorGuid, new()
        where TRep : IRepConGuid<TEntidade>
        where TView : IdGuidView, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        public AplicListaGuidBase(IUnitOfWork unitOfWork, TRep rep) : base(unitOfWork, rep)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual RetConView Listar(QueryParams queryParams)
        {
            var query = Rep.Recuperar();
            var retPaginado = new ConsultaGuidHelper().Paginar<TEntidade, TView>(query, queryParams);
            var conteudoComPersonalizacao = _unitOfWork.MontarCamposPersonalizadosListaGuid<TEntidade>(retPaginado.Conteudo.ToList());

            return new RetConView()
            {
                Conteudo = conteudoComPersonalizacao,
                Total = retPaginado.Total
            };
        }

        public virtual object Listar(Guid id)
        {
            var query = Rep.Recuperar();
            var retPaginado = new ConsultaGuidHelper().Paginar<TEntidade, TView>(query, QueryParams.GetFiltroId(id));

            if (retPaginado.Conteudo != null && retPaginado.Conteudo.Any())
            {
                var conteudoComPersonalizacao = _unitOfWork.MontarCamposPersonalizadosListaGuid<TEntidade>(retPaginado.Conteudo.ToList());
                return conteudoComPersonalizacao.FirstOrDefault();
            }

            return new { };
        }

        public virtual object Listar(Guid id, List<string> fields)
        {
            var query = Rep.Where(p => p.Id == id);
            var view = fields.uVazio() ? query.Qry().New<TView>().FirstOrDefault()
                                       : query.Qry().New(TypeBuilderHelper.CreateType(typeof(TView), fields)).FirstOrDefault();
            if (view == null)
                return null;

            return _unitOfWork.MontarCamposPersonalizadosListaGuid<TEntidade>(view);
        }
    }
}
