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
    public abstract class AplicListaBase<TEntidade, TRep, TView> : Aplic<TEntidade, TRep>, IAplicListaBase<TView>
        where TEntidade : Identificador, new()
        where TRep : IRepCon<TEntidade>
        where TView : IdView, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        public AplicListaBase(IUnitOfWork unitOfWork, TRep rep) : base(unitOfWork, rep)
        {
            _unitOfWork = unitOfWork;
        }

        public virtual RetConView Listar(QueryParams queryParams)
        {
            var query = Rep.Recuperar();
            var retPaginado = new ConsultaHelper(_unitOfWork).Paginar<TEntidade, TView>(query, queryParams);
            var conteudoComPersonalizacao = _unitOfWork.MontarCamposPersonalizadosLista<TEntidade>(retPaginado.Conteudo.ToList());

            return new RetConView()
            {
                Conteudo = conteudoComPersonalizacao,
                Total = retPaginado.Total
            };
        }

        public virtual object Listar(int id)
        {
            var query = Rep.Recuperar();
            var retPaginado = new ConsultaHelper(_unitOfWork).Paginar<TEntidade, TView>(query, QueryParams.GetFiltroId(id));
            if (retPaginado.Conteudo != null && retPaginado.Conteudo.Any())
            {
                var conteudoComPersonalizacao = _unitOfWork.MontarCamposPersonalizadosLista<TEntidade>(retPaginado.Conteudo.ToList());
                return conteudoComPersonalizacao.FirstOrDefault();
            }
            return new { };
        }

        public virtual object Listar(int id, List<string> fields)
        {
            var query = Rep.Where(p => p.Id == id);
            var view = fields.uVazio() ? query.Qry().New<TView>().FirstOrDefault()
                                       : query.Qry().New(TypeBuilderHelper.CreateType(typeof(TView), fields)).FirstOrDefault();
            if (view == null)
                return null;

            return _unitOfWork.MontarCamposPersonalizadosLista<TEntidade>(view);
        }
    }
}
