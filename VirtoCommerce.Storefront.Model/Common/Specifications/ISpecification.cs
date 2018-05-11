using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Common.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);   
    }
}
