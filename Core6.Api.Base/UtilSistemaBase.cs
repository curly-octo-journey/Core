using System.Diagnostics;

namespace Core6.Api.Base
{
    public static class UtilSistemaBase
    {
        #region RecuperaDataJavascriptCache
        public static string RecuperaDataJavascriptCache(string pastaClienteSistema)
        {
            if (Debugger.IsAttached)
            {
                return DateTime.Now.ToString("ddMMyyyyHHmm");
            }
            var arquivoCompleto = Path.Combine(pastaClienteSistema, "app.js");
            if (!File.Exists(arquivoCompleto))
            {
                return new DateTime(1900, 1, 1).ToString("ddMMyyyyHHmm");
            }
            return File.GetLastWriteTime(arquivoCompleto).ToString("ddMMyyyyHHmm");
        }
        #endregion

        #region ValidacaoVersaoSistema
        public static void ValidacaoVersaoSistema(int versaoAplicacaoSistema, int versaoBuildSistema, int versaoReleaseSistema, int versaoBancoSistema,
                                                  int versaoAplicacaoBanco, int versaoBuildBanco, int versaoReleaseBanco, int versaoBancoBanco)
        {
            var versaoSistema = string.Format("{0}.{1}.{2}/{3}", versaoAplicacaoSistema, versaoReleaseSistema, versaoBuildSistema, versaoBancoSistema);
            var versaoBancoDados = string.Format("{0}.{1}.{2}/{3}", versaoAplicacaoBanco, versaoReleaseBanco, versaoBuildBanco, versaoBancoBanco);

            //Sistema desatualizado
            var msg = "Versão do sistema é menor que a versão do banco de dados.<br> " +
                      "Você deve atualizar o sistema para a nova versão.<br>" +
                      "Versão do banco de dados: " + versaoBancoDados + "<br> " +
                      "Sua versão de sistema: " + versaoSistema + "<br>" +
                      "Entre com contato com o administrador do sistema.";

            if (versaoBancoSistema > versaoBancoBanco)
            {
                string msgBanco = "Versão do sistema é maior que a versão do banco de dados.<br> " +
                                  "É necessário atualizar a estrutura do banco de dados.<br>" +
                                  "Versão do banco de dados: " + versaoBancoDados + "<br> " +
                                  "Sua versão de sistema: " + versaoSistema + "<br>" +
                                  "Entre com contato com o administrador do sistema.";
                throw new Exception(msgBanco);
            }
            if (versaoBancoBanco != versaoBancoSistema)
            {
                throw new Exception(msg);
            }
            if (versaoAplicacaoBanco != versaoAplicacaoSistema)
            {
                throw new Exception(msg);
            }
            if (versaoAplicacaoBanco == versaoAplicacaoSistema && versaoBuildBanco != versaoBuildSistema)
            {
                throw new Exception(msg);
            }
            if (versaoAplicacaoBanco == versaoAplicacaoSistema && versaoBuildBanco == versaoBuildSistema && versaoReleaseBanco != versaoReleaseSistema)
            {
                throw new Exception(msg);
            }
        }
        #endregion
    }
}
