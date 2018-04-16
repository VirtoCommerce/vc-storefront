using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class ReflectionExtension
    {
        public static IEnumerable<PropertyInfo> GetTypePropsRecursively(this Type baseType, Func<PropertyInfo, bool> predicate)
        {
            return IteratePropsInner(baseType, predicate);
        }

        private static IEnumerable<PropertyInfo> IteratePropsInner(Type baseType, Func<PropertyInfo, bool> predicate)
        {
            var props = baseType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in props.Where(x=> predicate(x)))
            {
                var type = ListTypesOrSelf(property.PropertyType);

                foreach (var info in IteratePropsInner(type, predicate))
                    yield return info;

                yield return property;
            }
        }

        public static Type ListTypesOrSelf(Type type)
        {
            if (!type.IsGenericType)
                return type;
            return type.GetGenericArguments()[0];
        }

        public static Type GetEnumerableType(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }
            return null;
        }

        public static bool IsAssignableFromGenericList(this Type type)
        {
            foreach (var intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return true;
                }
            }
            return false;
        }

        public static T[] GetFlatObjectsListWithInterface<T>(this object obj, List<T> resultList = null)
        {
            var retVal = new List<T>();

            if (resultList == null)
            {
                resultList = new List<T>();
            }
            //Ignore cycling references
            if (!resultList.Any(x => object.ReferenceEquals(x, obj)))
            {
                var objectType = obj.GetType();

                if (objectType.GetInterface(typeof(T).Name) != null)
                {
                    retVal.Add((T)obj);
                    resultList.Add((T)obj);
                }

                var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var objects = properties.Where(x => x.PropertyType.GetInterface(typeof(T).Name) != null)
                                        .Select(x => (T)x.GetValue(obj)).ToList();

                //Recursive call for single properties
                retVal.AddRange(objects.Where(x => x != null).SelectMany(x => x.GetFlatObjectsListWithInterface(resultList)));

                //Handle collection and arrays
                var collections = properties.Where(p => p.GetIndexParameters().Length == 0)
                                            .Select(x => x.GetValue(obj, null))
                                            .Where(x => x is IEnumerable && !(x is String))
                                            .Cast<IEnumerable>();

                foreach (var collection in collections)
                {
                    foreach (var collectionObject in collection)
                    {
                        if (collectionObject is T)
                        {
                            retVal.AddRange(collectionObject.GetFlatObjectsListWithInterface(resultList));
                        }
                    }
                }
            }
            return retVal.ToArray();
        }

        public static IDictionary<string, object> AsDictionary(this object source, Func<string, string> nameNormalizer = null, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => nameNormalizer != null ? nameNormalizer(propInfo.Name) : propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
