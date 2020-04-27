using System;
using System.Collections.Generic;
using Markdig;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model.StaticContent;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.StaticContent
{
    public class StaticContentItemBuilderTests
    {
        private static IEnumerable<IContentItemVisitor> visitors = new List<IContentItemVisitor>
        {
            new LangVisitor(),
            new YamlMetadataVisitor(),
            new PageMetadataVisitor(),
            new BlogExcerptMetadataVisitor(),
            new BlogMetadataVisitor(),
            new ContentPageMetadataVisitor(),
            new MetadataVisitor(),
            new MarkdownVisitor(new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()),
            new PageContentVisitor(),
            new UrlsVisitor()
        };

        private IStaticContentItemBuilder builder = new StaticContentItemBuilder(new StaticContentItemFactory(), visitors);

        [Fact]
        public void ReadContent_For_MarkdownStaticPage()
        {

            var result = builder.BuildFrom("", "custom/path/filename.md", StaticMarkdownPage);
            Assert.Equal("Custom page title", result.Title);
            Assert.Equal("Custom page description", result.Description);
            Assert.Equal(new DateTime(2020, 04, 27), result.PublishedDate);
            Assert.Equal("custom/path", result.Permalink);
            Assert.Equal("<p>some content</p>\n", result.Content);
        }

        [Fact]
        public void EmptyFile_Should_CorrectlyPrcessed()
        {
            var result = builder.BuildFrom("", "custom/file/filename.md", "");
            Assert.NotNull(result);
        }

        [Fact]
        public void BlogArticle_ShouldRead_ExcerptFromContent()
        {
            var result = (BlogArticle)builder.BuildFrom("", "blogs/news/article.md", BlogArticleWithExcerptInContent);
            Assert.Equal("some excerpt\r\n", result.Excerpt);
        }

        [Fact]
        public void JsonPage_Should_ReadMetadata()
        {
            var result = builder.BuildFrom("", "pages/page.page", StaticJsonPage);
            Assert.Equal("Custom page title", result.Title);
            Assert.Equal("custom/path", result.Permalink);
            Assert.Equal("page description", result.Description);
            Assert.Equal(StaticJsonPage, result.Content);
        }

        [Fact]
        public void PageLang_ShouldBeInvariant_WhenEmpty()
        {
            var result = builder.BuildFrom("", "pages/page.md", StaticMarkdownPage);
            Assert.True(result.Language.IsInvariant);
        }

        [Fact]
        public void PageLang_ShouldBe_AsInFilename()
        {
            var result = builder.BuildFrom("", "pages/page.en-us.md", StaticMarkdownPage);
            Assert.Equal("en-US", result.Language.CultureName);
        }

        [Fact]
        public void JsonArticle_ShouldBe_ProcessedCorrectly()
        {
            var result = (BlogArticle)builder.BuildFrom("", "blogs/blog-name/article.page", StaticJsonPage);
            Assert.Equal("Custom page title", result.Title);
            Assert.Equal("custom/path", result.Permalink);
            Assert.Equal("page description", result.Description);
            Assert.Equal("article excerpt", result.Excerpt);
        }

        private static string StaticMarkdownPage = @"---
title: Custom page title
description: Custom page description
date: 2020-04-27
permalink: custom/path
---
some content
";
        private static string BlogArticleWithExcerptInContent = @"---
---
some excerpt
<!--excerpt-->
some content
";
        private static string StaticJsonPage = @"[
    {
        ""type"": ""settings"",
        ""title"": ""Custom page title"",
        ""permalink"": ""custom/path"",
        ""description"": ""page description"",
        ""excerpt"": ""article excerpt""
    },
    {
        ""type"": ""content"",
        ""content"": ""<p>block content</p>""
    }
]";
    }
}
