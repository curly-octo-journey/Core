//using System.Data.Entity;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data;
using System.Reflection;

namespace Core6.Repositorio.Base.Infra
{
    //TODO
    public static class DbContextHelper
    {
        //    #region ConfigurarStringParaNaoUnicode
        //    public static void ConfigurarStringParaNaoUnicode(this DbModelBuilder modelBuilder)
        //    {
        //        modelBuilder
        //            .Properties()
        //            .Where(p => p.PropertyType == typeof(string))
        //            .Configure(p => p.IsUnicode(false));
        //    }
        //    #endregion

        #region ConfigurarCasasDecimais
        public static void ConfigurarCasasDecimais(ModelBuilder modelBuilder)
        {
            //Quando faz select em coluna decimal, que no banco está 15.666, o select retorna 15.6660
            //Esse código resolve isso
            var converter = new ValueConverter<decimal, double>(
                v => (double)v,
                v => (decimal)v
            );

            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetValueConverter(converter);
            }
        }
        #endregion

        public static List<T> SqlQuery<T>(this DbContext db, string query) where T : class, new()
        {
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
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
    }
}



