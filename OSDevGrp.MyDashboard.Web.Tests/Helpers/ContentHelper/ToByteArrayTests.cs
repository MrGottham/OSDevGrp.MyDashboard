using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Text;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.ContentHelper
{
    [TestClass]
    public class ToByteArrayTests
    {
        #region Private variables

        private Mock<IDataProtectionProvider> _dataProtectionProviderMock;
        private Mock<IDataProtector> _dataProtectorMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private Random _random;

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
        public void ToByteArray_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToByteArray((DashboardSettingsViewModel) null));

            Assert.AreEqual("dashboardSettingsViewModel", result.ParamName);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardSettingsViewModel_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            sut.ToByteArray(dashboardSettingsViewModel);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardSettingsViewModel_AssertProtectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            sut.ToByteArray(dashboardSettingsViewModel);

            _dataProtectorMock.Verify(m => m.Protect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardSettingsViewModel_ReturnsByteArray()
        {
            IContentHelper sut = CreateSut();

            DashboardSettingsViewModel dashboardSettingsViewModel = BuildDashboardSettingsViewModel();
            byte[] result = sut.ToByteArray(dashboardSettingsViewModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void ToByteArray_WhenStringValueIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToByteArray((string) null));

            Assert.AreEqual("value", result.ParamName);
        }

        [TestMethod]
        public void ToByteArray_WhenStringValueIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToByteArray(string.Empty));

            Assert.AreEqual("value", result.ParamName);
        }

        [TestMethod]
        public void ToByteArray_WhenStringValueIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToByteArray(" "));

            Assert.AreEqual("value", result.ParamName);
        }

        [TestMethod]
        public void ToByteArray_WhenStringValueIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToByteArray("  "));

            Assert.AreEqual("value", result.ParamName);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithStringValue_AssertCreateProtectorWasCalledOnDataProtectionProviderWithValueProtection()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            sut.ToByteArray(stringValue);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "ValueProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithStringValue_AssertProtectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            sut.ToByteArray(stringValue);

            _dataProtectorMock.Verify(m => m.Protect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Encoding.UTF8.GetString(value), stringValue) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithStringValue_ReturnsByteArray()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            byte[] result = sut.ToByteArray(stringValue);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
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