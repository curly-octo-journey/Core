using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Core6.Infra.Base.Queries
{
    public class OrderByQuery
    {
        public IQueryable<T> MontaOrderBy<T>(IQueryable<T> query, List<OrdemClass> ordem)
        {
            if (ordem == null)
                return query;

            foreach (var item in ordem)
            {
                // Verifica qual o método que deve ser chamado para ordenação. 
                var direcao = item.ASC() ? "By" : "ByDescending";
                var metodo = query.Expression.Type != typeof(IOrderedQueryable<T>) ? "Order" : "Then";

                bool possuiFilterValue;
                if (item.filterValue is string)
                {
                    possuiFilterValue = !string.IsNullOrEmpty((string)item.filterValue);
                }
                else
                {
                    possuiFilterValue = item.filterValue != null;
                }

                // Aplica a ordenação na query via reflection
                var generatedQuery = possuiFilterValue ?
                    ApplyOrder(query, item.property, item.filterValue, string.Format("{0}{1}", metodo, direcao)) :
                    ApplyOrder(query, item.property, string.Format("{0}{1}", metodo, direcao));

                if (generatedQuery != null)
                    query = generatedQuery;
            };

            return query;
        }

        private IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);

            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            foreach (var prop in props)
            {
                var pi = type.GetProperty(prop);

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        private IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, object filterValue, string methodName)
        {
            // Divide a string de propriedade por associações
            var props = property.Split('.');
            var type = typeof(T);

            // Cria o parametro da expressão > {x => x}
            var paramExpression = Expression.Parameter(type, "x");
            Expression expression = paramExpression;

            // Passa em cada associação montando a expressão, até chegar na última propriedade > {x => x.assoc.prop}
            foreach (var prop in props)
            {
                // Busca na entidade a prop. referente a string recebida
                var propInfo = type.GetProperty(prop);
                expression = Expression.Property(expression, propInfo);

                type = propInfo.PropertyType;
            }

            // Converte o valor do filtro para o tipo esperado da propriedade, e retorna caso não seja possível realizar a conversão
            filterValue = ConverterValorFiltro(filterValue, type);

            if (filterValue == null)
            {
                return ApplyOrder(source, property, methodName);
            }

            // Se a prop. for do tipo string, converte para lowercase, da mesma forma que o filtro faz
            if (type == typeof(string))
                expression = Expression.Call(expression, "ToLower", null);

            // Adiciona a comparação do valor da propriedade com o valor do filtro > {x.prop == filterValue}
            Expression compareExpression = Expression.Equal(expression, Expression.Constant(filterValue));

            // Por fim, transforma o resultado da condição em um int para funcionar na ordenação > {x => x.prop == filterValue ? 0 : 1}
            expression = Expression.Condition(compareExpression, Expression.Constant(0), Expression.Constant(1), typeof(int));

            // A partir daqui, a expressão lambda é criada, e o método do sort é chamado, passando os parâmetros necessários.
            LambdaExpression lambda = Expression.Lambda<Func<T, int>>(expression, paramExpression);

            var sortMethod = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2);

            var result = sortMethod.MakeGenericMethod(typeof(T), typeof(int))
                    .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        private static object ConverterValorFiltro(object filterValue, Type propertyType)
        {
            try
            {
                if (propertyType == typeof(long) || propertyType == typeof(long?))
                    return Convert.ToInt64(filterValue.ToString());

                if (propertyType == typeof(int) || propertyType == typeof(int?))
                    return Convert.ToInt32(filterValue.ToString());

                if (propertyType == typeof(string))
                    return filterValue.ToString().ToLower();

                return filterValue;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}