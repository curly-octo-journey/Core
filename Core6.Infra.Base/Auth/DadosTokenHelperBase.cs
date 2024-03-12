using Core6.Infra.Base.Criptografia;
using Core6.Infra.Base.DI;
using Core6.Infra.Base.MetodosExtensao;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Security.Claims;

namespace Core6.Infra.Base.Auth
{
    public class DadosTokenHelperBase
    {
        #region Ctor
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _configuration;

        public DadosTokenHelperBase(IHttpContextAccessor httpContext,
                                IConfiguration configuration)
        {
            _context = httpContext;
            _configuration = configuration;
        }

        #endregion

        #region Debug
        public static bool Debug
        {
            get
            {
                return Debugger.IsAttached;
            }
        }
        #endregion

        #region Identity
        private ClaimsIdentity Identity
        {
            get
            {
                try
                {
                    if (_context?.HttpContext?.User != null)
                    {
                        return _context.HttpContext.User.Identity as ClaimsIdentity;
                    }
                    return Thread.CurrentPrincipal != null ? Thread.CurrentPrincipal.Identity as ClaimsIdentity : null;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        #region RecuperarStringDeConexao
        public string RecuperarStringDeConexao()
        {
            try
            {
                if (Debug)
                {
                    var stringConexaoObject = DadosConexaoBase.Recuperar("StringConexao");
                    if (stringConexaoObject != null)
                    {
                        return stringConexaoObject.ToString();
                    }
                }

                var stringConexao = RecuperarStringConexaoNoClaim();
                if (string.IsNullOrWhiteSpace(stringConexao))
                {
                    throw new UnauthorizedAccessException("Não autorizado. String de conexão não encontrada. Verifique se o token foi enviado.");
                }
                return stringConexao;
            }
            catch (Exception exp)
            {
                throw new UnauthorizedAccessException(exp.uTratar());
            }
        }

        private string RecuperarStringConexaoNoClaim()
        {
            try
            {
                var identity = Identity;
                var claim = identity.Claims.FirstOrDefault(x => x.Type == "SC");
                if (claim != null)
                {
                    var stringDeConexaoCriptografada = claim.Value;
                    var stringConexao = CriptografiaSB2.Descriptografar(stringDeConexaoCriptografada, 21);
                    return stringConexao;
                }
            }
            catch
            {
            }
            return null;
        }
        #endregion

        #region RecuperarTenant
        public int RecuperarTenant()
        {
            if (Debug)
            {
                var codigoTenantObject = DadosConexaoBase.Recuperar("CodigoTenant");
                if (codigoTenantObject != null)
                {
                    return int.Parse(DadosConexaoBase.Recuperar("CodigoTenant").ToString());
                }
            }

            try
            {
                var tenant = RecuperarTenantNoClaim();
                if (tenant == null)
                {
                    throw new UnauthorizedAccessException("Não autorizado. Tenant não encontrado. Verifique se o token foi enviado.");
                }
                return tenant.Value;
            }
            catch (Exception exp)
            {
                throw new UnauthorizedAccessException(exp.uTratar());
            }
        }

        private int? RecuperarTenantNoClaim()
        {
            try
            {
                var identity = Identity;
                var claim = identity.Claims.FirstOrDefault(x => x.Type == "TENANT");
                if (claim != null)
                {
                    return int.Parse(claim.Value);
                }
            }
            catch
            {
            }
            return null;
        }
        #endregion

        #region RecuperarUsuario
        public int RecuperarUsuario()
        {
            if (Debug)
            {
                var codigoUsuarioObject = DadosConexaoBase.Recuperar("CodigoUsuario");
                if (codigoUsuarioObject != null)
                {
                    return int.Parse(codigoUsuarioObject.ToString());
                }
            }

            try
            {
                var codigoUsuario = RecuperarUsuarioNoClaim();
                if (codigoUsuario == null)
                {
                    throw new UnauthorizedAccessException("Não autorizado. Código do usuário não encontrado. Verifique se o token foi enviado.");
                }
                return codigoUsuario.Value;
            }
            catch (Exception exp)
            {
                throw new UnauthorizedAccessException(exp.uTratar());
            }
        }

        private int? RecuperarUsuarioNoClaim()
        {
            try
            {
                return int.Parse(Identity.Name);
            }
            catch
            {
            }
            return null;
        }
        #endregion

        #region RecuperarUrlApiCore
        public string RecuperarUrlApiCore()
        {
            try
            {
                var claimsIdentity = Identity;
                var urlApiCore = claimsIdentity.Claims.First(x => x.Type == "UrlApiCore").Value;
                return urlApiCore;
            }
            catch (Exception exp)
            {
                throw new Exception("Url da API do Core não encontrada. Erro: " + exp.Message);
            }
        }
        #endregion

        #region RecuperarHashDocumentos
        public string RecuperarHashDocumentos()
        {
            try
            {
                var hash = RecuperarHashDocumentosNoClaim();
                if (string.IsNullOrWhiteSpace(hash))
                {
                    throw new Exception("Hash de documentos não encontrada.");
                }
                return hash;
            }
            catch (Exception exp)
            {
                throw new Exception("Hash de documentos não encontrada. Erro= " + exp.uTratar());
            }
        }

        private string RecuperarHashDocumentosNoClaim()
        {
            try
            {
                var identity = Identity;
                var claim = identity.Claims.FirstOrDefault(x => x.Type == "HashDocumentos");
                if (claim != null)
                {
                    return claim.Value;
                }
            }
            catch
            {
            }
            return null;
        }
        #endregion

        #region Autenticado
        public bool Autenticado
        {
            get
            {
                if (Identity == null)
                    return false;

                return (bool)Identity?.Claims.Any();
            }
        }
        #endregion

        #region Logado
        public static DadosTokenHelperBase Dados()
        {
            DadosTokenHelperBase amb;

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var httpContext = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();
                var configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();

                amb = new DadosTokenHelperBase(httpContext, configuration);
            }

            return amb;
        }
        #endregion
    }
}