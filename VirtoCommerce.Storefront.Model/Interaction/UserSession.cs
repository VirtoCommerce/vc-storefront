using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Interaction
{
    public class UserSession
    {
        public DateTime? LoadTime { get; set; }

        public DateTime? UnloadTime { get; set; }

        public string Language { get; set; }

        public string Platform { get; set; }

        public string Port { get; set; }

        public Client ClientStart { get; set; }

        public Page Page { get; set; }

        public string Endpoint { get; set; }

        public UserEvent[] Interactions { get; set; }

        public Client ClientEnd { get; set; }
    }
}
