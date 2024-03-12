using Core6.Infra.Base.MetodosExtensao;
using Core6.Infra.Base.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Core6.Infra.Base.Json;

namespace Core6.Infra.Base.Integracao
{
    public class HttpHelper
    {
        #region ctor
        private int? CodigoEmpresa { get; set; }
        private int? CodigoFilial { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HttpHelper(IHttpContextAccessor httpContextAccessor, int codigoEmpresa)
        {
            _httpContextAccessor = httpContextAccessor;
            CodigoEmpresa = codigoEmpresa;
        }

        public HttpHelper(IHttpContextAccessor httpContextAccessor, int codigoEmpresa, int codigoFilial)
        {
            _httpContextAccessor = httpContextAccessor;
            CodigoEmpresa = codigoEmpresa;
            CodigoFilial = codigoFilial;
        }
        #endregion

        public TimeSpan TimeOut = TimeSpan.FromSeconds(60);

        #region GetHttpHandler
        public HttpMessageHandler GetHttpHandler()
        {
            HttpMessageHandler defaultHttpHandler;

#if DEBUG
            defaultHttpHandler = new HttpLoggingHandler(new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = GetCookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
#else
            defaultHttpHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = GetCookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
#endif
            return defaultHttpHandler;
        }
        #endregion

        #region GetCookieContainer
        public virtual CookieContainer GetCookieContainer()
        {
            return new CookieContainer();
        }
        #endregion

        #region CriarClientBase
        public HttpClient CriarClientBase(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Url para requisição não foi informada.");
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new UseDateTimeConversor() }
            };

            var client = new HttpClient(GetHttpHandler())
            {
                BaseAddress = new Uri(url),
                Timeout = TimeOut
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            return client;
        }
        #endregion

        #region RequestToken
        public virtual string RecuperarToken()
        {
            if (_httpContextAccessor.HttpContext == null) return null;

            //TODO
            var request = _httpContextAccessor.HttpContext.Request;
            StringValues queryValBearer;
            if (request.Query.TryGetValue("BearerToken", out queryValBearer))
            {
                var tokenUrl = queryValBearer.ToString();
                if (!string.IsNullOrEmpty(tokenUrl))
                {
                    return tokenUrl;
                }
            }

            var tokenHeader = request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(tokenHeader))
            {
                return tokenHeader;
            }

            var tokenCookie = request.Cookies["BearerToken"];
            if (tokenCookie != null)
            {
                return tokenCookie;
            }

            return null;
        }
        #endregion

        #region CriarClient
        public virtual HttpClient CriarClient(string url)
        {
            var client = CriarClientBase(url);
            var token = RecuperarToken();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Add("Authorization", token);
            }

            if (CodigoEmpresa.HasValue)
            {
                client.DefaultRequestHeaders.Add("UseAuth-Empresa", CodigoEmpresa.Value.ToString());
            }

            if (CodigoFilial.HasValue)
            {
                client.DefaultRequestHeaders.Add("UseAuth-Filial", CodigoFilial.Value.ToString());
            }

            return client;
        }
        #endregion

        #region CriaParametros
        public QueryParams CriaParametros(List<string> fields = null,
                                                 GrupoFiltroQuery filtroGroup = null,
                                                 List<string> includes = null,
                                                 List<OrdemClass> ordem = null,
                                                 int qtdCamposPorPagina = 20,
                                                 int numeroPagina = 1,
                                                 int inicio = 0)
        {
            if (fields == null)
            {
                fields = new List<string>();
            }

            if (filtroGroup == null)
            {
                filtroGroup = new GrupoFiltroQuery();
            }

            return new QueryParams
            {
                fields = JsonConvert.SerializeObject(fields.ToArray()),
                filter = JsonConvert.SerializeObject(filtroGroup.Filtros.ToArray()),
                includes = includes == null ? "" : JsonConvert.SerializeObject(includes.ToArray()),
                limit = qtdCamposPorPagina,
                page = numeroPagina,
                sort = ordem == null ? "" : JsonConvert.SerializeObject(ordem.ToArray()),
                start = inicio
            };
        }
        #endregion

