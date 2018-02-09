using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public abstract class ContentItem : IHasLanguage
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

        public IDictionary<string, IEnumerable<string>> MetaInfo { get; set; }

        public virtual void LoadContent(string content, IDictionary<string, IEnumerable<string>> metaInfoMap)
        {
            if (metaInfoMap != null)
            {
                foreach (var setting in metaInfoMap)
                {
                    var settingValue = setting.Value.FirstOrDefault();
                    switch (setting.Key.ToLower())
                    {
                        case "permalink":
                            Permalink = settingValue;
                            break;

                        case "aliases":
                            Aliases = setting.Value.ToList();
                            break;

                        case "title":
                            Title = settingValue;
                            break;

                        case "author":
                            Author = settingValue;
                            break;

                        case "published":
                            bool isPublished;
                            IsPublished = bool.TryParse(settingValue, out isPublished) ? isPublished : true;
                            break;

                        case "date":
                            DateTime date;
                            PublishedDate = CreatedDate = DateTime.TryParse(settingValue, out date) ? date : new DateTime();
                            break;
                        case "tags":
                            Tags = setting.Value.ToList();
                            break;

                        case "categories":
                            Categories = setting.Value.ToList();
                            break;

                        case "category":
                            Category = settingValue;
                            break;

                        case "layout":
                            Layout = settingValue;
                            break;

                        case "priority":
                            int priority;
                            Priority = int.TryParse(settingValue, out priority) ? priority : 0;
                            break;

                        case "description":
                            Description = settingValue;
                            break;

                        case "authorize":
                            bool isAuthorize;
                            Authorize = bool.TryParse(settingValue, out isAuthorize) ? isAuthorize : false;
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
