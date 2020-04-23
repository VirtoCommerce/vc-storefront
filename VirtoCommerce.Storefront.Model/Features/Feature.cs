namespace VirtoCommerce.Storefront.Model.Features
{
    using System.Collections.Generic;

    public class Feature
    {
        public List<string> Conflicts { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public List<string> Replaces { get; set; }

        public List<string> Requires { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
