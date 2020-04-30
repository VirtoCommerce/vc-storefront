using System;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model.StaticContent;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.StaticContent
{
    public class StaticContentItemBuilderTests
    {
        private IStaticContentItemBuilder builder = new StaticContentItemBuilder(new StaticContentItemFactory(), new ContentItemReaderFactory(), new ContentRestorerFactory());

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

        [Fact]
        public void PageTemplate_ShouldBe_JsonPage()
        {
            var result = builder.BuildFrom("", "pages/custom-page.page", string.Empty);
            Assert.Equal("json-page", result.Template);
        }

        [Fact]
        public void PageTemplate_ShouldBe_Page()
        {
            var result = builder.BuildFrom("", "pages/custom-page.md", string.Empty);
            Assert.Equal("page", result.Template);
        }

        [Fact]
        public void PageTemplate_ShouldBe_Article()
        {
            var result = builder.BuildFrom("", "blogs/blog/article.md", string.Empty);
            Assert.Equal("article", result.Template);
        }

        [Fact]
        public void PageTemplate_ShouldBe_JsonArticle()
        {
            var result = builder.BuildFrom("", "blogs/blog/article.page", string.Empty);
            Assert.Equal("json-article", result.Template);
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
