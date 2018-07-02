using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Domain
{
    public class FileSystemBlobContentOptions
    {
        public string Path { get; set; }
        //Need to be careful with enabling this setting, this can lead to serious performance issues
        public bool PollForChanges { get; set; } = false;
    }
}
