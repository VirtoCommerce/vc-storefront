using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class BlogArticle : ContentItem
    {
        private static readonly string _excerpToken = "<!--excerpt-->";

        public override string Type { get { return "post"; } }

        public string Excerpt { get; set; }

        public string BlogName { get; set; }

        public string ImageUrl { get; set; }

        public bool IsSticked { get; set; }

        public bool IsTrending { get; set; }

        public override void LoadContent(string content, IDictionary<string, IEnumerable<string>> metaInfoMap)
        {
            var parts = content.Split(new[] { _excerpToken }, StringSplitOptions.None);

            if (parts.Length > 1)
            {
                Excerpt = parts[0];
                content = content.Replace(_excerpToken, string.Empty);
            }

            if (metaInfoMap.ContainsKey("main-image"))
            {
                ImageUrl = metaInfoMap["main-image"].FirstOrDefault();
            }

            if (metaInfoMap.ContainsKey("excerpt"))
            {
                Excerpt = metaInfoMap["excerpt"].FirstOrDefault();
            }

            if (metaInfoMap.ContainsKey("is-sticked"))
            {

                bool.TryParse(metaInfoMap["is-sticked"].FirstOrDefault(), out var isSticked);

                IsSticked = isSticked;
            }

            if (metaInfoMap.ContainsKey("is-trending"))
            {
                bool.TryParse(metaInfoMap["is-trending"].FirstOrDefault(), out var isTrending);

                IsTrending = isTrending;
            }

            base.LoadContent(content, metaInfoMap);
        }
    }
}
