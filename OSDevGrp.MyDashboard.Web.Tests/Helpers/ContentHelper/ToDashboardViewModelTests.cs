using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public class ToDashboardViewModelTests
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

            _dataProtectionProviderMock.Setup(m => m.CreateProtector(It.IsAny<string>()))
                .Returns(_dataProtectorMock.Object);
            _dataProtectorMock.Setup(m => m.Protect(It.IsAny<byte[]>()))
                .Returns<byte[]>(value => value);
        }

        [TestMethod]
        [ExpectedArgumentNullException("byteArray")]
        public void ToDashboardViewModel_WhenByteArrayIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToDashboardViewModel(null);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCannotBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithDashboardProtection()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            sut.ToDashboardViewModel(byteArray);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "DashboardProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCannotBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            sut.ToDashboardViewModel(byteArray);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCannotBeDeserialized_ReturnsNull()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithDashboardProtection()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            sut.ToDashboardViewModel(byteArray);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "DashboardProtection") == 0)), Times.Exactly(2));
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            sut.ToDashboardViewModel(byteArray);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedInformations()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Informations);
            Assert.AreEqual(1, result.Informations.Count);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedLatestInformationsWithImage()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.LatestInformationsWithImage);
            Assert.AreEqual(1, result.LatestInformationsWithImage.Count);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedSystemErrors()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SystemErrors);
            Assert.AreEqual(1, result.SystemErrors.Count);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedSettings()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Settings);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedRedditAuthenticatedUser()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RedditAuthenticatedUser);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardViewModelWithDeserializedRedditSubreddits()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RedditSubreddits);
            Assert.AreEqual(1, result.RedditSubreddits.Count);
        }

        [TestMethod]
        public void ToDashboardViewModel_WhenByteArrayCanBeDeserializedAndCryptographicExceptionWasThrown_ReturnsNull()
        {
            CryptographicException exception = new CryptographicException();
            IContentHelper sut = CreateSut(exception);

            byte[] byteArray = BuildDashboardViewModelAsByteArray();
            DashboardViewModel result = sut.ToDashboardViewModel(byteArray);

            Assert.IsNull(result);
        }

        private IContentHelper CreateSut(CryptographicException cryptographicException = null)
        {
            if (cryptographicException == null)
            {
                _dataProtectorMock.Setup(m => m.Unprotect(It.IsAny<byte[]>()))
                    .Returns<byte[]>(value => value);
            }
            else
            {
                _dataProtectorMock.Setup(m => m.Unprotect(It.IsAny<byte[]>()))
                    .Throws(cryptographicException);
            }

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

        private byte[] BuildDashboardViewModelAsByteArray()
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

            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                Informations = new List<InformationViewModel> {informationViewModel},
                LatestInformationsWithImage = new List<ImageViewModel<InformationViewModel>> {imageViewModel},
                SystemErrors = new List<SystemErrorViewModel> {systemErrorViewModel},
                Settings = BuildDashboardSettingsViewModel(),
                RedditAuthenticatedUser = redditAuthenticatedUserObjectViewModel,
                RedditSubreddits = new List<ObjectViewModel<IRedditSubreddit>> {redditSubredditObjectViewModel}
            };

            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object).ToByteArray(dashboardViewModel);
        }
    }
}