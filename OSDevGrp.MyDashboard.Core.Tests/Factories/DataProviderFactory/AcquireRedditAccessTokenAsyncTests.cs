using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Factories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Factories.DataProviderFactory
{
    [TestClass]
    public class AcquireRedditAccessTokenAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsNull_ThrowsArgumentNullException()
        {
            const string clientId = null;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsEmpty_ThrowsArgumentNullException()
        {
            string clientId = string.Empty;
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsWhitespace_ThrowsArgumentNullException()
        {
            const string clientId = " ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        [TestMethod]
        [ExpectedArgumentNullException("clientId")]
        public void AcquireRedditAccessTokenAsync_WhenClientIdIsWhitespaces_ThrowsArgumentNullException()
        {
            const string clientId = "  ";
            string state = Guid.NewGuid().ToString("D");
            Uri redirectUri = new Uri($"http://localhost/{state}");

            IDataProviderFactory sut = CreateSut();

            sut.AcquireRedditAccessTokenAsync(clientId, state, redirectUri);
        }

        private IDataProviderFactory CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Factories.DataProviderFactory();
        }
    }
}