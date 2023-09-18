using System;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public class ContentInThemeSearchCriteria
    {
        public string Permalink { get; set; }

        [Obsolete("Use TemplateName instead")]
        public string Template
        {
            get => TemplateName;
            set => TemplateName = value;
        }

        [Required]
        public string TemplateName { get; set; }
    }
}
