using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Tests;
using FluentAssertions;
using Kentico.Content.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using Tests.DocumentEngine;

namespace XperienceCommunity.PageNavigationRedirects.Tests
{
    [TestFixture]
    public class PageCustomDataRedirectResourceFilterTests : UnitTests
    {
        [SetUp]
        public void Setup()
        {
            DocumentGenerator.RegisterDocumentType<TreeNode>(TreeNode.TYPEINFO.ObjectType);

            Fake().DocumentType<TreeNode>(TreeNode.TYPEINFO.ObjectType);
        }

        [Test]
        public async Task No_Result_Will_Be_Set_When_No_PageDataContext_Available()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var contextRetriever = Substitute.For<IPageDataContextRetriever>();
            var urlRetriever = Substitute.For<IPageUrlRetriever>();
            var pageRetriever = Substitute.For<IPageRetriever>();
            var valuesRetriever = new PageNavigationRedirectsValuesRetriever(options);

            contextRetriever
                .TryRetrieve<TreeNode>(out Arg.Any<IPageDataContext<TreeNode>>())
                .Returns(false);

            var sut = new PageCustomDataRedirectResourceFilter(contextRetriever, urlRetriever, pageRetriever, valuesRetriever, options);

            var (executing, executed) = fixture.CreateFilterContexts();

            await sut.OnResourceExecutionAsync(executing, () => executed);

            executing.Result.Should().BeNull();
        }

