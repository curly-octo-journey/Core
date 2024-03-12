using Core6.Infra.Base.Helpers.TypeBuilders;
using Core6.Infra.Base.Http;
using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Core6.Api.Base
{
    public abstract class ControllerCoreBase<TAplic> : ControllerCoreBase
    {
        protected TAplic Aplic { get; set; }
        protected ControllerCoreBase(TAplic aplic)
        {
            Aplic = aplic;
        }
    }

    public class ControllerCoreBase : ControllerBase
    {
        #region Ret
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Ret<TResposta>(HttpStatusCode statusCode, TResposta resposta)
        {
            return new ObjectResult(resposta)
            {
                StatusCode = (int)statusCode
            };
        }
        #endregion

        #region RetErro
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("RetErro1Parametro")]
        public IActionResult RetErro(UseHttpErrorMessage reposta)
        {
            return BadRequest(reposta);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("RetErro3Parametros")]
        public IActionResult RetErro(string mensagem = "", object content = null, int? erroCore = null)
        {
            if (erroCore.HasValue)
            {
                return RetErro(new UseHttpErrorMessage { Message = mensagem, Content = content, Type = "ErroCore" + erroCore });
            }
            return RetErro(new UseHttpErrorMessage { Message = mensagem, Content = content });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("RetErroException")]
        public IActionResult RetErro(Exception exp)
        {
            return RetErro(exp.uTratar());
        }
        #endregion

        #region RetSucesso
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("RetErro1Parametro")]
        public IActionResult RetSucesso(UseHttpSuccessMessage reposta)
        {
            return Ok(reposta);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("RetErro3Parametros")]
        public IActionResult RetSucesso(string mensagem = "", object content = null, int? codeRequest = null)
        {
            if (codeRequest.HasValue)
            {
                return RetSucesso(new UseHttpSuccessMessage
                {
                    Message = mensagem,
                    Content = content ?? new { },
                    CodeRequest = codeRequest
                });
            }
            return RetSucesso(new UseHttpSuccessMessage
            {
                Message = mensagem,
                Content = content ?? new { }
            });
        }
        #endregion

        #region ExecutarPaginacao
        private static List<object> ExecutarPaginacao(IQueryable<object> query, int page, int limit)
        {
            return query.Skip((page - 1) * limit).Take(limit).ToList();
        }
        #endregion

        [ApiExplorerSettings(IgnoreApi = true)]
        public static ExecutarConsultaQueryDTO MontaQueryFinal<T>(QueryParams queryParams, IQueryable<T> query) where T : class
        {
            query = MontaOrdem(queryParams, query);

            IQueryable<object> queryFinal = query as IQueryable<object>;
            var type = query.ElementType;
            var fields = queryParams.GetFields();
            if (fields.Count > 0)
            {
                queryFinal = query.Qry().New(TypeBuilderHelper.CreateType(typeof(T), fields));
            }
            var total = queryFinal.Count();

            return new ExecutarConsultaQueryDTO
            {
                Dados = queryFinal,
                Total = total
            };
        }

        private static IQueryable<T> MontaOrdem<T>(QueryParams queryParams, IQueryable<T> query)
        {
            var ordem = queryParams.GetOrdem();
            if (ordem == null)
            {
                ordem = new List<OrdemClass>();
                ordem.Add(new OrdemClass() { property = "Id", direction = "asc" });
            }

            var cons = new OrderByQuery();
            query = cons.MontaOrderBy(query, ordem);
            return query;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static ExecutarConsultaQueryDTO MontaQueryFinalPaginada<T>(QueryParams queryParams, IQueryable<T> query) where T : class
        {
            var queryFinal = MontaQueryFinal(queryParams, query);
            var content = ExecutarPaginacao(queryFinal.Dados, queryParams.page ?? 0, queryParams.limit ?? 25);
            return new ExecutarConsultaQueryDTO
            {
                Dados = content.AsQueryable(),
                Total = queryFinal.Total
            };
        }
    }

    public class ExecutarConsultaQueryDTO
    {
        public IQueryable<object> Dados { get; set; }
        public int Total { get; set; }
    }
}