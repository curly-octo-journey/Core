using System.Transactions;

namespace Core6.Aplicacao.Base
{
    public static class TransactionScopeHelper
    {
        #region CriarTransactionScope
        public static TransactionScope CriarTransactionScope(TransactionScopeOption option = TransactionScopeOption.Required)
        {
            //TODO
            //if (UtilConexao.NomeBancoDados() == BancoDeDados.Postgres)
            //{
            return new TransactionScope(option, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead
            });
            //}
            //return new TransactionScope(option);
        }
        #endregion
    }
}
