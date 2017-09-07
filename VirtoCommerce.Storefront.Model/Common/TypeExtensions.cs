using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class TypeExtensions
    {
        public static Type[] GetTypeInheritanceChainTo(this Type type, Type toBaseType)
        {
            var retVal = new List<Type> { type };

            var baseType = type.BaseType;

            while (baseType != toBaseType && baseType != typeof(object))
            {
                retVal.Add(baseType);
                baseType = baseType.BaseType;
            }

            return retVal.ToArray();
        }
    }
}
