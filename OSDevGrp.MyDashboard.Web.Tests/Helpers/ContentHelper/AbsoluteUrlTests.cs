using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.ContentHelper
{
    [TestClass]
    public class AbsoluteUrlTests
    {
        #region Private variables

        private Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private Mock<IDataProtector> _dataProtectorMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IUrlHelper> _urlHelperMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>(0);
            _dataProtectorMock = new Mock<IDataProtector>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _urlHelperMock = new Mock<IUrlHelper>();
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndActionIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(null, Guid.NewGuid().ToString("D"));
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndActionIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(string.Empty, Guid.NewGuid().ToString("D"));
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndActionIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(" ", Guid.NewGuid().ToString("D"));
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndActionIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl("  ", Guid.NewGuid().ToString("D"));
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndControllerIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), null);
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndControllerIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), string.Empty);
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndControllerIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), " ");
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndControllerIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), "  ");
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValues_AssertHttpContextWacCalledOnHttpContextAccessor()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpContextWasNotReturend_AssertActionWasNotCalledOnUrlHelper()
        {
            IContentHelper sut = CreateSut(false);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            _urlHelperMock.Verify(m => m.Action(It.IsAny<UrlActionContext>()), Times.Never);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpContextWasNotReturend_ReturnsNull()
        {
            IContentHelper sut = CreateSut(false);

            string result = sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpContextWasReturend_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            IContentHelper sut = CreateSut(httpContext: httpContextMock.Object);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasNotReturend_AssertActionWasNotCalledOnUrlHelper()
        {
            HttpContext httpContext = BuildHttpContext(false);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            _urlHelperMock.Verify(m => m.Action(It.IsAny<UrlActionContext>()), Times.Never);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasNotReturend_ReturnsNull()
        {
            HttpContext httpContext = BuildHttpContext(false);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            string result = sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasReturend_AssertSchemeWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            httpRequestMock.Verify(m => m.Scheme, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasReturend_AssertIsHttpsWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            httpRequestMock.Verify(m => m.IsHttps, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasReturend_AssertHostWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            httpRequestMock.Verify(m => m.Host, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasReturend_AssertPathBaseWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"));

            httpRequestMock.Verify(m => m.PathBase, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutValuesAndHttpRequestWasReturend_AssertActionWasCalledOnUrlHelper()
        {
            IContentHelper sut = CreateSut();

            string action = Guid.NewGuid().ToString("D");
            string controller = Guid.NewGuid().ToString("D");
            sut.AbsoluteUrl(action, controller);

            _urlHelperMock.Verify(m => m.Action(It.Is<UrlActionContext>(value => value != null && string.CompareOrdinal(value.Controller, controller) == 0 && string.CompareOrdinal(value.Action, action) == 0 && value.Values == null)), Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithoutAndHttpRequestWasReturend_ReturnsAbsoluteUrl()
        {
            const string scheme = "http";
            const string host = "localhost";
            string pathBase = $"/{Guid.NewGuid().ToString("D")}";
            HttpRequest httpRequest = BuildHttpRequest(scheme, host, pathBase);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            string action = Guid.NewGuid().ToString("D");
            string controller = Guid.NewGuid().ToString("D");
            string result = sut.AbsoluteUrl(action, controller);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, $"{scheme}://{host}{pathBase}/{controller}/{action}");
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithValuesAndActionIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(null, Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithValuesAndActionIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(string.Empty, Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithValuesAndActionIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(" ", Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("action")]
        public void AbsoluteUrl_WhenCalledWithValuesAndActionIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl("  ", Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithValuesAndControllerIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), null, new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithValuesAndControllerIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), string.Empty, new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithValuesAndControllerIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), " ", new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("controller")]
        public void AbsoluteUrl_WhenCalledWithValuesAndControllerIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), "  ", new {id = Guid.NewGuid()});
        }

        [TestMethod]
        [ExpectedArgumentNullException("values")]
        public void AbsoluteUrl_WhenCalledWithValuesAndValuesIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), null);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValues_AssertHttpContextWacCalledOnHttpContextAccessor()
        {
            IContentHelper sut = CreateSut();

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            _httpContextAccessorMock.Verify(m => m.HttpContext, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpContextWasNotReturend_AssertActionWasNotCalledOnUrlHelper()
        {
            IContentHelper sut = CreateSut(false);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            _urlHelperMock.Verify(m => m.Action(It.IsAny<UrlActionContext>()), Times.Never);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpContextWasNotReturend_ReturnsNull()
        {
            IContentHelper sut = CreateSut(false);

            string result = sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpContextWasReturend_AssertRequestWasCalledOnReturnedHttpContext()
        {
            Mock<HttpContext> httpContextMock = BuildHttpContextMock();
            IContentHelper sut = CreateSut(httpContext: httpContextMock.Object);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            httpContextMock.Verify(m => m.Request, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasNotReturend_AssertActionWasNotCalledOnUrlHelper()
        {
            HttpContext httpContext = BuildHttpContext(false);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            _urlHelperMock.Verify(m => m.Action(It.IsAny<UrlActionContext>()), Times.Never);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasNotReturend_ReturnsNull()
        {
            HttpContext httpContext = BuildHttpContext(false);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            string result = sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_AssertSchemeWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            httpRequestMock.Verify(m => m.Scheme, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_AssertIsHttpsWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            httpRequestMock.Verify(m => m.IsHttps, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_AssertHostWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            httpRequestMock.Verify(m => m.Host, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_AssertPathBaseWasCalledOnReturnedHttpRequest()
        {
            Mock<HttpRequest> httpRequestMock = BuildHttpRequestMock();
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequestMock.Object);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            sut.AbsoluteUrl(Guid.NewGuid().ToString("D"), Guid.NewGuid().ToString("D"), new {id = Guid.NewGuid()});

            httpRequestMock.Verify(m => m.PathBase, Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_AssertActionWasCalledOnUrlHelper()
        {
            IContentHelper sut = CreateSut();

            string action = Guid.NewGuid().ToString("D");
            string controller = Guid.NewGuid().ToString("D");
            object values = new {id = Guid.NewGuid()};
            sut.AbsoluteUrl(action, controller, values);

            _urlHelperMock.Verify(m => m.Action(It.Is<UrlActionContext>(value => value != null && string.CompareOrdinal(value.Controller, controller) == 0 && string.CompareOrdinal(value.Action, action) == 0 && value.Values == values)), Times.Once);
        }

        [TestMethod]
        public void AbsoluteUrl_WhenCalledWithValuesAndHttpRequestWasReturend_ReturnsAbsoluteUrl()
        {
            const string scheme = "http";
            const string host = "localhost";
            string pathBase = $"/{Guid.NewGuid().ToString("D")}";
            HttpRequest httpRequest = BuildHttpRequest(scheme, host, pathBase);
            HttpContext httpContext = BuildHttpContext(httpRequest: httpRequest);
            IContentHelper sut = CreateSut(httpContext: httpContext);

            string action = Guid.NewGuid().ToString("D");
            string controller = Guid.NewGuid().ToString("D");
            object values = new {id = Guid.NewGuid()};
            string result = sut.AbsoluteUrl(action, controller, values);

            Assert.IsNotNull(result);
            Assert.AreEqual(result, $"{scheme}://{host}{pathBase}/{controller}/{action}");
        }

        private IContentHelper CreateSut(bool hasHttpContext = true, HttpContext httpContext = null)
        {
            _httpContextAccessorMock.Setup(m => m.HttpContext)
                .Returns(hasHttpContext ? httpContext ?? BuildHttpContext() : null);
            _urlHelperMock.Setup(m => m.Action(It.IsAny<UrlActionContext>()))
                .Returns<UrlActionContext>(context => $"~/{context.Controller}/{context.Action}");

            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object);
        }

        public HttpContext BuildHttpContext(bool hasHttpRequest = true, HttpRequest httpRequest = null)
        {
            return BuildHttpContextMock(hasHttpRequest, httpRequest).Object;
        }

        private Mock<HttpContext> BuildHttpContextMock(bool hasHttpRequest = true, HttpRequest httpRequest = null)
        {
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(m => m.Request)
                .Returns(hasHttpRequest == false ? (HttpRequest) null : httpRequest ?? BuildHttpRequest());
            return httpContextMock;
        }

        private HttpRequest BuildHttpRequest(string scheme = null, string host = null, string pathBase = null)
        {
            return BuildHttpRequestMock(scheme, host, pathBase).Object;
        }

        private Mock<HttpRequest> BuildHttpRequestMock(string scheme = null, string host = null, string pathBase = null)
        {
            Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(m => m.Scheme)
                .Returns(scheme ?? "http");
            httpRequestMock.Setup(m => m.IsHttps)
                .Returns(false);
            httpRequestMock.Setup(m => m.Host)
                .Returns(new HostString(host ?? "localhost"));
            httpRequestMock.Setup(m => m.PathBase)
                .Returns(new PathString(pathBase ?? $"/{Guid.NewGuid().ToString("D")}"));
            return httpRequestMock;
        }
    }
}