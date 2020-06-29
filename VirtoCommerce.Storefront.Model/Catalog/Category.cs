using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Category : Entity, IAccessibleByIndexKey, IHasBreadcrumbs
    {
        public Category()
        {
            Images = new List<Image>();
            Properties = MutablePagedList<CatalogProperty>.Empty;
        }

        public string CatalogId { get; set; }

        //All parents categories
        [JsonIgnore]
        public IMutablePagedList<Category> Parents { get; set; }
        public string ParentId { get; set; }

        public string Code { get; set; }

        public string TaxType { get; set; }
        public string DefaultSortBy { get; set; } = "manual";
        public string Title => Name;
        public string Name { get; set; }

        /// <summary>
        /// All parent categories ids concatenated with "/". E.g. (1/21/344)
        /// </summary>
        public string Outline { get; set; }

        //Level in hierarchy
        public int Level => Outline?.Split("/").Count() ?? 0;

        /// <summary>
        /// Slug  path e.g /camcorders
        /// </summary>
        public string SeoPath { get; set; }
        /// <summary>
        /// Application relative url e.g ~/camcorders
        /// </summary>
        public string Url { get; set; }

        public SeoInfo SeoInfo { get; set; }

        [JsonIgnore]
        public int AllProductsCount { get { return Products.GetTotalCount(); } }

        /// <summary>
        /// Category main image
        /// </summary>
        public Image PrimaryImage { get; set; }
        public Image Image => PrimaryImage;

        public IList<Image> Images { get; set; }
        [JsonIgnore]
        public IMutablePagedList<Product> Products { get; set; }

        /// <summary>
        /// Child categories
        /// </summary>
        public IMutablePagedList<Category> Categories { get; set; }
        [JsonIgnore]
        public IMutablePagedList<Category> Collections => Categories;

        public override string ToString()
        {
            return SeoPath ?? base.ToString();
        }

        public IEnumerable<Breadcrumb> GetBreadcrumbs()
        {
            foreach (var parentCategory in Parents.Distinct())
            {
                if (!parentCategory.SeoPath.IsNullOrEmpty())
                {
                    yield return new CategoryBreadcrumb(parentCategory)
                    {
                        SeoPath = parentCategory.SeoPath,
                        Url =  parentCategory.SeoPath,
                        Title = parentCategory.Name,
                    };
                }
            }
            if (!SeoPath.IsNullOrEmpty())
            {
                yield return new CategoryBreadcrumb(this)
                {
                    Title = Title,
                    SeoPath = SeoPath,
                    Url = Url
                };
            }
        }

        public IMutablePagedList<CatalogProperty> Properties { get; set; }
        public string Handle => SeoInfo?.Slug ?? Id;
        public string IndexKey => Handle;
    }
}
