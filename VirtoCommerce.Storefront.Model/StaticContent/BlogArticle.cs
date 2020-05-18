namespace VirtoCommerce.Storefront.Model.StaticContent
{
    /// <summary>
    /// TODO: Comments and author user info
    /// </summary>
    public partial class BlogArticle : ContentItem
    {
        public override string Type { get { return "post"; } }

        public string Excerpt { get; set; }

        public string BlogName { get; set; }

        public string ImageUrl { get; set; }

        public bool IsSticked { get; set; }

        public bool IsTrending { get; set; }
    }
}
