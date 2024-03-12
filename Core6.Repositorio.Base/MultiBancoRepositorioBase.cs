using Devart.Data.Oracle;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Transactions;
using Devart.Data.PostgreSql;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Core6.Infra.Base.Oracle;
using Core6.Repositorio.Base.Infra;

namespace Core6.Repositorio.Base
{
    public static class MultiBancoRepositorioBase
    {
        #region UseMultiBancoCore
        public static DbContextOptionsBuilder UseMultiBancoCore(this DbContextOptionsBuilder options, string stringConexao, IConfiguration configuration)
        {
            return options.UseMultiBancoCore(stringConexao, configuration, new SessionContextInterceptor());
        }

        public static DbContextOptionsBuilder UseMultiBancoCore(this DbContextOptionsBuilder options, string stringConexao, IConfiguration configuration, IDbConnectionInterceptor interceptor)
        {
            if (stringConexao?.ToLower().Contains("initial schema") == true)
            {
                var stringPostgres = new PgSqlConnectionStringBuilder(stringConexao)
                {
                    LicenseKey = LicencaDevartUtil.KEY_POSTGRES,
                    Unicode = true
                };
                options.UsePostgreSql(stringPostgres.ToString());
            }
            else
            {
                var stringOracle = new OracleConnectionStringBuilder(stringConexao)
                {
                    LicenseKey = LicencaDevartUtil.KEY_ORACLE
                };
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                 && stringOracle.Direct == false)
                {
                    //var dadosOracle = configuration.GetSection("DadosOracle").Get<List<DadosOracleBancoTenant>>();
                    //var dadoOracle = dadosOracle.FirstOrDefault(x => x.Nome.ToLower() == stringOracle.Server.ToLower());
                    //if (dadoOracle != null)
                    //{
                    //    stringOracle.Direct = true;
                    //    stringOracle.Server = dadoOracle.Host;
                    //    stringOracle.Port = dadoOracle.Port;
                    //    stringOracle.ServiceName = dadoOracle.ServiceName;
                    //}
                }
                options.UseOracle(stringOracle.ToString());
            }
            if (interceptor != null)
            {
                options.AddInterceptors(interceptor);
            }
            return options;
        }
        #endregion

        #region GerarSequencia
        public static int GerarSequencia(this DbContext entidades, string nomeSequencia)
        {
            if (entidades.Database.IsOracle())
            {
                var resp = entidades.Executar($"SELECT {nomeSequencia.ToLower()}.NEXTVAL as \"id\" FROM DUAL").First();
                return (int)resp.id;
            }
            if (entidades.Database.IsPostgreSql())
            {
                var resp = entidades.Executar($"select nextval('{nomeSequencia.ToLower()}') as id from (SELECT * FROM GENERATE_SERIES(1, 1) as seq) as GERADOR order by GERADOR.seq").First();
                return (int)resp.id;
            }
            throw new NotImplementedException();
        }
        #endregion

        #region Executar Sql
        public static IEnumerable<dynamic> Executar(this DbContext entidades, string sql)
        {
            var lista = new List<dynamic>();
            using (var cmd = entidades.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                entidades.Database.OpenConnection();
                try
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            {
                                row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                            }
                            lista.Add(row);
                        }
                        return lista;
                    }
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open) { cmd.Connection.Close(); }
                }
            }
        }

        public static List<T> SqlQuery<T>(this DbContext entidades, string sql, params object[] parametros) where T : class, new()
        {
            var conexao = entidades.Database.GetDbConnection();
            using (var cmd = conexao.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                if (parametros.Any())
                {
                    foreach (var parametro in parametros)
                    {
                        cmd.Parameters.Add(parametro);
                    }
                }

                entidades.Database.OpenConnection();

                using (var reader = cmd.ExecuteReader())
                {
                    var lst = new List<T>();
                    var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                    while (reader.Read())
                    {
                        var newObject = new T();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));
                            if (prop == null)
                            {
                                continue;
                            }
                            var val = reader.IsDBNull(i) ? null : reader[i];
                            prop.SetValue(newObject, val, null);
                        }
                        lst.Add(newObject);
                    }
                    return lst;
                }
            }
        }
        #endregion

        #region GerarSequenciaTenant
        public static void CriarSequence(this DbContext entidades, string nomeSequencia)
        {
            if (entidades.Database.IsOracle())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    DroparSequenceOracle(nomeSequencia, entidades);
                    CriarSequenceOracle(nomeSequencia, entidades);

                    scope.Complete();
                }
            }
            else if (entidades.Database.IsPostgreSql())
            {
                CriarSequencePostgre(nomeSequencia, entidades);
            }
            else
            {
                throw new Exception("Banco de dados não preparado para a criação de Sequences.");
            }
        }

        private static void CriarSequencePostgre(string nomeSequencia, DbContext entidades)
        {
            var sqlSequence = string.Format("CREATE SEQUENCE IF NOT EXISTS {0} INCREMENT 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1", nomeSequencia);
            entidades.Database.ExecuteSqlRaw(sqlSequence);
        }

        private static void CriarSequenceOracle(string nomeSequencia, DbContext entidades)
        {
            var sqlSequenceOracle = string.Format("CREATE SEQUENCE {0} START WITH 1 INCREMENT BY 1 NOCACHE", nomeSequencia);
            entidades.Database.ExecuteSqlRaw(sqlSequenceOracle);
        }

        private static void DroparSequenceOracle(string nomeSequencia, DbContext entidades)
        {
            var sqlDropSequenceOracle = string.Format("DECLARE" +
                                                      "   ANEXISTE NUMBER; " +
                                                      "BEGIN " +
                                                      "   SELECT COUNT(*) " +
                                                      "     INTO ANEXISTE " +
                                                      "     FROM USER_SEQUENCES " +
                                                      "    WHERE SEQUENCE_NAME = '{0}'; " +
                                                      " " +
                                                      "IF (ANEXISTE > 0) THEN " +
                                                      "    EXECUTE IMMEDIATE 'DROP SEQUENCE {0}'; " +
                                                      "END IF; " +
                                                      "END;", nomeSequencia);
            entidades.Database.ExecuteSqlRaw(sqlDropSequenceOracle);
        }

        public static int GerarSequenciaTenant(this DbContext entidades, string nomeSequencia, int codigoTenant)
        {
            var nomeSequenciaTenant = string.Format("{0}_{1}", nomeSequencia, codigoTenant);
            try
            {
                if (entidades.Database.IsPostgreSql())
                {
                    entidades.CriarSequence(nomeSequenciaTenant);
                }
                return entidades.GerarSequencia(nomeSequenciaTenant);
            }
            catch (OracleException ex)
            {
                if (ex.Message.ToUpper().Contains("ORA-02289"))
                {
                    entidades.CriarSequence(nomeSequenciaTenant);
                    return entidades.GerarSequencia(nomeSequenciaTenant);
                }
                throw;
            }
        }
        #endregion

        #region OnModelCreating
        public static void OnModelCreating(DbContext entidades)
        {
            if (entidades.Database.IsPostgreSql())
            {
                var configPostgres = Devart.Data.PostgreSql.Entity.Configuration.PgSqlEntityProviderConfig.Instance;
                configPostgres.Workarounds.DisableQuoting = true;
            }
            else if (entidades.Database.IsOracle())
            {
                var configOracle = Devart.Data.Oracle.Entity.Configuration.OracleEntityProviderConfig.Instance;
                configOracle.Workarounds.IgnoreSchemaName = true;
                configOracle.CodeFirstOptions.UseNonUnicodeStrings = true;
                configOracle.Workarounds.ColumnTypeCasingConventionCompatibility = true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
