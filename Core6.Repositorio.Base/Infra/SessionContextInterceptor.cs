using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Devart.Data.Oracle;
using Devart.Data.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Core6.Infra.Base.Auth;

namespace Core6.Repositorio.Base.Infra
{
    public class SessionContextInterceptor : IDbConnectionInterceptor
    {
        #region ConnectionClosed
        public void ConnectionClosed(DbConnection connection, ConnectionEndEventData eventData)
        {

        }
        #endregion

        #region ConnectionClosedAsync
        public Task ConnectionClosedAsync(DbConnection connection, ConnectionEndEventData eventData)
        {
            return Task.FromResult(0);
        }
        #endregion

        #region ConnectionClosing
        public InterceptionResult ConnectionClosing(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            return new InterceptionResult();
        }
        #endregion

        #region ConnectionClosingAsync
        public Task<InterceptionResult> ConnectionClosingAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            return null;
        }
        #endregion

        #region ConnectionFailed
        public void ConnectionFailed(DbConnection connection, ConnectionErrorEventData eventData)
        {

        }
        #endregion

        #region ConnectionFailedAsync
        public Task ConnectionFailedAsync(DbConnection connection, ConnectionErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
        #endregion

        #region ConnectionOpened
        public void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            try
            {
                var codigoTenant = DadosTokenHelperBase.Dados().RecuperarTenant();
                using var cmd = connection.CreateCommand();
                if (eventData.Context.Database.IsPostgreSql())
                {
                    using var cmd2 = new PgSqlCommand("set session use.idtenant = :IDTENANT", (PgSqlConnection)cmd.Connection);
                    cmd2.UnpreparedExecute = true;
                    cmd2.Parameters.Add(new PgSqlParameter("IDTENANT", PgSqlType.Int)
                    {
                        Value = codigoTenant
                    });
                    cmd2.ExecuteNonQuery();
                    cmd2.Dispose();
                }
                else if (eventData.Context.Database.IsOracle())
                {
                    cmd.CommandText = "call pkg_use_policy.set_tenant(:idtenant)";
                    cmd.Parameters.Add(new OracleParameter("idtenant", OracleDbType.Number)
                    {
                        Value = codigoTenant
                    });
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception exp)
            {
                //ignore
            }
        }
        #endregion

        #region ConnectionOpenedAsync
        public Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
        #endregion

        #region ConnectionOpening
        public InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            return new InterceptionResult();
        }
        #endregion

        #region ConnectionOpeningAsync
        public Task<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            return null;
        }

        ValueTask<InterceptionResult> IDbConnectionInterceptor.ConnectionClosingAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
        {
            throw new NotImplementedException();
        }

        ValueTask<InterceptionResult> IDbConnectionInterceptor.ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}