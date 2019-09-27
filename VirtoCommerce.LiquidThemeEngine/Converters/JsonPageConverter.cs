using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    //public static class JsonPageConverter
    //{
    //    public static JsonPage ToShopifyModel(this StorefrontModel.JsonPage page)
    //    {
    //        var converter = new ShopifyModelConverter();
    //        return converter.ToLiquidCmsPage(page);
    //    }
    //}

    //public partial class ShopifyModelConverter
    //{
    //    public virtual JsonPage ToLiquidCmsPage(StorefrontModel.JsonPage source)
    //    {
    //        var result = new JsonPage
    //        {
    //            Settings = source.Settings,
    //            Blocks = new List<IDictionary<string, object>>()
    //        };

    //        foreach (var block in source.Blocks)
    //        {
    //            var collection = ProcessBlockRecursively(block);
    //            result.Blocks.Add(collection);
    //        }

    //        return result;
    //    }

    //    private MetafieldsCollection ProcessBlockRecursively(JObject block)
    //    {
    //        var result = new Dictionary<string, object>();
    //        foreach (var keyValue in block)
    //        {
    //            var key = keyValue.Key;
    //            var value = keyValue.Value;
    //            if (value is JArray)
    //            {
    //                var array = (JArray)value;
    //                var resultArray = new List<IDictionary<string, object>>();
    //                foreach (JObject item in array)
    //                {
    //                    resultArray.Add(ProcessBlockRecursively(item));
    //                }
    //                result.Add(key, resultArray);
    //            }
    //            else if (value is JObject v)
    //            {
    //                result.Add(key, ProcessBlockRecursively(v));
    //            }
    //            else
    //            {
    //                result.Add(key, value.ToString());
    //            }
    //        }
    //        return new MetafieldsCollection(string.Empty, result);
    //    }
    //}
}
