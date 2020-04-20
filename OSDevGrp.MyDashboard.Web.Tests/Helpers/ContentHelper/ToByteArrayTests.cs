using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

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
        [ExpectedArgumentNullException("dashboardSettingsViewModel")]
        public void ToByteArray_WhenDashboardSettingsViewModelIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray((DashboardSettingsViewModel) null);
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
        [ExpectedArgumentNullException("dashboardViewModel")]
        public void ToByteArray_WhenDashboardViewModelIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray((DashboardViewModel) null);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardViewModel_AssertCreateProtectorWasCalledOnDataProtectionProviderWithDashboardProtection()
        {
            IContentHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToByteArray(dashboardViewModel);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "DashboardProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardViewModel_AssertProtectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            sut.ToByteArray(dashboardViewModel);

            _dataProtectorMock.Verify(m => m.Protect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToByteArray_WhenCalledWithDashboardViewModel_ReturnsByteArray()
        {
            IContentHelper sut = CreateSut();

            DashboardViewModel dashboardViewModel = BuildDashboardViewModel();
            byte[] result = sut.ToByteArray(dashboardViewModel);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToByteArray_WhenStringValueIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray((string) null);
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToByteArray_WhenStringValueIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray(string.Empty);
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToByteArray_WhenStringValueIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray(" ");
        }

        [TestMethod]
        [ExpectedArgumentNullException("value")]
        public void ToByteArray_WhenStringValueIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToByteArray("  ");
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

        private DashboardViewModel BuildDashboardViewModel()
        {
            InformationViewModel informationViewModel = new InformationViewModel
            {
                InformationIdentifier = Guid.NewGuid().ToString("D"),
                Timestamp = DateTime.Now,
                Header = Guid.NewGuid().ToString("D"),
                Summary = Guid.NewGuid().ToString("D"),
                Details = Guid.NewGuid().ToString("D"),
                ImageUrl = Guid.NewGuid().ToString("D"),
                Provider = Guid.NewGuid().ToString("D"),
                Author = Guid.NewGuid().ToString("D"),
                ExternalUrl = Guid.NewGuid().ToString("D")
            };
            ImageViewModel<InformationViewModel> imageViewModel = new ImageViewModel<InformationViewModel>(informationViewModel, Convert.FromBase64String("R0lGODlhDgAOAKIAAAAAAP///wAAgP//AP8AAMDAwICAgP///yH5BAEAAAcALAAAAAAOAA4AAAM+aLq8YCPIOGV5YdV5IRFgCAbdFlFDNygkWRQGiXHZkj5rXKDWHBUu2A3C2jFkMRolMDyReMiXdCoFWK/YbAIAOw=="));

            SystemErrorViewModel systemErrorViewModel = new SystemErrorViewModel
            {
                SystemErrorIdentifier = Guid.NewGuid().ToString("D"),
                Timestamp = DateTime.Now,
                Message = Guid.NewGuid().ToString("D"),
                Details = Guid.NewGuid().ToString("D"),
            };

            ObjectViewModel<IRedditAuthenticatedUser> redditAuthenticatedUserObjectViewModel = new ObjectViewModel<IRedditAuthenticatedUser>
            {
                ObjectIdentifier = Guid.NewGuid().ToString("D"),
                Object = new RedditAuthenticatedUser(),
                Timestamp = DateTime.Now,
                Html = Guid.NewGuid().ToString("D")
            };

            ObjectViewModel<IRedditSubreddit> redditSubredditObjectViewModel = new ObjectViewModel<IRedditSubreddit>
            {
                ObjectIdentifier = Guid.NewGuid().ToString("D"),
                Object = new RedditSubreddit(),
                Timestamp = DateTime.Now,
                Html = Guid.NewGuid().ToString("D")
            };

            return new DashboardViewModel
            {
                Informations = new List<InformationViewModel> {informationViewModel},
                LatestInformationsWithImage = new List<ImageViewModel<InformationViewModel>> {imageViewModel},
                SystemErrors = new List<SystemErrorViewModel> {systemErrorViewModel},
                Settings = BuildDashboardSettingsViewModel(),
                RedditAuthenticatedUser = redditAuthenticatedUserObjectViewModel,
                RedditSubreddits = new List<ObjectViewModel<IRedditSubreddit>> {redditSubredditObjectViewModel}
            };
        }
    }
}