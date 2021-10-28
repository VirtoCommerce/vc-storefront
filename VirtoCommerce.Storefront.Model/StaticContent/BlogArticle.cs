using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    /// <summary>
    /// TODO: Comments and author user info
    /// </summary>
    public partial class BlogArticle : ContentItem
    {
        private static string _excerpToken = "<!--excerpt-->";

        public override string Type { get { return "post"; } }

        public string Excerpt { get; set; }

        public string BlogName { get; set; }

        public string ImageUrl { get; set; }

        public bool IsSticked { get; set; }

        public bool IsTrending { get; set; }

        public new Dictionary<string, object> MetaFields { get; set; }

        public override void LoadContent(string content, IDictionary<string, object> metaInfoMap)
        {
            var parts = content.Split(new[] { _excerpToken }, StringSplitOptions.None);

            if (parts.Length > 1)
            {
                Excerpt = parts[0];
                content = content.Replace(_excerpToken, string.Empty);
            }

            if (metaInfoMap.ContainsKey("main-image"))
            {
                ImageUrl = metaInfoMap["main-image"].ToString();
            }

            if (metaInfoMap.ContainsKey("excerpt"))
            {
                Excerpt = metaInfoMap["excerpt"].ToString();
            }

            if (metaInfoMap.ContainsKey("is-sticked"))
            {
                var isSticked = false;

                bool.TryParse(metaInfoMap["is-sticked"].ToString(), out isSticked);

                IsSticked = isSticked;
            }

            if (metaInfoMap.ContainsKey("is-trending"))
            {
                var isTrending = false;

                bool.TryParse(metaInfoMap["is-trending"].ToString(), out isTrending);

                IsTrending = isTrending;
            }

            MetaFields = new Dictionary<string, object>();
            foreach (var meta in metaInfoMap)
            {
                if (meta.Value is string)
                {
                    MetaFields.Add(meta.Key, meta.Value.ToString());
                }
                else
                {
                    MetaFields.Add(meta.Key, meta.Value);
                }
            }

            base.LoadContent(content, metaInfoMap);
        }
    }
}
