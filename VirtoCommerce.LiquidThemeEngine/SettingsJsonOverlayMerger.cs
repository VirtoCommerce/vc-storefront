using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.LiquidThemeEngine
{
    public static class SettingJsonOverlayMerger
    {
        internal class JsonSettingDoc
        {
            public JsonPreset CurrentPreset { get; set; }
            public IList<JsonPreset> Presets { get; set; } = new List<JsonPreset>();
        }

        internal class JsonPreset
        {
            public bool NameIsSet => !string.IsNullOrEmpty(Name);
            public string Name { get; set; }
            public JObject Json { get; set; }
        }

        public static JObject Merge(JObject baseJson, JObject headJson)
        {
            if (baseJson == null)
            {
                throw new ArgumentNullException(nameof(baseJson));
            }
            if (headJson == null)
            {
                throw new ArgumentNullException(nameof(headJson));
            }

            var baseSettingDoc = ReadSettingDoc(baseJson);
            var headSettingDoc = ReadSettingDoc(headJson);
            //Change the current preset for base doc according to head preset value if it specified
            if (headSettingDoc.CurrentPreset.NameIsSet)
            {
                baseSettingDoc.CurrentPreset = baseSettingDoc.Presets.FirstOrDefault(x => x.Name == headSettingDoc.CurrentPreset.Name);
            }
            var result = baseSettingDoc.CurrentPreset.Json;
            result.Merge(headSettingDoc.CurrentPreset.Json, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
            return result;
        }

        private static JsonSettingDoc ReadSettingDoc(JObject json)
        {
            var result = new JsonSettingDoc
            {
                CurrentPreset = new JsonPreset
                {
                    Json = json
                }
            };

            if (json.GetValue("presets") is JObject presets)
            {
                var allJsonPresets = presets.Children().Cast<JProperty>().ToList();
                foreach (var jsonPreset in allJsonPresets)
                {
                    var presset = new JsonPreset
                    {
                        Name = jsonPreset.Name,
                        Json = jsonPreset.Value as JObject
                    };
                    result.Presets.Add(presset);
                }
            }

            var currentPreset = json.GetValue("current");
            if (currentPreset is JValue currentPresetValue)
            {
                var presetName = currentPresetValue.Value.ToString();
                var currentJson = result.Presets.FirstOrDefault(x => x.Name == presetName)?.Json;
                if (currentJson == null && result.Presets.Any())
                {
                    throw new StorefrontException($"Setting preset with name '{presetName}' not found");
                }
                result.CurrentPreset = new JsonPreset { Name = presetName, Json = currentJson ?? json };
            }
            if (currentPreset is JObject preset)
            {
                result.CurrentPreset.Json = preset;
            }

            return result;
        }
    }
}
