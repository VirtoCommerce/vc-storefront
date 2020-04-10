using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class CategoryBreadcrumb : Breadcrumb
    {
        public CategoryBreadcrumb(Category category) : base(nameof(Category))
        {
            Category = category;
        }

        public Category Category { get; private set; }
    }
}
