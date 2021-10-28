using System;
using System.Collections.Generic;
using System.Linq;
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
        public IDictionary<string, object> MetaInfo { get; set; }
        public IDictionary<string, object> MetaFields => MetaInfo;

        public virtual string IndexKey => Handle;

        public virtual void LoadContent(string content, IDictionary<string, object> metaInfoMap)
        {
            if (metaInfoMap != null)
            {
                foreach (var setting in metaInfoMap)
                {
                    var settingValue = setting.Value;
                    switch (setting.Key.ToLowerInvariant())
                    {
                        case "permalink":
                            Permalink = settingValue.ToString();
                            break;

                        case "aliases":
                            Aliases = setting.Value as List<string>;
                            break;

                        case "title":
                            Title = settingValue.ToString();
                            break;

                        case "author":
                            Author = settingValue.ToString();
                            break;

                        case "published":
                            bool isPublished;
                            IsPublished = bool.TryParse(settingValue.ToString(), out isPublished) ? isPublished : true;
                            break;

                        case "date":
                            DateTime date;
                            PublishedDate = CreatedDate = DateTime.TryParse(settingValue.ToString(), out date) ? date : new DateTime();
                            break;
                        case "tags":
                            Tags = (setting.Value as List<string>).OrderBy(t => t).Select(t => t.Handelize()).ToList();
                            break;

                        case "categories":
                            Categories = (setting.Value as List<string>).Select(x => x.Handelize()).ToList();
                            break;

                        case "category":
                            Category = settingValue.ToString()?.Handelize();
                            break;

                        case "layout":
                            Layout = settingValue.ToString();
                            break;

                        case "priority":
                            int priority;
                            Priority = int.TryParse(settingValue.ToString(), out priority) ? priority : 0;
                            break;

                        case "description":
                            Description = settingValue.ToString();
                            break;

                        case "authorize":
                            bool isAuthorize;
                            if (bool.TryParse(settingValue.ToString(), out isAuthorize))
                            {
                                Authorize = isAuthorize;
                            }
                            break;
                    }
                }
            }

            MetaInfo = metaInfoMap;
            Content = content;
            if (Title == null)
            {
                Title = Name;
            }
        }

        public override string ToString()
        {
            return Url ?? Name;
        }
    }
}
