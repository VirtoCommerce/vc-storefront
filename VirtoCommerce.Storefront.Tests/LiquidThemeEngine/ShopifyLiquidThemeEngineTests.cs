using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Features;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.LiquidThemeEngine
{
    public sealed class ShopifyLiquidThemeEngineTests : IDisposable
    {
        private enum DefaultThemeType
        {
            WithoutPresets,
            WithPresets,
            WithPresetsAndCurrentObject
        }

        private static readonly string ThemesPath = "Themes";
        private static readonly string BaseThemePath = "odt\\default";
        private static readonly string CurrentThemePath = "odt\\current";
        private static readonly string SettingsPath = "config\\settings_data.json";

        private static JObject DefaultSettingsWithoutPresets => JObject.Parse(@"
        {
            'background_color': '#fff',
            'foreground_color': '#000'
        }
        ");

        private static JObject DefaultSettingsWithPresets => JObject.Parse(@"
        {
            'current': 'Light',
            'presets': {
                'Dark': {
                    'background_color': '#000',
                    'foreground_color': '#fff'
                },
                'Light': {
                    'background_color': '#fff',
                    'foreground_color': '#000'
                }
            }
        }
        ");

        private static JObject DefaultSettingsWithPresetsAndCurrentObject => JObject.Parse(@"
        {
            'current': {
                'background_color': '#fff',
                'foreground_color': '#000'
            },
            'presets': {
                'Dark': {
                    'background_color': '#000',
                    'foreground_color': '#fff'
                },
                'Light': {
                    'background_color': '#fff',
                    'foreground_color': '#000'
                }
            }
        }
        ");

        private static JObject CurrentSettingsWithoutSelectedPreset => JObject.Parse(@"
        {
            'foreground_color': '#333'
        }
        ");

        private static JObject CurrentSettingsWithSelectedPreset => JObject.Parse(@"
        {
            'current': 'Dark',
            'foreground_color': '#333'
        }
        ");

        private readonly StreamWriter _defaultThemeStreamWriter = new StreamWriter(new MemoryStream()) { AutoFlush = true };
        private readonly StreamWriter _currentThemeStreamWriter = new StreamWriter(new MemoryStream()) { AutoFlush = true };

        private Stream DefaultThemeStream { get; set; }
        private Stream CurrentThemeStream { get; set; }

        [Fact]
        public void Settings_Without_Inheritance_Flat()
        {
            InitializeStreams(DefaultThemeType.WithoutPresets, false);
            Check_Without_Inheritance();
        }

        [Fact]
        public void Settings_Without_Inheritance_Presets()
        {
            InitializeStreams(DefaultThemeType.WithPresets, false);
            Check_Without_Inheritance();
        }

        [Fact]
        public void Settings_Without_Inheritance_Presets_And_Current_Object()
        {
            InitializeStreams(DefaultThemeType.WithPresetsAndCurrentObject, false);
            Check_Without_Inheritance();
        }

        private void Check_Without_Inheritance()
        {
            var options = new LiquidThemeEngineOptions();
            var shopifyLiquidThemeEngine = GetThemeEngine(false, options);
            var settings = shopifyLiquidThemeEngine.GetSettings();
            Assert.Equal("#fff", settings["background_color"]);
            Assert.Equal("#000", settings["foreground_color"]);
        }

        [Fact]
        public void Settings_Inheritance_Backward_Compatibility_Base_Theme_Name()
        {
            var options = new LiquidThemeEngineOptions()
            {
#pragma warning disable 618
                BaseThemeName = "odt"
#pragma warning restore 618
            };

            Check_Inheritance_Backward_Compatibility(options);
        }

        [Fact]
        public void Settings_Inheritance_Backward_Compatibility_Base_Theme_Path_Without_Merge()
        {
            var options = new LiquidThemeEngineOptions()
            {
                BaseThemePath = "odt\\default"
            };

            Check_Inheritance_Backward_Compatibility(options);
        }

        private void Check_Inheritance_Backward_Compatibility(LiquidThemeEngineOptions options)
        {
            var shopifyLiquidThemeEngine = GetThemeEngine(true, options);
            InitializeStreams(DefaultThemeType.WithoutPresets, false);
            var settings = shopifyLiquidThemeEngine.GetSettings();
            Assert.False(settings.ContainsKey("background_color"));
            Assert.Equal("#333", settings["foreground_color"]);
        }

        [Fact]
        public void Settings_Inheritance_Both_Are_Flat()
        {
            InitializeStreams(DefaultThemeType.WithoutPresets, false);
            Check_Colors_In_Merged_Settings();
        }

        [Fact]
        public void Settings_Inheritance_Base_Has_Preset_Current_Is_Flat()
        {
            InitializeStreams(DefaultThemeType.WithPresets, false);
            Check_Colors_In_Merged_Settings();
        }

        [Fact]
        public void Settings_Inheritance_Current_Select_Preset_From_Base()
        {
            InitializeStreams(DefaultThemeType.WithPresets, true);
            Check_Colors_In_Merged_Settings(true);
        }

        private void Check_Colors_In_Merged_Settings(bool isDarkPreset = false)
        {
            var options = new LiquidThemeEngineOptions()
            {
                BaseThemePath = "odt\\default",
                MergeBaseSettings = true
            };
            var shopifyLiquidThemeEngine = GetThemeEngine(true, options);
            var settings = shopifyLiquidThemeEngine.GetSettings();
            Assert.Equal(isDarkPreset ? "#000" : "#fff", settings["background_color"]);
            Assert.Equal("#333", settings["foreground_color"]);
        }

        private void InitializeStreams(DefaultThemeType defaultThemeType, bool currentThemeHasSelectedPreset)
        {
            JObject defaultThemeJson;
            switch (defaultThemeType)
            {
                case DefaultThemeType.WithoutPresets:
                    defaultThemeJson = DefaultSettingsWithoutPresets;
                    break;
                case DefaultThemeType.WithPresets:
                    defaultThemeJson = DefaultSettingsWithPresets;
                    break;
                case DefaultThemeType.WithPresetsAndCurrentObject:
                    defaultThemeJson = DefaultSettingsWithPresetsAndCurrentObject;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(defaultThemeType), defaultThemeType, null);
            }

            InitializeStream(_defaultThemeStreamWriter, out var defaultThemeStream, defaultThemeJson);
            DefaultThemeStream = defaultThemeStream;

            InitializeStream(_currentThemeStreamWriter, out var currentThemeStream, currentThemeHasSelectedPreset ? CurrentSettingsWithSelectedPreset : CurrentSettingsWithoutSelectedPreset);
            CurrentThemeStream = currentThemeStream;
        }

        private void InitializeStream<T>(StreamWriter writer, out Stream stream, T content)
        {
            // Clear
            writer.BaseStream.Position = 0;
            writer.Flush();
            // Write
            writer.Write(content);
            // Reset position
            writer.BaseStream.Position = 0;
            // Copy, because stream reader will automatically destroy it
            stream = new MemoryStream();
            writer.BaseStream.CopyTo(stream);
            stream.Position = 0;
        }

        private IContentBlobProvider ContentBlobProvider
        {
            get
            {
                var mock = new Mock<IContentBlobProvider>();
                var baseThemeSettingsPath = Path.Combine(ThemesPath, BaseThemePath, SettingsPath);
                mock.Setup(service => service.PathExists(baseThemeSettingsPath))
                    .Returns(() => true);
                mock.Setup(service => service.OpenRead(baseThemeSettingsPath))
                    .Returns(() => DefaultThemeStream);
                var currentThemeSettingsPath = Path.Combine(ThemesPath, CurrentThemePath, SettingsPath);
                mock.Setup(service => service.PathExists(currentThemeSettingsPath))
                    .Returns(() => true);
                mock.Setup(service => service.OpenRead(currentThemeSettingsPath))
                    .Returns(() => CurrentThemeStream);
                return mock.Object;
            }
        }

        public IStorefrontMemoryCache MemoryCache
        {
            get
            {
                var cacheEntry = Mock.Of<ICacheEntry>();
                Mock.Get(cacheEntry).SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
                var memoryCacheMock = new Mock<IStorefrontMemoryCache>();
                memoryCacheMock
                    .Setup(x => x.CreateEntry(It.IsAny<string>()))
                    .Returns((string key) => cacheEntry);

                memoryCacheMock.Setup(c => c.GetDefaultCacheEntryOptions()).Returns(
                    new MemoryCacheEntryOptions());
                return memoryCacheMock.Object;
            }
        }

        private IWorkContextAccessor GetWorkContextAccessor(bool useThemesInheritance)
        {
            var mock = new Mock<IWorkContextAccessor>();
            mock.Setup(service => service.WorkContext)
                .Returns(() => new WorkContext
                {
                    CurrentStore = new Store
                    {
                        Id = "odt",
                        ThemeName = useThemesInheritance ? "current" : "default"
                    }
                });
            return mock.Object;
        }

        private IHttpContextAccessor HttpContextAccessor
        {
            get
            {
                var mock = new Mock<IHttpContextAccessor>();
                mock.Setup(service => service.HttpContext.Request.Query["preview_mode"])
                    .Returns(() => Enumerable.Empty<string>().ToArray());
                return mock.Object;
            }
        }

        private ShopifyLiquidThemeEngine GetThemeEngine(bool useThemesInheritance, LiquidThemeEngineOptions options)
        {
            return new ShopifyLiquidThemeEngine(MemoryCache, GetWorkContextAccessor(useThemesInheritance), HttpContextAccessor,
                null, ContentBlobProvider, null, new OptionsWrapper<LiquidThemeEngineOptions>(options), new FeaturesAgent(), null);
        }

        public void Dispose()
        {
            MemoryCache.Dispose();
            _defaultThemeStreamWriter.Dispose();
            _currentThemeStreamWriter.Dispose();
        }
    }
}
