using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.ContentHelper
{
    [TestClass]
    public class ToValueTests
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
        public void ToValue_WhenByteArrayIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToValue((byte[]) null);
        }

        [TestMethod]
        public void ToValue_WhenByteArrayCanBeConvertedToUTF8_AssertCreateProtectorWasCalledOnDataProtectionProviderWithValueProtection()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildStringValueAsByteArray(Guid.NewGuid().ToString("D"));
            sut.ToValue(byteArray);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "ValueProtection") == 0)), Times.Exactly(2));
        }

        [TestMethod]
        public void ToValue_WhenByteArrayCanBeConvertedToUTF8_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            byte[] byteArray = BuildStringValueAsByteArray(Guid.NewGuid().ToString("D"));
            sut.ToValue(byteArray);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Encoding.UTF8.GetString(value), Encoding.UTF8.GetString(byteArray)) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToValue_WhenByteArrayCanBeConvertedToUTF8_ReturnsValue()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            byte[] byteArray = BuildStringValueAsByteArray(stringValue);
            string result = sut.ToValue(byteArray);

            Assert.IsNotNull(result);
            Assert.AreEqual(stringValue, result);
        }

        [TestMethod]
        public void ToValue_WhenByteArrayCanBeConvertedToUTF8AndCryptographicExceptionWasThrown_ReturnsNull()
        {
            CryptographicException exception = new CryptographicException();
            IContentHelper sut = CreateSut(exception);

            byte[] byteArray = BuildStringValueAsByteArray();
            string result = sut.ToValue(byteArray);

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedArgumentNullException("base64String")]
        public void ToValue_WhenBase64StringIsNull_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToValue((string) null);
        }

        [TestMethod]
        [ExpectedArgumentNullException("base64String")]
        public void ToValue_WhenBase64StringIsEmpty_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToValue(string.Empty);
        }

        [TestMethod]
        [ExpectedArgumentNullException("base64String")]
        public void ToValue_WhenBase64StringIsWhiteSpace_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToValue(" ");
        }

        [TestMethod]
        [ExpectedArgumentNullException("base64String")]
        public void ToValue_WhenBase64StringIsWhiteSpaces_ThrowsArgumentNullException()
        {
            IContentHelper sut = CreateSut();

            sut.ToValue("  ");
        }

        [TestMethod]
        public void ToValue_WhenBase64StringIsNotBase64String_AssertCreateProtectorNotWasCalledOnDataProtectionProviderWithValueProtection()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            sut.ToValue(base64String);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "ValueProtection") == 0)), Times.Never);
        }

        [TestMethod]
        public void ToValue_WhenBase64StringIsNotBase64String_AssertUnprotectWasNotCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            sut.ToValue(base64String);

            _dataProtectorMock.Verify(m => m.Unprotect(It.IsAny<byte[]>()), Times.Never);
        }

        [TestMethod]
        public void ToValue_WhenBase64StringIsNotBase64String_ReturnsNull()
        {
            IContentHelper sut = CreateSut();

            string base64String = Guid.NewGuid().ToString("D");
            string result = sut.ToValue(base64String);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToValue_WhenBase64StringCanBeConvertedToUTF8_AssertCreateProtectorWasCalledOnDataProtectionProviderWithValueProtection()
        {
            IContentHelper sut = CreateSut();

            string base64String = BuildStringValueAsBase64String(Guid.NewGuid().ToString("D"));
            sut.ToValue(base64String);

            _dataProtectionProviderMock.Verify(m => m.CreateProtector(It.Is<string>(value => string.CompareOrdinal(value, "ValueProtection") == 0)), Times.Exactly(2));
        }

        [TestMethod]
        public void ToValue_WhenBase64StringCanBeConvertedToUTF8_AssertUnprotectWasCalledOnDataProtector()
        {
            IContentHelper sut = CreateSut();

            string base64String = BuildStringValueAsBase64String(Guid.NewGuid().ToString("D"));
            sut.ToValue(base64String);

            _dataProtectorMock.Verify(m => m.Unprotect(It.Is<byte[]>(value => value != null && string.CompareOrdinal(Convert.ToBase64String(value), base64String) == 0)), Times.Once);
        }

        [TestMethod]
        public void ToValue_WhenBase64StringCanBeConvertedToUTF8_ReturnsValue()
        {
            IContentHelper sut = CreateSut();

            string stringValue = Guid.NewGuid().ToString("D");
            string base64String = BuildStringValueAsBase64String(stringValue);
            string result = sut.ToValue(base64String);

            Assert.IsNotNull(result);
            Assert.AreEqual(stringValue, result);
        }

        [TestMethod]
        public void ToValue_WhenBase64StringCanBeConvertedToUTF8AndCryptographicExceptionWasThrown_ReturnsNull()
        {
            CryptographicException exception = new CryptographicException();
            IContentHelper sut = CreateSut(exception);

            string base64String = BuildStringValueAsBase64String();
            string result = sut.ToValue(base64String);

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

        private byte[] BuildStringValueAsByteArray(string stringValue = null)
        {
            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object).ToByteArray(stringValue ?? Guid.NewGuid().ToString("D"));
        }

        private string BuildStringValueAsBase64String(string stringValue = null)
        {
            return new Web.Helpers.ContentHelper(_dataProtectionProviderMock.Object, _httpContextAccessorMock.Object, _urlHelperMock.Object).ToBase64String(stringValue ?? Guid.NewGuid().ToString("D"));
        }
    }
}