using Core6.Infra.Base.Auth;
using Core6.Infra.Base.Personalizacao;
using Core6.Repositorio.Base.Personalizacao;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Core6.Infra.Base.Http;

namespace Core6.Api.Base.Personalizacao
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/personalizacao")]
    public class PersonalizacaoController : ControllerCoreBase
    {
        private readonly IRepPersonalizacaoBanco _repPersonalizacaoBanco;
        public PersonalizacaoController(IRepPersonalizacaoBanco repPersonalizacaoBanco)
        {
            _repPersonalizacaoBanco = repPersonalizacaoBanco;
        }

        #region Listar
        [Route("")]
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var caches = PersonalizacaoCache.RecuperarDadosCache();
                return Ret(HttpStatusCode.OK, new UseHttpReaderMessage
                {
                    Content = caches,
                    Total = caches.Count
                });
            }
            catch (Exception e)
            {
                return RetErro(e.Message);
            }
        }
        #endregion

        #region Recarregar
        [Route("Recarregar/{codigoSistema}")]
        [HttpPost]
        public IActionResult Recarregar([FromRoute] int codigoSistema)
        {
            try
            {
                var codigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                var personalizacoesBanco = _repPersonalizacaoBanco.RecuperarPersonalizacao(codigoSistema);
                PersonalizacaoCache.Inserir(personalizacoesBanco);

                return RetSucesso("Cache de personalização recarregado com sucesso");
            }
            catch (Exception e)
            {
                return RetErro(e.Message);
            }
        }
        #endregion

        #region RecuperarPersonalizacao
        [Route("{codigoSistema}")]
        [HttpPost]
        public IActionResult RecuperarPersonalizacao([FromRoute] int codigoSistema)
        {
            try
            {
                var codigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                var personalizacao = _repPersonalizacaoBanco.RecuperarPersonalizacaoTela(codigoSistema);

                return RetSucesso(content: personalizacao);
            }
            catch (Exception e)
            {
                return RetErro(e.Message);
            }
        }
        #endregion
    }
}
