using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Core6.Infra.Base.Integracao;
using Core6.Infra.Base.Integracao;
using Core6.Infra.Base.DI;
using Core6.Infra.Base.MetodosExtensao;

namespace Core6.Infra.Base.Personalizacao.Helper
{
    public static class HttpHelperPersonalizacao
    {
        public static void ChamarPersonalizacao(object obj, string url, int? codigoEmpresa = null, int? codigoFilial = null)
        {
            try
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    var httpContext = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();
                    var helper = new HttpHelper(httpContext);
                    if (codigoEmpresa.HasValue)
                    {
                        helper = new HttpHelper(httpContext, codigoEmpresa.Value);
                    }

                    if (codigoEmpresa.HasValue && codigoFilial.HasValue)
                    {
                        helper = new HttpHelper(httpContext, codigoEmpresa.Value, codigoFilial.Value);
                    }

                    var retorno = helper.PostAsync<UseHttpResponse, object>(url, obj);
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Tempo limite esgotado, tente novamente.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.uTratar());
            }
        }
    }
}
