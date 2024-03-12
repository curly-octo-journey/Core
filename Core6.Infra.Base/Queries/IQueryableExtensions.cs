using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core6.Infra.Base.Queries
{
    public static class TypeExt
    {
        /// <summary>
        /// Search for a method by name and parameter types.  Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, params Type[] parameterTypes)
        {
            return thisType.GetMethodExt(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, parameterTypes);
        }

        /// <summary>
        /// Search for a method by name, parameter types, and binding flags.  Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.IsInterface)
            {
                foreach (Type interfaceType in thisType.GetInterfaces())
                    GetMethodExt(ref matchingMethod, interfaceType, name, bindingFlags, parameterTypes);
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod, Type type, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
            {
                // Check that the parameter counts and types match, with 'loose' matching on generic parameters
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!parameterInfos[i].ParameterType.IsSimilarType(parameterTypes[i]))
                            break;
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                            matchingMethod = methodInfo;
                        else
                            throw new AmbiguousMatchException("More than one matching method found!");
                    }
                }
            }
        }

        /// <summary>
        /// Special type used to match any generic parameter type in GetMethodExt().
        /// </summary>
        public class T
        { }

        /// <summary>
        /// Determines if the two types are either identical, or are both generic parameters or generic types
        /// with generic parameters in the same locations (generic parameters match any other generic paramter,
        /// but NOT concrete types).
        /// </summary>
        private static bool IsSimilarType(this Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef)
                thisType = thisType.GetElementType();
            if (type.IsByRef)
                type = type.GetElementType();

            // Handle array types
            if (thisType.IsArray && type.IsArray)
                return thisType.GetElementType().IsSimilarType(type.GetElementType());

            // If the types are identical, or they're both generic parameters or the special 'T' type, treat as a match
            if (thisType == type || (thisType.IsGenericParameter || thisType == typeof(T)) && (type.IsGenericParameter || type == typeof(T)))
                return true;

            // Handle any generic arguments
            if (thisType.IsGenericType && type.IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                            return false;
                    }
                    return true;
                }
            }

            return false;
        }

    }

    public static class IQueryableExtensions
    {
        public static int paramCounter = 0;

        /// <summary>
        /// Use this to select only specific fields, instead fetching the whole entity.
        /// </summary>
        /// <example><code>
        /// var query =
        ///     YOUR_QUERYABLE_OBJECT
        ///     .SelectDynamicFields(new List<string>(){
        ///         "Property1",
        ///         "Property2",
        ///     });
        ///     
        /// dynamic FirstObj = query.FirstOrDefault();
        /// Console.WriteLine(FirstObj.Property1); //Name of the member will validated in runtime!
        /// </code></example>
        /// <typeparam name="T">Type of Source IQueryable</typeparam>
        /// <param name="source">Source IQueryable</param>
        /// <param name="propertyNames">List of Property-Names you want to Select</param>
        /// <returns>A dynamic IQueryable Object. The object includes all Property-Names you have given as Fields.</returns>
        public static IQueryable<dynamic> SelectDynamic<T>(this IQueryable<T> source, IEnumerable<string> propertyNames)
        {
            if (source == null) throw new ArgumentNullException("Source Object is NULL");

            //Here we do something similar to
            //
            //  Select(source => new {
            //      property1 = source.property1,
            //      property2 = source.property2,
            //      [...]
            //  })
            //
            //We build here firstly the Expression needed by the Select-Method dynamicly.
            //Beyond this we build even the class dynamicly. The class includes only
            //the Properties we want to project. The difference is, that the class is
            //not an anonymous type. Its a "Type built in Runtime" using Reflection.Emit.

            var selector = getSelector(source.ElementType, propertyNames);

            //Now Select and return the IQueryable object
            return source.Select(selector as Expression<Func<T, dynamic>>);
        }

        private static LambdaExpression getSelector(Type type, IEnumerable<string> propertyNames)
        {
            //Prepare ParameterExpression refering to the source object
            var sourceItem = Expression.Parameter(type, "t" + paramCounter.ToString());
            paramCounter++;

            //Get PropertyInfos from Source Object (Filter all Misspelled Property-Names)
            var sourceProperties = propertyNames.Where(name => findPropertyByPath(type, name) != null).ToDictionary(name => name, name => findPropertyByPath(type, name));

            //Build dynamic a Class that includes the Fields (no inheritance, no interfaces)
            //sourceProperties.Values.ToDictionary(f => f.Name, f => f.PropertyType)
            Dictionary<string, Type> dictionaryOfProps = new Dictionary<string, Type>();
            foreach (var prop in sourceProperties)
            {
                var _type = prop.Value.PropertyType;
                var name = prop.Key;
                //var name = prop.Key.Replace(".", "");
                dictionaryOfProps.Add(name, _type);
            }
            var dynamicType = DynamicTypeBuilder.GetDynamicType(dictionaryOfProps, typeof(object), Type.EmptyTypes);

            //Create the Projection
            var init = getMemberInitialization(dynamicType, type, sourceItem, sourceProperties);

            var funcType = typeof(Func<,>);
            var genericFunc = funcType.MakeGenericType(type, typeof(object));
            var selector = Expression.Lambda(genericFunc, init, sourceItem);

            return selector;
        }

        private static MemberInitExpression getMemberInitialization(Type dynamicType, Type typeSource, ParameterExpression sourceItem, Dictionary<string, PropertyInfo> sourceProperties, string baseName = null)
        {
            //Create the Binding Expressions
            List<MemberAssignment> _bindings = new List<MemberAssignment>();
            foreach (var field in dynamicType.GetFields())
            {
                Expression exprToBind = null;
                //var isList = field.FieldType.GetGenericArguments().Count() > 0;
                var isList = field is IList;

                var fieldName = field.Name;
                if (baseName != null)
                {
                    fieldName = string.Format("{0}.{1}", baseName, field.Name);
                }

                // Encontrar uma maneira mais bonita de saber se o field é dinâmico ou se é apenas uma propriedade comum
                if (field.FieldType.Name.StartsWith("DynamicLinq"))
                {
                    exprToBind = getMemberInitialization(field.FieldType, typeSource, sourceItem, sourceProperties, fieldName);
                }

                // Se for lista é preciso fazer um select new nos filhos também
                else if (isList)
                {
                    var _source = makeMemberExpressionByPath(typeSource, field.Name, sourceItem);
                    var dynamicChildrenType = field.FieldType.GetGenericArguments().FirstOrDefault();
                    var _props = sourceProperties.Where(x => x.Key.StartsWith(field.Name + ".")).Select(x => x.Key.Remove(0, x.Key.IndexOf(".") + 1));
                    var childrenSourceProperty = typeSource.GetProperty(field.Name);
                    var childrensMemberExpr = makeMemberExpressionByPath(typeSource, field.Name, sourceItem);

                    if (childrenSourceProperty != null)
                    {
                        var childrenType = childrenSourceProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                        var selector = getSelector(childrenType, _props);

                        var funcType = typeof(Func<,>);
                        var genericFunc = funcType.MakeGenericType(childrenType, typeof(object));

                        var selectMethod = typeof(Enumerable).GetMethods()
                               .Where(m => m.Name == "Select" && m.GetParameters().Length == 2)
                               .FirstOrDefault();

                        var exprSelect = Expression.Call(childrensMemberExpr, selectMethod, selector);
                    }

                }

                // Se for um cara normal faz a pena a expressão (x.Campo)
                else
                {
                    exprToBind = makeMemberExpressionByPath(typeSource, findPathByName(sourceProperties, fieldName), sourceItem);
                }

                if (exprToBind != null)
                    _bindings.Add(Expression.Bind(field, exprToBind));
            }

            var init = Expression.MemberInit(Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), _bindings);

            return init;
        }

        /// <summary>
        /// <para>
        /// Selecting and including related entities, instead of using the "Include"-Method from EF.
        /// With the "Include"-Method it is not possible to specify conditions what entities to include.
        /// With this method you can specify exactly what entities to load, and how they will be filtered
        /// or ordered.
        /// </para>
        /// <para>
        /// Attention! This selection will use AsEnumerable(). This means, that the database will be
        /// queried at this point of the chain!
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of Source IQueryable</typeparam>
        /// <param name="source">Source IQueryable</param>
        /// <param name="includeExpessions">Lamda Expressions, defining what entities to include</param>
        /// <returns></returns>
        public static IEnumerable<T> SelectIncluding<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, object>>> includeExpessions)
        {
            if (source == null) throw new ArgumentNullException("Source Object is NULL");


            //Here we do something similar to this:

            //First, select into a anonymous type with all the related entity-collections you need.
            //The relation can be defined by every query you want.

            //var query = _dbSet
            //    .Select(mainEntity => new
            //    {
            //
            //        //The main object. We need this field to unwrap later.
            //        mainEntity,
            //
            //        //Example how to retrieve only the newest history entry
            //        newestHistoryEntry = mainEntity.HistoryEntries.OrderByDescending(x => x.Timestamp).Take(1),
            //
            //        //Example how to order related entities
            //        itemSpecMSRPrices = mainEntity.OtherEntities.OrderBy(y => y.Something).ThenBy(y => y.SomeOtherThing),
            //
            //        //Example how to retrieve entities one level deeper
            //        secondLevel = mainEntity.CollectionWithRelations.Select(x => x.EntityCollectionOnSecondLevel),
            //
            //        //Of course you can order or subquery the deeper level
            //        //Here you should use SelectMany, to flatten the query
            //        secondLevelOrdered = mainEntity.CollectionWithRelations.SelectMany(x => x.EntityCollectionOnSecondLevel.OrderBy(y => y.Something).ThenBy(y => y.SomeOtherThing)),
            //
            //    });

            //Now we fire up the query (AsEnumerable) and then unwrap the SupplierItem out 
            //of the anonymous type (Select). 

            //return query.AsEnumerable().Select(mainEntity => mainEntity.mainEntity);

            //Because the ObjectContext have collected all the related entities, it have also linked each other correctly over 
            //navigation-properties and reference-properties. Please read this Tip, too:
            //http://blogs.msdn.com/b/alexj/archive/2009/10/13/tip-37-how-to-do-a-conditional-include.aspx

            //We build here firstly the Expression needed by the Select-Method dynamicly.
            //Beyond this we build even the class dynamicly. The class includes only
            //the Properties we want to project. The difference is, that the class is
            //not an anonymous type. Its a "Type built in Runtime" using Reflection.Emit.

            //Remark to the fields within the dynamic generated class:

            //All of them will be declared with the type really needed. So even the dynamicly
            //generated class will be "strong-type-safe".

            //Remark to the paramter "includeExpressions":

            //The method expect a collection of LambdaExpression-Objects. In fact we do not need
            //the LamdaExpression, but only the body of them. The LambdaExpression is only a
            //pleasant way to enable the user to define expression in a strong-type way.

            //Prepare ParameterExpression refering to the source object
            var sourceItem = Expression.Parameter(source.ElementType, "t");

            //Prepare helper class to replace the user-parameter of the LambdExpression with ours
            var paramRewriter = new PredicateRewriterVisitor(sourceItem);

            //Loop all expression and:
            //  1.) Determine returned type.
            //  2.) Get Body and replace the Parameter used by the user with ours
            //  2.) Give all of them a name and save them in a Dictionary
            Dictionary<string, Tuple<Expression, Type>> dynamicFields = new Dictionary<string, Tuple<Expression, Type>>();
            int dynamicFieldsCounter = 0;
            foreach (Expression<Func<T, object>> includeExpession in includeExpessions)
            {
                //Detect Type
                Type typeDetected;
                if (includeExpession.Body.NodeType == ExpressionType.Convert ||
                    includeExpession.Body.NodeType == ExpressionType.ConvertChecked)
                {
                    var unary = includeExpession.Body as UnaryExpression;
                    if (unary != null)
                        typeDetected = unary.Operand.Type;
                }
                typeDetected = includeExpession.Body.Type;
                //Save into List
                dynamicFields.Add("f" + dynamicFieldsCounter, new Tuple<Expression, Type>(
                    paramRewriter.ReplaceParameter(includeExpession.Body, includeExpession.Parameters[0]),
                    typeDetected
                    ));
                //Count
                dynamicFieldsCounter++;
            }

            //Add a field in which the source object will be saved
            dynamicFields.Add("sourceObject", new Tuple<Expression, Type>(
                sourceItem,
                source.ElementType
                ));

            //Build dynamic a Class that includes the Fields (no inheritance, no interfaces)
            var dynamicType = DynamicTypeBuilder.GetDynamicType(dynamicFields.ToDictionary(x => x.Key, x => x.Value.Item2), typeof(object), Type.EmptyTypes);

            //Create the Binding Expressions
            var bindings = dynamicType.GetFields().Select(p => Expression.Bind(p, dynamicFields[p.Name].Item1)).OfType<MemberBinding>().ToList();

            //Create the Projection
            var selector = Expression.Lambda<Func<T, dynamic>>(Expression.MemberInit(Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);

            return source.Select(selector).AsEnumerable().Select(x => (T)x.sourceObject);
        }

        private static MemberExpression makeMemberExpressionByPath(Type baseType, string path, ParameterExpression param)
        {
            PropertyInfo prop = null;
            Type propType = baseType;
            MemberExpression propOriginal = null;

            string[] parts = path.Split('.').Select(x => x.Trim()).ToArray();

            foreach (var part in parts)
            {
                prop = propType.GetProperty(part);

                var xx = prop is IList;

                if (prop == null)
                    break;

                // Monta o membro da propriedade "x.Propridade"
                if (propOriginal == null)
                {
                    propOriginal = Expression.Property(param, prop);
                }

                // Monta o membro da propriedade quando for associação "x.Associacao.....Propriedade"
                else
                {
                    propOriginal = Expression.Property(propOriginal, prop);
                }

                propType = prop.PropertyType;
            }

            return propOriginal;
        }

        private static PropertyInfo findPropertyByPath(Type baseType, string path)
        {
            PropertyInfo prop = null;
            Type propType = baseType;

            string[] parts = path.Split('.').Select(x => x.Trim()).ToArray();

            foreach (var part in parts)
            {
                prop = propType.GetProperty(part);

                if (prop == null)
                    break;

                propType = prop.PropertyType;

                if (propType.GetGenericArguments().Count() > 0)
                {
                    var type = propType.GetGenericArguments()[0];
                    propType = type;
                }
            }

            return prop;
        }

        private static string findPathByName(Dictionary<string, PropertyInfo> dictionaryOfProps, string name)
        {
            var path = dictionaryOfProps.Where(x => x.Key.Replace(".", "") == name).FirstOrDefault().Key;
            if (path != null)
                return path;
            else
                return name;
        }

        public static IQueryable<dynamic> SelectDynamic<T>(this IQueryable<T> source, string propertyNames)
        {
            string[] properties = propertyNames.Split(',').Select(x => x.Trim()).ToArray();
            return source.SelectDynamic(properties);
        }


        /// <summary>
        /// Helper class
        /// </summary>
        private class PredicateRewriterVisitor : ExpressionVisitor
        {
            private ParameterExpression _parameterExpression;
            private ParameterExpression _parameterExpressionToReplace;
            public PredicateRewriterVisitor(ParameterExpression parameterExpression)
            {
                _parameterExpression = parameterExpression;
            }
            public Expression ReplaceParameter(Expression node, ParameterExpression parameterExpressionToReplace)
            {
                _parameterExpressionToReplace = parameterExpressionToReplace;
                return base.Visit(node);
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _parameterExpressionToReplace) return _parameterExpression;
                else return node;
            }
        }

    }
}