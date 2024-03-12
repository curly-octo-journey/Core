using System.Reflection;
using Core6.Infra.Base.MetodosExtensao;
using Microsoft.AspNetCore.Mvc;

namespace Core6.Api.Base.Health
{
    [Route("api/health")]
    public class HealthController : ControllerCoreBase
    {
        #region Health
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Health()
        {
            var health = new HealthDTO
            {
                DataInicio = HealthConfig.DataInicio,
                Duracao = HealthConfig.DataInicio == null ? TimeSpan.FromSeconds(0) : DateTime.Now.Subtract(HealthConfig.DataInicio.Value)
            };

            try
            {
                Assembly dllApi = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.GetName().Name.EndsWith(".Api"));

                string nome = "";
                int aplicacao = 0;
                int release = 0;
                int build = 0;
                int banco = 0;

                if (dllApi != null)
                {

                    var dataModificacao = System.IO.File.GetLastWriteTime(dllApi.Location);

                    aplicacao = dllApi.GetName().Version.Major;
                    release = dllApi.GetName().Version.Minor;
                    build = dllApi.GetName().Version.Build;
                    banco = dllApi.GetName().Version.Revision;
                    nome = dllApi.GetName().Name;
                    health.DataDll = dataModificacao;
                }

                var versao = string.Format("{0}.{1}.{2}/{3}", aplicacao, release, build, banco);

                health.Nome = nome;
                health.Versao = versao;

                health.Mensagem = "Executando";
                var resposta = RetSucesso(content: health);

                return resposta;
            }
            catch (Exception e)
            {
                health.Mensagem = e.uTratar();
                var respostaErro = RetErro(content: health);
                return respostaErro;
            }
        }
        #endregion
    }

    #region Dtos
    public class HealthDTO
    {
        public DateTime? DataInicio { get; set; }
        public string Mensagem { get; set; }
        public TimeSpan Duracao { get; set; }
        public string Nome { get; set; }
        public string Versao { get; set; }
        public DateTime DataDll { get; set; }
    }
    #endregion
}