        [Test]
        public async Task No_Result_Will_Be_Set_When_PageRedirectionType_Is_None()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var contextRetriever = Substitute.For<IPageDataContextRetriever>();
            var urlRetriever = Substitute.For<IPageUrlRetriever>();
            var pageRetriever = Substitute.For<IPageRetriever>();
            var valuesRetriever = new PageNavigationRedirectsValuesRetriever(options);

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.Value.RedirectionTypeFieldName, PageRedirectionType.None);
            });

            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(page);

            contextRetriever
                .TryRetrieve<TreeNode>(out Arg.Any<IPageDataContext<TreeNode>>())
                .Returns(x =>
                {
                    x[0] = dataContext;
                    return true;
                });

            var sut = new PageCustomDataRedirectResourceFilter(contextRetriever, urlRetriever, pageRetriever, valuesRetriever, options);

            var (executing, executed) = fixture.CreateFilterContexts();

            await sut.OnResourceExecutionAsync(executing, () => executed);

            executing.Result.Should().BeNull();
        }

        [Test]
        public async Task RedirectResult_Will_Be_Set_When_PageRedirectionType_Is_External()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();
            options.Value.UsePermanentRedirect = false;

            var contextRetriever = Substitute.For<IPageDataContextRetriever>();
            var urlRetriever = Substitute.For<IPageUrlRetriever>();
            var pageRetriever = Substitute.For<IPageRetriever>();
            var valuesRetriever = new PageNavigationRedirectsValuesRetriever(options);

            string redirectUrl = fixture.Create<string>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.Value.RedirectionTypeFieldName, PageRedirectionType.External);
                p.DocumentCustomData.SetValue(options.Value.ExternalRedirectURLFieldName, redirectUrl);
                p.DocumentCustomData.SetValue(options.Value.PageUsePermanentRedirectsFieldName, 1);
            });

            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(page);

            contextRetriever
                .TryRetrieve<TreeNode>(out Arg.Any<IPageDataContext<TreeNode>>())
                .Returns(x =>
                {
                    x[0] = dataContext;
                    return true;
                });

            var sut = new PageCustomDataRedirectResourceFilter(contextRetriever, urlRetriever, pageRetriever, valuesRetriever, options);

            var (executing, executed) = fixture.CreateFilterContexts();

            await sut.OnResourceExecutionAsync(executing, () => executed);

            executing.Result.Should().BeOfType<RedirectResult>();

            var result = (executing.Result as RedirectResult)!;

            result.Should().NotBeNull();
            result.Url.Should().Be(redirectUrl);
            result.Permanent.Should().Be(true);
        }

        [Test]
        public async Task RedirectResult_Will_Be_Set_When_PageRedirectionType_Is_Internal()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var contextRetriever = Substitute.For<IPageDataContextRetriever>();
            var urlRetriever = Substitute.For<IPageUrlRetriever>();
            var pageRetriever = Substitute.For<IPageRetriever>();
            var valuesRetriever = new PageNavigationRedirectsValuesRetriever(options);

            Guid nodeGUID = fixture.Create<Guid>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.Value.RedirectionTypeFieldName, PageRedirectionType.Internal);
                p.DocumentCustomData.SetValue(options.Value.InternalRedirectNodeGUIDFieldName, nodeGUID);
            });

            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(page);

            contextRetriever
                .TryRetrieve<TreeNode>(out Arg.Any<IPageDataContext<TreeNode>>())
                .Returns(x =>
                {
                    x[0] = dataContext;
                    return true;
                });

            var linkedPage = TreeNode.New<TreeNode>().With(p =>
            {

            });

            pageRetriever.RetrieveAsync<TreeNode>(
                Arg.Any<Action<DocumentQuery<TreeNode>>>(),
                Arg.Any<Action<IPageCacheBuilder<TreeNode>>>(),
                Arg.Any<CancellationToken>())
                .Returns(new[] { linkedPage });

            string redirectUrl = fixture.Create<string>();

            urlRetriever
                .Retrieve(linkedPage)
                .Returns(new PageUrl { RelativePath = redirectUrl });

            var sut = new PageCustomDataRedirectResourceFilter(contextRetriever, urlRetriever, pageRetriever, valuesRetriever, options);

            var (executing, executed) = fixture.CreateFilterContexts();

            await sut.OnResourceExecutionAsync(executing, () => executed);

            executing.Result.Should().BeOfType<RedirectResult>();

            var result = (executing.Result as RedirectResult)!;

            result.Should().NotBeNull();
            result.Url.Should().Be(redirectUrl);
            result.Permanent.Should().Be(options.Value.UsePermanentRedirect);
        }

        [Test]
        public async Task RedirectResult_Will_Be_Set_When_PageRedirectionType_Is_FirstChild()
        {
            var fixture = new Fixture();

            var options = fixture.CreateOptions();

            var contextRetriever = Substitute.For<IPageDataContextRetriever>();
            var urlRetriever = Substitute.For<IPageUrlRetriever>();
            var pageRetriever = Substitute.For<IPageRetriever>();
            var valuesRetriever = new PageNavigationRedirectsValuesRetriever(options);

            Guid nodeGUID = fixture.Create<Guid>();
            string className = fixture.Create<string>();

            var page = TreeNode.New<TreeNode>().With(p =>
            {
                p.DocumentCustomData.SetValue(options.Value.RedirectionTypeFieldName, PageRedirectionType.FirstChild);
                p.DocumentCustomData.SetValue(options.Value.InternalRedirectNodeGUIDFieldName, nodeGUID);
                p.DocumentCustomData.SetValue(options.Value.FirstChildClassNameFieldName, className);
            });

            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(page);

            contextRetriever
                .TryRetrieve<TreeNode>(out Arg.Any<IPageDataContext<TreeNode>>())
                .Returns(x =>
                {
                    x[0] = dataContext;
                    return true;
                });

            var linkedPage = TreeNode.New<TreeNode>().With(p =>
            {

            });

            pageRetriever.RetrieveAsync<TreeNode>(
                Arg.Any<Action<DocumentQuery<TreeNode>>>(),
                Arg.Any<Action<IPageCacheBuilder<TreeNode>>>(),
                Arg.Any<CancellationToken>())
                .Returns(new[] { linkedPage });

            string redirectUrl = fixture.Create<string>();

            urlRetriever
                .Retrieve(linkedPage)
                .Returns(new PageUrl { RelativePath = redirectUrl });

            var sut = new PageCustomDataRedirectResourceFilter(contextRetriever, urlRetriever, pageRetriever, valuesRetriever, options);

            var (executing, executed) = fixture.CreateFilterContexts();

            await sut.OnResourceExecutionAsync(executing, () => executed);

            executing.Result.Should().BeOfType<RedirectResult>();

            var result = (executing.Result as RedirectResult)!;

            result.Should().NotBeNull();
            result.Url.Should().Be(redirectUrl);
            result.Permanent.Should().Be(options.Value.UsePermanentRedirect);
        }
    }
}
