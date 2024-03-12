using Devart.Data.Oracle.Entity;
using Devart.Data.PostgreSql.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Core6.Infra.Base;

namespace Core6.Infra.Base.Queries
{
    public class WhereQuery
    {
        #region MontaWhere
        public IQueryable<T> MontaWhere<T>(IQueryable<T> query, List<GrupoFiltroQuery> grupoFiltros)
        {
            if (grupoFiltros == null)
                return query;
            try
            {
                Type tipoEntidade = typeof(T);
                ParameterExpression param = Expression.Parameter(tipoEntidade, "x");
                Expression exprPower = null;
                foreach (var item in grupoFiltros)
                {
                    Expression expr = null;

                    foreach (var filtro in item.Filtros)
                    {
                        MemberExpression member = null;
                        PropertyInfo property = tipoEntidade.GetProperty(filtro.property);

                        if (property == null)
                        {
                            string[] partes = filtro.property.Split('.');
                            var tipoEntidadeParte = tipoEntidade;

                            foreach (var parte in partes)
                            {
                                property = tipoEntidadeParte.GetProperty(parte);

                                if (property == null)
                                    continue;

                                tipoEntidadeParte = property.PropertyType;

                                PropertyInfo propInfoChild = property;
                                if (member == null)
                                {
                                    member = Expression.MakeMemberAccess(param, propInfoChild);
                                }
                                else
                                {
                                    member = Expression.MakeMemberAccess(member, propInfoChild);
                                }

                                // Caso for uma lista, o membro é a própria lista
                                if (property.PropertyType.IsGenericType)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            member = Expression.MakeMemberAccess(param, property);
                        }

                        if (property == null)
                            throw new Exception("Propriedade '" + filtro.property + "' não encontrada.");

                        if (filtro.value.ToString() == "")
                            continue;

                        Expression condicao = MontaCondicao(filtro, property, param, member, query);
                        if (condicao != null)
                        {
                            // Negando a condição
                            if (filtro.Not)
                                condicao = Expression.Not(condicao);

                            // AND ou OR
                            if (expr == null)
                                expr = condicao;
                            else
                            {
                                if (filtro.And)
                                    expr = Expression.AndAlso(expr, condicao);
                                else
                                    expr = Expression.Or(expr, condicao);
                            }
                        }
                        /*    if (expr != null)
                            {
                                lambda = Expression.Lambda<Func<T, bool>>(expr, param);
                            }*/
                    }

                    if (expr != null)
                    {
                        if (exprPower == null)
                            exprPower = expr;
                        else
                        {
                            if (item.And)
                                exprPower = Expression.AndAlso(exprPower, expr);
                            else
                                exprPower = Expression.Or(exprPower, expr);
                        }
                    }
                }

                if (exprPower != null)
                {
                    Expression<Func<T, bool>> lambda = null;
                    lambda = Expression.Lambda<Func<T, bool>>(exprPower, param);
                    query = query.Where(lambda);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao fitrar '" + e.Message + "' ");
            }
            return query;
        }
        #endregion

        #region _converteItem
        private object ConverteItem(MemberExpression exprEsquerda, FiltroClass filtro, PropertyInfo property)
        {
            try
            {
                if (filtro.value is IList)
                {
                    IList valoresSelecionados = (IList)filtro.value;
                    List<int> intVals = new List<int>();
                    List<string> stringVals = new List<string>();
                    List<Guid> guidVals = new List<Guid>();
                    foreach (JValue i in valoresSelecionados)
                    {
                        if (i.Type == JTokenType.Boolean)
                        {
                            intVals.Add((bool)i == true ? 1 : 0);
                        }
                        else if (i.Type == JTokenType.String)
                        {
                            Guid guid;
                            if (Guid.TryParse(i.ToString(), out guid))
                            {
                                guidVals.Add(guid);
                            }
                            else
                            {
                                string enumString = i.ToString();
                                stringVals.Add(enumString);
                            }
                        }
                        else
                        {
                            var enumInt = Convert.ToInt32(i.ToString());
                            intVals.Add(enumInt);
                        }
                    }

                    if (stringVals.Count > 0)
                    {
                        return stringVals;
                    }
                    else if (guidVals.Count > 0)
                    {
                        return guidVals;
                    }
                    return intVals;
                }

                // String
                else if (property.PropertyType == typeof(string))
                {
                    return filtro.value.ToString();
                }

                //DataTime
                else if (property.PropertyType == typeof(DateTime))
                {
                    var data = Convert.ToDateTime(filtro.value.ToString());
                    if (filtro.GetOperador() == OperadorFiltros.MenorOuIgual)
                    {
                        if (data.Hour == 0 && data.Minute == 0 && data.Second == 0)
                        {
                            data = data.AddDays(1).AddSeconds(-1);
                        }
                    }
                    return data;
                }
                else if (property.PropertyType == typeof(DateTime?))
                {
                    var data = Convert.ToDateTime(filtro.value.ToString());
                    if (data.Year <= 1900 || data.Year >= 2900)
                    {
                        return null;
                    }
                    if (filtro.GetOperador() == OperadorFiltros.MenorOuIgual)
                    {
                        if (data.Hour == 0 && data.Minute == 0 && data.Second == 0)
                        {
                            data = data.AddDays(1).AddSeconds(-1);
                        }
                    }
                    return data;
                }
                //Int64
                else if (property.PropertyType == typeof(long))
                {
                    return Convert.ToInt64(filtro.value.ToString());
                }
                //Int64?
                else if (property.PropertyType == typeof(long?))
                {
                    return Convert.ToInt64(filtro.value.ToString());
                }
                //Int
                else if (property.PropertyType == typeof(int))
                {
                    return Convert.ToInt32(filtro.value.ToString());
                }
                else if (property.PropertyType == typeof(int?))
                {
                    return Convert.ToInt32(filtro.value.ToString());
                }

                //boolean
                else if (property.PropertyType == typeof(bool))
                {
                    if (filtro.value == null)
                    {
                        return null;
                    }

                    if (filtro.value.ToString() == "1")
                    {
                        return true;
                    }

                    if (filtro.value.ToString().ToLower() == "true")
                    {
                        return true;
                    }

                    return false;
                }
                else if (property.PropertyType == typeof(bool?))
                {
                    if (filtro.value == null)
                    {
                        return null;
                    }
                    if (filtro.value.ToString() == "1")
                    {
                        return true;
                    }

                    if (filtro.value.ToString().ToLower() == "true")
                    {
                        return true;
                    }

                    return false;
                }

                //decimal
                else if (property.PropertyType == typeof(decimal))
                {
                    return Convert.ToDecimal(filtro.value.ToString());
                }
                else if (property.PropertyType == typeof(decimal?))
                {
                    return Convert.ToDecimal(filtro.value.ToString());
                }

                //Guid
                else if (property.PropertyType == typeof(Guid))
                {
                    return new Guid(filtro.value.ToString());
                }

                else if (property.PropertyType == typeof(Guid?))
                {
                    return new Guid(filtro.value.ToString());
                }

                // Enum
                else if (exprEsquerda.Type.BaseType == typeof(Enum))
                {
                    List<int> intVals = new List<int>();
                    try
                    {
                        IList valoresSelecionados = (IList)filtro.value;
                        foreach (var i in valoresSelecionados)
                        {
                            var enumInt = Convert.ToInt32(i.ToString());
                            intVals.Add(enumInt);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Convert.ToInt32(filtro.value);
                    }
                    return intVals;
                }

                return filtro.value;
            }
            catch (Exception)
            {
                return null;
                //throw new Exception(string.Format("Não foi possível converter o filtro '{0}' para '{1}'. Erro:{2}", filtro.value.ToString(), property.PropertyType, e.Message));
            }
        }
        #endregion

        #region _getExprDireita
        private Expression GetExprDireita(FiltroClass filtro, PropertyInfo property, object value)
        {
            try
            {
                if (property.PropertyType.BaseType == typeof(Enum))
                    return Expression.Constant(value);
                else
                    if (filtro.value is IList)
                    return Expression.Constant(value);
                else
                    return Expression.Constant(value, property.PropertyType);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Não foi possível montar o 'Expression.Constant' do filtro '{0} ({1})' para '{2}'. Erro:{3}", filtro.property, value, property.PropertyType, e.Message));
            }
        }
        #endregion

        #region MontaExpressaoDaCondicao
        private Expression MontaExpressaoDaCondicao(FiltroClass filtro, Expression exprDireita, Expression exprEsquerda, PropertyInfo property, object value, IQueryable query)
        {
            Expression condicao = null;
            switch (filtro.GetOperador())
            {
                case OperadorFiltros.Contem:
                    {
                        // A principio apenas o contem terá a condição seguida por LOWER, caso tivermos que colocar nos outros métodos,
                        // é interessante conversarmos e analisarmos por questões de performance
                        if (property.PropertyType == typeof(string))
                        {
                            exprDireita = Expression.Constant(value.ToString().ToLower(), property.PropertyType);
                            var exprLower = Expression.Call(exprEsquerda, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                            condicao = Expression.Call(exprLower, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), exprDireita);
                        }
                        break;
                    }
                case OperadorFiltros.ContemTodos:
                    {
                        // A principio apenas o contem terá a condição seguida por LOWER, caso tivermos que colocar nos outros métodos,
                        // é interessante conversarmos e analisarmos por questões de performance
                        if (property.PropertyType == typeof(string))
                        {
                            var partesBusca = value.ToString().Split(' ').Where(x => !string.IsNullOrEmpty(x));

                            Expression condicaoFinal = null;
                            foreach (var busca in partesBusca)
                            {
                                MontaExpressaoContains(out exprDireita, exprEsquerda, property, busca, out condicao);
                                if (condicaoFinal == null)
                                {
                                    condicaoFinal = condicao;
                                }
                                else
                                {
                                    condicaoFinal = Expression.And(condicaoFinal, condicao);
                                }
                            }
                            condicao = condicaoFinal;
                        }
                        break;
                    }
                case OperadorFiltros.In:
                    {
                        if (exprEsquerda.Type.BaseType == typeof(Enum))
                        {
                            var exprConvertida = Expression.Convert(exprEsquerda, typeof(int));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        else if (exprEsquerda.Type == typeof(int))
                        {
                            var exprConvertida = Expression.Convert(exprEsquerda, typeof(int));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        else if (exprEsquerda.Type == typeof(string))
                        {
                            var exprConvertida = Expression.Convert(exprEsquerda, typeof(string));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        else if (exprEsquerda.Type == typeof(Guid))
                        {
                            var exprConvertida = Expression.Convert(exprEsquerda, typeof(Guid));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        else if (filtro.value is IList)
                        {
                            var exprConvertida = Expression.Convert(exprEsquerda, typeof(int));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        break;
                    }
                case OperadorFiltros.InOuNull:
                    {
                        UnaryExpression exprConvertida;
                        if (exprEsquerda.Type.BaseType == typeof(Enum))
                        {
                            exprConvertida = Expression.Convert(exprEsquerda, typeof(int));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        if (filtro.value is IList)
                        {
                            exprConvertida = Expression.Convert(exprEsquerda, typeof(int));
                            condicao = Expression.Call(exprDireita, value.GetType().GetMethod("Contains"), exprConvertida);
                        }
                        try
                        {
                            var expressaoNull = exprEsquerda;
                            while (!IsNullable(expressaoNull.Type))
                            {
                                expressaoNull = expressaoNull.GetType().GetProperty("Expression").GetValue(expressaoNull) as Expression;
                            }
                            if (expressaoNull != null && IsNullable(expressaoNull.Type))
                            {
                                var condicaoNull = Expression.Equal(expressaoNull, Expression.Constant(null));
                                condicao = Expression.Or(condicao, condicaoNull);
                            }
                        }
                        catch
                        {
                        }
                        break;
                    }
                case OperadorFiltros.Maior:
                    condicao = Expression.GreaterThan(exprEsquerda, Expression.Convert(exprDireita, exprEsquerda.Type));
                    break;
                case OperadorFiltros.Menor:
                    condicao = Expression.LessThan(exprEsquerda, Expression.Convert(exprDireita, exprEsquerda.Type));
                    break;
                case OperadorFiltros.Igual:
                    condicao = Expression.Equal(exprEsquerda, Expression.Convert(exprDireita, exprEsquerda.Type));
                    break;
                case OperadorFiltros.MaiorOuIgual:
                    condicao = Expression.GreaterThanOrEqual(exprEsquerda, Expression.Convert(exprDireita, exprEsquerda.Type));
                    break;
                case OperadorFiltros.MenorOuIgual:
                    condicao = Expression.LessThanOrEqual(exprEsquerda, Expression.Convert(exprDireita, exprEsquerda.Type));
                    break;

                case OperadorFiltros.IniciaCom:
                    {
                        // Esse tipo de filtro foi construido para suportar apenas string e integer.
                        if (property.PropertyType == typeof(string))
                        {
                            exprDireita = Expression.Constant(value.ToString().ToLower(), property.PropertyType);
                            var exprLower = Expression.Call(exprEsquerda, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                            //                        condicao = Expression.Call(exprLower, typeof(string).GetMethod("StartsWith"), exprDireita);
                            condicao = Expression.Call(exprLower, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), exprDireita);
                        }

                        if (property.PropertyType == typeof(int))
                        {
                            exprDireita = Expression.Constant(value.ToString().ToLower());
                            var exprConvertidaDecimal = Expression.Convert(exprEsquerda, typeof(decimal?));

                            switch (RecuperarTipoBancoDeDados(query))
                            {
                                case EnumBancoDeDados.Oracle:
                                    MethodInfo metodoToCharOracle = typeof(OracleFunctions).GetMethod("ToChar", new Type[] { typeof(decimal?) });
                                    var expToCharOracle = Expression.Call(metodoToCharOracle, exprConvertidaDecimal);
                                    condicao = Expression.Call(expToCharOracle, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), exprDireita);
                                    break;
                                case EnumBancoDeDados.Postgres:
                                    MethodInfo metodoToCharPostGre = typeof(PgSqlFunctions).GetMethod("ToChar", new Type[] { typeof(decimal?), typeof(string) });
                                    var expToCharPostGre = Expression.Call(metodoToCharPostGre, exprConvertidaDecimal, Expression.Constant("FM999999999"));
                                    condicao = Expression.Call(expToCharPostGre, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), exprDireita);
                                    break;
                                default:
                                    break;
                            }

                        }
                        break;
                    }

                case OperadorFiltros.TerminaCom:
                    {
                        // Esse tipo de filtro foi construido para suportar apenas string e integer.
                        if (property.PropertyType == typeof(string))
                        {
                            exprDireita = Expression.Constant(value.ToString().ToLower(), property.PropertyType);
                            var exprLower = Expression.Call(exprEsquerda, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                            //                        condicao = Expression.Call(exprLower, typeof(string).GetMethod("StartsWith"), exprDireita);
                            condicao = Expression.Call(exprLower, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), exprDireita);

                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            exprDireita = Expression.Constant(value.ToString().ToLower());
                            var exprConvertidaDecimal = Expression.Convert(exprEsquerda, typeof(decimal?));

                            switch (RecuperarTipoBancoDeDados(query))
                            {
                                case EnumBancoDeDados.Oracle:
                                    var metodoToCharOracle = typeof(OracleFunctions).GetMethod("ToChar", new Type[] { typeof(decimal?) });
                                    var expToCharOracle = Expression.Call(metodoToCharOracle, exprConvertidaDecimal);
                                    condicao = Expression.Call(expToCharOracle, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), exprDireita);
                                    break;
                                case EnumBancoDeDados.Postgres:
                                    MethodInfo metodoToCharPostGre = typeof(PgSqlFunctions).GetMethod("ToChar", new Type[] { typeof(decimal?), typeof(string) });
                                    var expToCharPostGre = Expression.Call(metodoToCharPostGre, exprConvertidaDecimal, Expression.Constant("FM999999999"));
                                    condicao = Expression.Call(expToCharPostGre, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), exprDireita);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    }

                default:
                    throw new Exception("Não foi implementado operador para " + filtro.Operador);
            }
            return condicao;
        }
        #endregion

        #region MontaExpressaoContains
        private void MontaExpressaoContains(out Expression exprDireita, Expression exprEsquerda, PropertyInfo property, object value, out Expression condicao)
        {
            exprDireita = Expression.Constant(value.ToString().ToLower(), property.PropertyType);
            var exprLower = Expression.Call(exprEsquerda, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            condicao = Expression.Call(exprLower, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), exprDireita);
        }
        #endregion

        #region MontaCondicao
        private Expression MontaCondicao(FiltroClass filtro, PropertyInfo property, ParameterExpression param, MemberExpression member, IQueryable query)
        {
            Expression condicao = null;
            MemberExpression exprEsquerda = member;
            object value = null;

            // Guarda a propriedade original
            PropertyInfo propertyOriginal = property;

            // Caso for tipo genérico (listas etc), deve-se procurar via reflection a propriedade que sera filtrada
            // Ex.: Caso o client mande ItemAlmox.CodigoAlmox, nesse momento "property" é ItemAmox, porém, ela deve ser "CodigoAlmox"

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                Type tipoDaLista = property.PropertyType.GetGenericArguments()[0];
                property = tipoDaLista.GetProperty(filtro.NomeDaPropriedade);
                if (property == null)
                    return null;

                // Montamos a parte esquerda do que será filtrado
                ParameterExpression subParam = Expression.Parameter(tipoDaLista, "p");
                Expression subExprEsquerda = Expression.MakeMemberAccess(subParam, property);
                value = ConverteItem(exprEsquerda, filtro, property);
                Expression subExprDireita = GetExprDireita(filtro, property, value);
                Expression subExpression = MontaExpressaoDaCondicao(filtro, subExprDireita, subExprEsquerda, property, value, query);

                // O método Any é generico (Any<T>), logo, é preciso pegar o mesmo e depois "criar" o fortemente tipado
                MethodInfo metodoAnyGeneric = typeof(Enumerable)
                                            .GetMethods(BindingFlags.Static | BindingFlags.Public)
                                            .Where(mi => mi.Name == "Any").LastOrDefault();

                // Cria o método Any tipado Any<T>. Onde T é o tipo da lista
                MethodInfo metodoAny = metodoAnyGeneric.MakeGenericMethod(tipoDaLista);

                // Montamos a sub expressão que irá dentro do método Any
                // Ela precisa ser uma expressão lambda (p => p.....)
                var subExprLambda = Expression.Lambda(subExpression, subParam);

                // Montamos a expression principal que utilizará o método "Any" que pertence a coleções
                condicao = Expression.Call(metodoAny, exprEsquerda, subExprLambda);
            }
            else
            {
                // Situação normal
                value = ConverteItem(exprEsquerda, filtro, property);
                if (value != null)
                {
                    Expression exprDireita = GetExprDireita(filtro, property, value);
                    condicao = MontaExpressaoDaCondicao(filtro, exprDireita, exprEsquerda, property, value, query);
                }
            }
            return condicao;
        }
        #endregion

        #region IsNullable
        private bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
        #endregion

        // TODO: Retirar EntityQueryProvider
        #region RecuperarTipoBancoDeDados
        private EnumBancoDeDados RecuperarTipoBancoDeDados(IQueryable query)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
            var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);

            var dependencies = typeof(RelationalQueryContextFactory).GetField("_dependencies", bindingFlags).GetValue(queryContextFactory);
            var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
            var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
            var stateManager = (IStateManager)stateManagerProperty;

            return stateManager.Context.Database.IsPostgreSql() ? EnumBancoDeDados.Postgres : stateManager.Context.Database.IsOracle() ? EnumBancoDeDados.Oracle : throw new Exception("Banco de dados não suportado");
        }
        #endregion
    }
}
