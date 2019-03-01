using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class SearchCategoriesResult
    {
        public IPagedList<Category> Categories { get; set; }
        public IPagedList MetaData { get; set; }
    }
}
