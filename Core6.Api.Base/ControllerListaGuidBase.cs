using Core6.Aplicacao.Base.Base;
using Core6.Dominio.Base.Helpers.Consultas.View;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Core6.Infra.Base.Http;

namespace Core6.Api.Base
{
    public class ControllerListaGuidBase<TAplic, TView> : ControllerCoreBase<TAplic>
        where TView : IdGuidView, new()
        where TAplic : IAplicListaGuidBase<TView>
    {
        public ControllerListaGuidBase(TAplic aplic) : base(aplic)
        {
        }

        [HttpGet]
        public virtual IActionResult Listar([FromQuery] QueryParams queryParams)
        {
            try
            {
                var ret = Aplic.Listar(queryParams);
                return Ret(HttpStatusCode.OK, new UseHttpReaderMessage
                {
                    Content = ret.Conteudo,
                    Total = ret.Total
                });
            }
            catch (Exception exp)
            {
                return RetErro(exp);
            }
        }

        [Route("{id}")]
        [HttpGet]
        public virtual IActionResult Listar([FromRoute] Guid id, [FromQuery] QueryField dto)
        {
            try
            {
                var fields = dto == null ? null : dto.GetFields();
                var reg = fields.uVazio() ? Aplic.Listar(id)
                                          : Aplic.Listar(id, fields);
                return RetSucesso(content: reg);
            }
            catch (Exception exp)
            {
                return RetErro(exp);
            }
        }
    }
}