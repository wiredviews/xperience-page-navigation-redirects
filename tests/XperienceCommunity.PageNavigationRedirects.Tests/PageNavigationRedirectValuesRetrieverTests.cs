using System;
using AutoFixture;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Tests;
using FluentAssertions;
using NUnit.Framework;
using Tests.DocumentEngine;

namespace XperienceCommunity.PageNavigationRedirects.Tests
{
    [TestFixture]
    public class PageNavigationRedirectValuesRetrieverTests : UnitTests
    {
        [SetUp]
        public void Setup()
        {
            DocumentGenerator.RegisterDocumentType<TreeNode>(TreeNode.TYPEINFO.ObjectType);

            Fake().DocumentType<TreeNode>(TreeNode.TYPEINFO.ObjectType);
        }

        [Test]
        public void RedirectionType_Will_Return_None_When_No_Data()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.None);
        }

        [Test]
        public void RedirectionType_Will_Return_None_When_Value_Cannot_Be_Parsed()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.Value.RedirectionTypeFieldName, "hello");
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.None);
        }

        [Test]
        public void RedirectionType_Will_Return_Custom_Value_When_Set()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.RedirectionTypeFieldName, PageRedirectionType.FirstChild, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.FirstChild);
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_null_When_No_Data()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? url = sut.ExternalRedirectURL(page);

            url.Should().BeNull();
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_null_When_Value_Is_Empty()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.ExternalRedirectURLFieldName, "", options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? url = sut.ExternalRedirectURL(page);

            url.Should().BeNull();
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_Custom_Value_When_Set()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();
            string redirectUrl = fixture.Create<string>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.ExternalRedirectURLFieldName, redirectUrl, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? url = sut.ExternalRedirectURL(page);

            url.Should().Be(redirectUrl);
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_null_When_No_Data()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().BeNull();
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_null_When_Default_Value()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.InternalRedirectNodeGUIDFieldName, default(Guid), options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().BeNull();
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_Custom_Value_When_Set()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();
            Guid guid = fixture.Create<Guid>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.InternalRedirectNodeGUIDFieldName, guid, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().Be(guid);
        }

        [Test]
        public void FirstChildClassName_Will_Return_null_When_No_Data()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? className = sut.FirstChildClassName(page);

            className.Should().BeNull();
        }

        [Test]
        public void FirstChildClassName_Will_Return_null_When_Default_Value()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.FirstChildClassNameFieldName, "", options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? className = sut.FirstChildClassName(page);

            className.Should().BeNull();
        }

        [Test]
        public void FirstChildClassName_Will_Return_Custom_Value_When_Set()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();
            string codeName = fixture.Create<string>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.FirstChildClassNameFieldName, codeName, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            string? className = sut.FirstChildClassName(page);

            className.Should().Be(codeName);
        }

        [Test]
        public void UsePermanentRedirects_Will_Return_Options_Value_When_No_Value()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.PageUsePermanentRedirectsFieldName, null, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            bool usePermanentRedirects = sut.UsePermanentRedirects(page);

            usePermanentRedirects.Should().Be(options.Value.UsePermanentRedirect);
        }

        [TestCase(-1, true, true)]
        [TestCase(-1, false, false)]
        [TestCase(0, true, false)]
        [TestCase(1, false, true)]
        public void UsePermanentRedirects_Will_Return_Custom_Value_When_Set(int value, bool defaultValue, bool expected)
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();
            options.Value.UsePermanentRedirect = defaultValue;

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.SetPageDatasourceValue(options.Value.PageUsePermanentRedirectsFieldName, value, options.Value);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(options);

            bool usePermanentRedirects = sut.UsePermanentRedirects(page);

            usePermanentRedirects.Should().Be(expected);
        }
    }
}
