using System;

namespace VirtoCommerce.Storefront.Model.Interaction
{
    public class UserEvent
    {
        public string Type { get; set; }

        public string Event { get; set; }

        public string TargetTag { get; set; }

        public string TargetClasses { get; set; }

        public string Content { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
