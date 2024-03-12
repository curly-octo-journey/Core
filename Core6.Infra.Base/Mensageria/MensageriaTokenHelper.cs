namespace Core6.Infra.Base.Mensageria
{
    public static class MensageriaTokenHelper
    {
        #region Criar
        public static string Criar(int codigoUsuario, string usuario, string nome, bool adm, int codigoTenant, string hash, string stringConexaoCriptografada)
        {
            return Criptografia.Criptografia.Criptografar(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7:ddMMyyyyHHmmss}", codigoUsuario, usuario, nome, adm, codigoTenant, hash, stringConexaoCriptografada, DateTime.Now));
        }
        #endregion
    }
}
