using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Domain
{
    public class FileSystemBlobContentOptions
    {
        public string Path { get; set; }
        public bool PollForChanges { get; set; } = true;
    }
}
