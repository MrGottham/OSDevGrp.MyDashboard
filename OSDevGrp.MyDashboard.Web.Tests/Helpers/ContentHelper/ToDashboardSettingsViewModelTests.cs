using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.ContentHelper
{
    [TestClass]
    public class ToDashboardSettingsViewModelTests
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
        public void ToDashboardSettingsViewModel_WhenByteArrayIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToDashboardSettingsViewModel((byte[]) null));

            Assert.AreEqual("byteArray", result.ParamName);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCannotBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            sut.ToDashboardSettingsViewModel(byteArray);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCannotBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            sut.ToDashboardSettingsViewModel(byteArray);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCannotBeDeserialized_ReturnsNull()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D"));
            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray();
            sut.ToDashboardSettingsViewModel(byteArray);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Exactly(2));
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray();
            sut.ToDashboardSettingsViewModel(byteArray);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && value.Length > 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedNumberOfNews()
        {
            IContentHelper sut = CreateSut();

            int numberOfNews = _random.Next(25, 50);
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(numberOfNews);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedUseReddit()
        {
            IContentHelper sut = CreateSut();

            bool useReddit = _random.Next(100) > 0;
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(useReddit: useReddit);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(useReddit, result.UseReddit);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedAllowNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool allowNsfwContent = _random.Next(100) > 0;
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(allowNsfwContent: allowNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(allowNsfwContent, result.AllowNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedIncludeNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool includeNsfwContent = _random.Next(100) > 0;
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(includeNsfwContent: includeNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(includeNsfwContent, result.IncludeNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedOnlyNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool onlyNsfwContent = _random.Next(100) > 0;
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(onlyNsfwContent: onlyNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(onlyNsfwContent, result.OnlyNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedRedditAccessToken()
        {
            IContentHelper sut = CreateSut();

            string redditAccessToken = Guid.NewGuid().ToString("D");
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(redditAccessToken: redditAccessToken);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(redditAccessToken, result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedExportData()
        {
            IContentHelper sut = CreateSut();

            bool exportData = _random.Next(100) > 0;
            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray(exportData: exportData);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(exportData, result.ExportData);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenByteArrayCanBeDeserializedAndCryptographicExceptionWasThrown_ReturnsNull()
        {
            CryptographicException exception = new CryptographicException();
            IContentHelper sut = CreateSut(exception);

            byte[] byteArray = BuildDashboardSettingsViewModelAsByteArray();
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(byteArray);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToDashboardSettingsViewModel((string) null));

            Assert.AreEqual("base64String", result.ParamName);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToDashboardSettingsViewModel(string.Empty));

            Assert.AreEqual("base64String", result.ParamName);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToDashboardSettingsViewModel(" "));

            Assert.AreEqual("base64String", result.ParamName);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.ToDashboardSettingsViewModel("  "));

            Assert.AreEqual("base64String", result.ParamName);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsNotBase64String_AssertCreateProtectorNotWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsNotBase64String_AssertUnprotectWasNotCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectorMock.Verify(m => m.Unprotect(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringIsNotBase64String_ReturnsNull()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCannotBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCannotBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Convert.ToBase64String(value), base64String) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCannotBeDeserialized_ReturnsNull()
        {
            IContentHelper sut = CreateSut();

            string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("D")));
            DashboardSettingsViewModel result = sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_AssertCreateProtectorWasCalledOnDataProtectionProviderWithSettingsProtection()
        {
            IContentHelper sut = CreateSut();

            string base64String = BuildDashboardSettingsViewModelAsBase64String();
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "SettingsProtection") == 0)), Times.Exactly(2));
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string base64String = BuildDashboardSettingsViewModelAsBase64String();
            sut.ToDashboardSettingsViewModel(base64String);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Convert.ToBase64String(value), base64String) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedNumberOfNews()
        {
            IContentHelper sut = CreateSut();

            int numberOfNews = _random.Next(25, 50);
            string base64String = BuildDashboardSettingsViewModelAsBase64String(numberOfNews);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(numberOfNews, result.NumberOfNews);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedUseReddit()
        {
            IContentHelper sut = CreateSut();

            bool useReddit = _random.Next(100) > 0;
            string base64String = BuildDashboardSettingsViewModelAsBase64String(useReddit: useReddit);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(useReddit, result.UseReddit);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedAllowNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool allowNsfwContent = _random.Next(100) > 0;
            string base64String = BuildDashboardSettingsViewModelAsBase64String(allowNsfwContent: allowNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(allowNsfwContent, result.AllowNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedIncludeNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool includeNsfwContent = _random.Next(100) > 0;
            string base64String = BuildDashboardSettingsViewModelAsBase64String(includeNsfwContent: includeNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(includeNsfwContent, result.IncludeNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedOnlyNsfwContent()
        {
            IContentHelper sut = CreateSut();

            bool onlyNsfwContent = _random.Next(100) > 0;
            string base64String = BuildDashboardSettingsViewModelAsBase64String(onlyNsfwContent: onlyNsfwContent);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(onlyNsfwContent, result.OnlyNsfwContent);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedRedditAccessToken()
        {
            IContentHelper sut = CreateSut();

            string redditAccessToken = Guid.NewGuid().ToString("D");
            string base64String = BuildDashboardSettingsViewModelAsBase64String(redditAccessToken: redditAccessToken);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(redditAccessToken, result.RedditAccessToken);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserialized_ReturnsDashboardSettingsViewModelWithDeserializedExportData()
        {
            IContentHelper sut = CreateSut();

            bool exportData = _random.Next(100) > 0;
            string base64String = BuildDashboardSettingsViewModelAsBase64String(exportData: exportData);
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(exportData, result.ExportData);
        }

        [TestMethod]
        public void ToDashboardSettingsViewModel_WhenBase64StringCanBeDeserializedAndCryptographicExceptionWasThrown_ReturnsNull()
        {
            CryptographicException exception = new CryptographicException();
            IContentHelper sut = CreateSut(exception);

            string base64String = BuildDashboardSettingsViewModelAsBase64String();
            DashboardSettingsViewModel result =  sut.ToDashboardSettingsViewModel(base64String);

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

        private DashboardSettingsViewModel BuildDashboardSettingsViewModel(int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null, bool? exportData = null)
        {
            return new DashboardSettingsViewModel
            {
                NumberOfNews = numberOfNews ?? _random.Next(25, 50),
                UseReddit = useReddit ?? _random.Next(100) > 50,
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent,
                RedditAccessToken = redditAccessToken,
                ExportData = exportData ?? _random.Next(100) > 50
            };
        }

        private byte[] BuildDashboardSettingsViewModelAsByteArray(int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null, bool? exportData = null)
        {
            DashboardSettingsViewModel dashboard = BuildDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken, exportData);

            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object).ToByteArray(dashboard);
        }

        private string BuildDashboardSettingsViewModelAsBase64String(int? numberOfNews = null, bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null, bool? exportData = null)
        {
            DashboardSettingsViewModel dashboard = BuildDashboardSettingsViewModel(numberOfNews, useReddit, allowNsfwContent, includeNsfwContent, onlyNsfwContent, redditAccessToken, exportData);

            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object).ToBase64String(dashboard);
        }
    }
}