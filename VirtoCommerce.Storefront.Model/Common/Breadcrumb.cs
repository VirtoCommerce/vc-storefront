namespace VirtoCommerce.Storefront.Model.Common
{
    public abstract class Breadcrumb : ValueObject
    {
        protected Breadcrumb(string type)
        {
            TypeName = type;
        }
        public string TypeName { get; private set; }
        public virtual string Title { get; set; }
        public virtual string SeoPath { get; set; }
        public virtual string Url { get; set; }
    }
}
