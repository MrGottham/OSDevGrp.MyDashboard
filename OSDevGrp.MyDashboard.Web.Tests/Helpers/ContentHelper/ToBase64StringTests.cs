using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.ContentHelper
{
    [TestClass]
    public class ToBase64StringTests
    {
        #region Private variables

        private Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private Mock<IDataProtector> _dataProtectorMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private Random _random;
        private readonly Regex _base64StringRegex = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(32));

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dataProtectionProviderMock = new Mock<IDataProtectionProvider>(0);
            _dataProtectorMock = new Mock<IDataProtector>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public void ToBase64String_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToBase64String((DashboardSettingsViewModel) null);
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithDashboardSettingsViewModel_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            sut.ToBase64String(dashboardSettingsViewModel);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithDashboardSettingsViewModel_AssertProtectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            sut.ToBase64String(dashboardSettingsViewModel);

            _dataProtectorMock.Verify(m => m.Protect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithDashboardSettingsViewModel_ReturnsBase64String()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            string result = sut.ToBase64String(dashboardSettingsViewModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.IsTrue(result.Trim().Length % 4 == 0 && _base64StringRegex.IsMatch(result.Trim()));
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToBase64String_WhenStringValueIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToBase64String((string) null);
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToBase64String_WhenStringValueIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToBase64String(string.Empty);
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToBase64String_WhenStringValueIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToBase64String(" ");
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToBase64String_WhenStringValueIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToBase64String("  ");
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithStringValue_AssertCreateProtectorWasCalledOnDataProtectionProviderWithValueProtection()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            sut.ToBase64String(stringValue);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "ValueProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithStringValue_AssertProtectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            sut.ToBase64String(stringValue);

            _dataProtectorMock.Verify(m => m.Protect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Encoding.UTF8.GetString(value), stringValue) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToBase64String_WhenCalledWithStringValue_ReturnsBase64String()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            string result = sut.ToBase64String(stringValue);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.IsTrue(result.Trim().Length % 4 == 0 && _base64StringRegex.IsMatch(result.Trim()));
        }

        private IContentHelper CreateSut()
        {
            _dataProtectionProviderMock.Setup(m => m.CreateProtector(It.IsAny<string>()))
                .Returns(_dataProtectorMock.Object);
            _dataProtectorMock.Setup(m => m.Protect(It.IsAny<byte[]>()))
                .Returns<byte[]>(value => value);

            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object);
        }

        private DashboardSettingsViewModel BuildDashboardSettingsViewModel()
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = _random.Next(25, 50),
                UseReddit = _random.Next(100) > 50,
                AllowNsfwContent = _random.Next(100) > 50,
                IncludeNsfwContent = _random.Next(100) > 50,
                OnlyNsfwContent = _random.Next(100) > 50,
                RedditAccessToken = Guid.NewGuid().ToString("D"),
                ExportData = _random.Next(100) > 50
            };
        }
    }
}