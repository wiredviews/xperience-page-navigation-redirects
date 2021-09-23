using System;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Tests;
using FluentAssertions;
using Microsoft.Extensions.Options;
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
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.None);
        }

        [Test]
        public void RedirectionType_Will_Return_None_When_Value_Cannot_Be_Parsed()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.RedirectionTypeFieldName, "hello");
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.None);
        }

        [Test]
        public void RedirectionType_Will_Return_Custom_Value_When_Set()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.RedirectionTypeFieldName, PageRedirectionType.FirstChild);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.FirstChild);
        }

        [Test]
        public void RedirectionType_Will_Return_Custom_Value_When_Set_With_Custom_Options()
        {
            var options = new PageNavigationRedirectOptions
            {
                RedirectionTypeFieldName = "abc"
            };

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.RedirectionTypeFieldName, PageRedirectionType.FirstChild);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            var redirectionType = sut.RedirectionType(page);

            redirectionType.Should().Be(PageRedirectionType.FirstChild);
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_null_When_No_Data()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? url = sut.ExternalRedirectURL(page);

            url.Should().BeNull();
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_null_When_Value_Is_Empty()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.ExternalRedirectURLFieldName, "");
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? url = sut.ExternalRedirectURL(page);

            url.Should().BeNull();
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_Custom_Value_When_Set()
        {
            string data = "https://localhost";

            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.ExternalRedirectURLFieldName, data);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? url = sut.ExternalRedirectURL(page);

            url.Should().Be(data);
        }

        [Test]
        public void ExternalRedirectURL_Will_Return_Custom_Value_When_Set_With_Custom_Options()
        {
            string data = "https://localhost";

            var options = new PageNavigationRedirectOptions
            {
                ExternalRedirectURLFieldName = "abc"
            };

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.ExternalRedirectURLFieldName, data);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? url = sut.ExternalRedirectURL(page);

            url.Should().Be(data);
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_null_When_No_Data()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().BeNull();
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_null_When_Default_Value()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.InternalRedirectNodeGUIDFieldName, default(Guid));
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().BeNull();
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_Custom_Value_When_Set()
        {
            Guid guid = Guid.NewGuid();

            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.InternalRedirectNodeGUIDFieldName, guid);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().Be(guid);
        }

        [Test]
        public void InternalRedirectNodeGUID_Will_Return_Custom_Value_When_Set_With_Custom_Options()
        {
            Guid guid = Guid.NewGuid();

            var options = new PageNavigationRedirectOptions
            {
                InternalRedirectNodeGUIDFieldName = "abc"
            };

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.InternalRedirectNodeGUIDFieldName, guid);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            Guid? nodeGUID = sut.InternalRedirectNodeGUID(page);

            nodeGUID.Should().Be(guid);
        }

        [Test]
        public void FirstChildClassName_Will_Return_null_When_No_Data()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {

            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? className = sut.FirstChildClassName(page);

            className.Should().BeNull();
        }

        [Test]
        public void FirstChildClassName_Will_Return_null_When_Default_Value()
        {
            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.FirstChildClassNameFieldName, "");
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? className = sut.FirstChildClassName(page);

            className.Should().BeNull();
        }

        [Test]
        public void FirstChildClassName_Will_Return_Custom_Value_When_Set()
        {
            string codeName = "Sandbox.PageType";

            var options = new PageNavigationRedirectOptions();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.FirstChildClassNameFieldName, codeName);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? className = sut.FirstChildClassName(page);

            className.Should().Be(codeName);
        }

        [Test]
        public void FirstChildClassName_Will_Return_Custom_Value_When_Set_With_Custom_Options()
        {
            string codeName = "Sandbox.PageType";

            var options = new PageNavigationRedirectOptions
            {
                FirstChildClassNameFieldName = "abc"
            };

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.FirstChildClassNameFieldName, codeName);
            });

            var sut = new PageNavigationRedirectsValuesRetriever(Options.Create(options));

            string? className = sut.FirstChildClassName(page);

            className.Should().Be(codeName);
        }
    }
}
