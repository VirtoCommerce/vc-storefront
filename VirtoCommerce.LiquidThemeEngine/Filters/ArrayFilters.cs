using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public static partial class ArrayFilters
    {
        public static object Tree(object input, string propName, string titlePropName, string delimiter, string sortByPropName)
        {
            var tree = new List<TreeNode>();

            var enumerable = input as IEnumerable;
            if (enumerable != null)
            {
                var elementType = enumerable.GetType().GetEnumerableType();
                var propInfo = elementType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var titlePropInfo = elementType.GetProperty(titlePropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var sortByPropInfo = elementType.GetProperty(sortByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var charDelimiter = delimiter.ToCharArray();

                    foreach (var element in enumerable)
                    {
                        var path = propInfo.GetValue(element) as string;
                        var parts = path.Split(charDelimiter);
                        for (var i = parts.Length; i > 0; i--)
                        {
                            path = string.Join(delimiter, parts.Take(i));
                            var treeNode = tree.FirstOrDefault(n => n.Path == path);
                            if (treeNode == null)
                            {
                                tree.Add(new TreeNode
                                {
                                    Level = i,
                                    ParentPath = string.Join(delimiter, parts.Take(Math.Max(0, i - 1))),
                                    Path = path,
                                    Priority = sortByPropInfo != null ? sortByPropInfo.GetValue(element) as int? : null,
                                    Title = titlePropInfo != null ? titlePropInfo.GetValue(element) as string : null
                                });
                            }
                        }
                    }

                    foreach (var treeNode in tree.OrderBy(n => n.Priority))
                    {
                        if (!string.IsNullOrEmpty(treeNode.ParentPath))
                        {
                            var parent = tree.FirstOrDefault(n => n.Path == treeNode.ParentPath);
                            if (parent != null)
                            {
                                treeNode.Parent = parent;
                                parent.Children.Add(treeNode);
                            }
                        }

                        treeNode.AllChildren = tree.Where(n => n.Path.StartsWith(treeNode.Path + delimiter)).ToList();
                    }
                }
            }

            return tree;
        }

        /// <summary>
        /// Filter the elements of an array by a given condition
        /// {% assign sorted = pages | where:"propName","==","value" %}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static object Where(object input, string propName, string op, string value)
        {
            var retVal = input;
            var enumerable = retVal as IEnumerable;
            if (enumerable != null)
            {
                var queryable = enumerable.AsQueryable();
                var elementType = enumerable.GetType().GetEnumerableType();

                var paramX = Expression.Parameter(elementType, "x");
                var propInfo = elementType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var left = Expression.Property(paramX, propInfo);
                var objValue = ParseString(value);
                var right = Expression.Constant(objValue);
                BinaryExpression binaryOp;

                if (op.EqualsInvariant("=="))
                    binaryOp = Expression.Equal(left, right);
                else if (op.EqualsInvariant("!="))
                    binaryOp = Expression.NotEqual(left, right);
                else if (op.EqualsInvariant(">"))
                    binaryOp = Expression.GreaterThan(left, right);
                else if (op.EqualsInvariant(">="))
                    binaryOp = Expression.GreaterThanOrEqual(left, right);
                else if (op.EqualsInvariant("=<"))
                    binaryOp = Expression.LessThan(left, right);
                else if (op.EqualsInvariant("contains"))
                {
                    Expression expr = null;
                    if (propInfo.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethods().Where(x => x.Name == "Contains").First();
                        expr = Expression.Call(left, containsMethod, right);
                    }
                    else
                    {
                        var containsMethod = typeof(Enumerable).GetMethods().Where(x => x.Name == "Contains" && x.GetParameters().Count() == 2).First().MakeGenericMethod(new Type[] { objValue.GetType() });
                        expr = Expression.Call(containsMethod, left, right);
                    }

                    //where(x=> x.Tags.Contains(y))
                    binaryOp = Expression.Equal(expr, Expression.Constant(true));
                }
                else
                    binaryOp = Expression.LessThanOrEqual(left, right);

                var delegateType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));

                //Construct Func<T, bool> = (x) => x.propName == value expression
                var lambda = Expression.Lambda(delegateType, binaryOp, paramX);

                //Find Queryable.Where(Expression<Func<TSource, bool>>) method
                var whereMethod = typeof(Queryable).GetMethods()
                 .Where(x => x.Name == "Where")
                 .Select(x => new { M = x, P = x.GetParameters() })
                 .Where(x => x.P.Length == 2
                             && x.P[0].ParameterType.IsGenericType
                             && x.P[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                             && x.P[1].ParameterType.IsGenericType
                             && x.P[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                 .Select(x => new { x.M, A = x.P[1].ParameterType.GetGenericArguments() })
                 .Where(x => x.A[0].IsGenericType
                             && x.A[0].GetGenericTypeDefinition() == typeof(Func<,>))
                 .Select(x => new { x.M, A = x.A[0].GetGenericArguments() })
                 .Where(x => x.A[0].IsGenericParameter
                             && x.A[1] == typeof(bool))
                 .Select(x => x.M)
                 .SingleOrDefault();

                retVal = whereMethod.MakeGenericMethod(elementType).Invoke(null, new object[] { queryable, lambda });

            }

            return retVal;
        }

        /// <summary>
        /// Sorts the elements of an array by a given attribute of an element in the array.
        /// {% assign sorted = pages | sort:"date:desc;name" %}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static object SortList(object input, string sort)
        {
            var retVal = input;
            IEnumerable enumerable = retVal as IEnumerable;
            IMutablePagedList muttablePagedList = input as IMutablePagedList;

            var sortInfos = SortInfo.Parse(sort).ToList();
            if (muttablePagedList != null)
            {
                muttablePagedList.Slice(muttablePagedList.PageNumber, muttablePagedList.PageSize, sortInfos);
            }
            if (enumerable != null)
            {
                //Queryable.Cast<T>(input).OrderBySortInfos(sortInfos) call by reflection
                var queryable = enumerable.AsQueryable();
                var elementType = enumerable.GetType().GetEnumerableType();
                MethodInfo castMethodInfo = typeof(Queryable).GetMethods().Where(x => x.Name == "Cast" && x.IsGenericMethod).First();
                castMethodInfo = castMethodInfo.MakeGenericMethod(new Type[] { elementType });

                var genericQueryable = castMethodInfo.Invoke(null, new object[] { queryable });

                var orderBySortInfosMethodInfo = typeof(IQueryableExtensions).GetMethod("OrderBySortInfos");
                orderBySortInfosMethodInfo = orderBySortInfosMethodInfo.MakeGenericMethod(new Type[] { elementType });
                retVal = orderBySortInfosMethodInfo.Invoke(null, new object[] { genericQueryable, sortInfos.ToArray() });
            }

            return retVal;
        }

        private static object ParseString(string str)
        {
            int intValue;
            double doubleValue;
            char charValue;
            bool boolValue;
            TimeSpan timespan;
            DateTime dateTime;

            // Place checks higher if if-else statement to give higher priority to type.
            if (int.TryParse(str, out intValue))
                return intValue;
            else if (double.TryParse(str, out doubleValue))
                return doubleValue;
            else if (TimeSpan.TryParse(str, out timespan))
                return timespan;
            else if (DateTime.TryParse(str, out dateTime))
                return dateTime;
            else if (char.TryParse(str, out charValue))
                return charValue;
            else if (bool.TryParse(str, out boolValue))
                return boolValue;

            return str;
        }
    }

}
