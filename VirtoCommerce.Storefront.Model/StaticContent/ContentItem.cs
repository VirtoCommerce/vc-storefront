using System;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public abstract class ContentItem : IHasLanguage, IAccessibleByIndexKey
    {
        protected ContentItem()
        {
        }

        public virtual string Type => "page";

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Relative content url
        /// </summary>
        public string Url { get; set; }

        public string Permalink { get; set; }

        public string Template { get; set; }

        /// <summary>
        /// Represent alternative urls which will be used for redirection to main url
        /// </summary>
        public IList<string> Aliases { get; set; } = new List<string>();
        public IList<string> AliasesUrls { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();

        public List<string> Categories { get; set; } = new List<string>();

        public string Category { get; set; }

        public bool IsPublished { get; set; } = true;

        public DateTime? PublishedAt => PublishedDate ?? CreatedDate;

        /// <summary>
        /// Content file name without extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Relative storage path in storage system (/blogs/page1)
        /// </summary>
        public string StoragePath { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// Liquid layout from store theme used as master page for page rendering. If its null will be used default layout.
        /// </summary>
        public string Layout { get; set; }

        public string FileName { get; set; }

        public Language Language { get; set; }

        public int Priority { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Limits anonymous access to the page. Set true to block anonymous access. Set false to grant anonymous access.  Default value is false. 
        /// </summary>
        public bool Authorize { get; set; }

        public virtual string Handle => Url;
        public IDictionary<string, IEnumerable<string>> MetaInfo { get; set; }
        public IDictionary<string, IEnumerable<string>> MetaFields => MetaInfo;

        public virtual string IndexKey => Handle;

        public override string ToString()
        {
            return Url ?? Name;
        }
    }
}
