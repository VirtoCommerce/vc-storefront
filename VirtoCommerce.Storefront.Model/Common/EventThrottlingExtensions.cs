using System;
using System.IO;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class EventThrottlingExtensions
    {
        public static FileSystemEventHandler Throttle(this FileSystemEventHandler handler, TimeSpan throttle)
        {
            var throttling = false;
            return (s, e) =>
            {
                if (throttling)
                    return;
                handler(s, e);
                throttling = true;
                Task.Delay(throttle).ContinueWith(x => throttling = false);
            };
        }

    }
}
