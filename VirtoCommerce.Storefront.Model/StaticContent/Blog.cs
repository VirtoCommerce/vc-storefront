using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class Blog : ContentItem
    {
        public Blog()
        {
            Permalink = ":folder";
        }

        public IMutablePagedList<BlogArticle> Articles { get; set; }

        public override string Handle => Name.Handelize();
    }
}