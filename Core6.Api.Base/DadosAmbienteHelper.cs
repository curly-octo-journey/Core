using Core6.Infra.Base.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Core6.Api.Base
{
    public static class DadosAmbienteHelper
    {
        public static bool Debug
        {
            get
            {
                return Debugger.IsAttached;
            }
        }
        #region RecuperarEmpresaCabecalhoRequisicao
        public static int RecuperarEmpresaCabecalhoRequisicao(this ControllerBase controller)
        {
            if (Debug) return int.Parse(DadosConexaoBase.Recuperar("CodigoEmpresa").ToString());

            try
            {
                var headerTokenEmpresa = controller.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "useauth-empresa");
                return int.Parse(headerTokenEmpresa.Value.First());
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao recuperar código da empresa no cabeçalho da requisição. " + e.Message);
            }
        }
        #endregion

        #region RecuperarFilialCabecalhoRequisicao
        public static int RecuperarFilialCabecalhoRequisicao(this ControllerBase controller)
        {
            if (Debug) return int.Parse(DadosConexaoBase.Recuperar("CodigoFilial").ToString());

            try
            {
                var headerTokenFilial = controller.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "useauth-filial");
                return int.Parse(headerTokenFilial.Value.First());
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao recuperar código da filial no cabeçalho da requisição." + e.Message);
            }
        }
        #endregion

        //#region RecuperarSistemaCabecalhoRequisicao
        //public static int RecuperarSistemaCabecalhoRequisicao()
        //{
        //    if (Debug) return int.Parse(DadosConexaoBase.Recuperar("Sistema").ToString());

        //    try
        //    {
        //        return int.Parse(HttpContext.Current.Request.Headers["UseAuth-Sistema"]);
        //    }
        //    catch
        //    {
        //        return 505;
        //    }
        //}
        //#endregion
    }
}