        #region PostAsync
        public T PostAsync<T, TObj>(string url, TObj obj)
        {
            try
            {
                using (var client = CriarClient(url))
                {
                    HttpResponseMessage response;

                    if (obj != null)
                    {
                        HttpContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        response = client.PostAsync(url, content).Result;
                    }
                    else
                    {
                        var param = new List<KeyValuePair<string, string>>();
                        response = client.PostAsync(url, new FormUrlEncodedContent(param)).Result;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(TratarErroComHttpMessage(response));
                    }

                    try
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(TratarErro(e));
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Ocorreu um problema na comunicação, verifique a conexão com a internet.");
            }
            catch (Exception ex)
            {
                throw new Exception(TratarErro(ex));
            }
        }
        #endregion

        #region GetAsync
        public T GetAsync<T>(string url)
        {
            try
            {
                using (var client = CriarClient(url))
                {
                    HttpResponseMessage response;

                    var param = new List<KeyValuePair<string, string>>();
                    response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(TratarErroComHttpMessage(response));
                    }

                    try
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(TratarErro(e));
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Ocorreu um problema na comunicação, verifique a conexão com a internet.");
            }
            catch (Exception ex)
            {
                throw new Exception(TratarErro(ex));
            }
        }

        #region PutAsync
        public T PutAsync<T, TObj>(string url, TObj obj)
        {
            try
            {
                using (var client = CriarClient(url))
                {
                    HttpResponseMessage response;

                    if (obj != null)
                    {
                        HttpContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        response = client.PutAsync(url, content).Result;
                    }
                    else
                    {
                        var param = new List<KeyValuePair<string, string>>();
                        response = client.PutAsync(url, new FormUrlEncodedContent(param)).Result;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(TratarErroComHttpMessage(response));
                    }

                    try
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<T>(content);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(TratarErro(e));
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Ocorreu um problema na comunicação, verifique a conexão com a internet.");
            }
            catch (Exception ex)
            {
                throw new Exception(TratarErro(ex));
            }
        }
        #endregion

        public byte[] GetByteArrayAsync(string url)
        {
            try
            {
                using (var client = CriarClient(url))
                {
                    var response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(TratarErroComHttpMessage(response));
                    }

                    try
                    {
                        using (var ms = new MemoryStream())
                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            var buffer = new byte[16 * 1024];
                            var isMoreToRead = true;

                            do
                            {
                                var read = stream.ReadAsync(buffer, 0, buffer.Length).Result;
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                }
                                else
                                {
                                    ms.Write(buffer, 0, read);
                                }
                            } while (isMoreToRead);

                            return ms.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(TratarErro(ex));
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Ocorreu um problema na comunicação, verifique a conexão com a internet.");
            }
            catch (Exception ex)
            {
                throw new Exception(TratarErro(ex));
            }
        }
        #endregion

        #region DeleteAsync
        public void DeleteAsync<TObj>(string url, TObj obj)
        {
            try
            {
                using (var client = CriarClient(url))
                {
                    HttpResponseMessage response;

                    if (obj != null)
                    {
                        HttpContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        var request = new HttpRequestMessage(HttpMethod.Delete, url);
                        request.Content = content;
                        response = client.SendAsync(request).Result;
                    }
                    else
                    {
                        response = client.DeleteAsync(url).Result;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(TratarErroComHttpMessage(response));
                    }

                    try
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(TratarErro(e));
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new Exception("Ocorreu um problema na comunicação, verifique a conexão com a internet.");
            }
            catch (Exception ex)
            {
                throw new Exception(TratarErro(ex));
            }
        }
        #endregion

        #region TratarErro
        public string TratarErro(Exception ex)
        {
            if (ex.GetType() == typeof(TaskCanceledException))
            {
                return "Servidor demorou muito para enviar uma resposta.";
            }

            return ex.uMsgCompleta();
        }

        public string TratarErroComHttpMessage(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return "Endereço não encontrado";
                case HttpStatusCode.Forbidden:
                    return "Sem premissão para acessar o recurso.\n Verifique sua conexão com a internet.";
                case HttpStatusCode.Unauthorized:
                    return "Não autorizado, verifique se o token foi enviado.";
            }

            var msg = GetMessageFromResponse(response).Result;
            if (string.IsNullOrEmpty(msg))
            {
                msg = "Não foi possível obter uma resposta válida do servidor.\nVerifique seu acesso a internet.";
            }

            return msg;
        }
        #endregion

        #region GetMessageFromResponse
        public async Task<string> GetMessageFromResponse(HttpResponseMessage response)
        {
            try
            {
                var content = await GetJsonInfoFromResponse(response, "Message", "ExceptionMessage", "invalid_grant", "error_description");
                return content;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao decodificar JSON.\n" + ex.Message);
            }
        }

        private async Task<string> GetJsonInfoFromResponse(HttpResponseMessage response, params string[] jsonInfos)
        {
            var resp = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(resp);
            return jsonInfos.Aggregate<string, string>(null, (current, jsonInfo) => current + obj[jsonInfo] + Environment.NewLine);
        }
        #endregion
    }
}
