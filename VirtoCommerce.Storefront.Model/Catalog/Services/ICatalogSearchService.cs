using PagedList.Core;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Services
{
    /// <summary>
    /// Represent abstraction to search in catalog api (products, categories etc)
    /// </summary>
    public interface ICatalogService
    {
        Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup);

        Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup);

        Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria);

        Task<IPagedList<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria);

        CatalogSearchResult SearchProducts(ProductSearchCriteria criteria);

        IPagedList<Category> SearchCategories(CategorySearchCriteria criteria);
    }
}
