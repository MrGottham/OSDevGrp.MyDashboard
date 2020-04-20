using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.HttpHelper
{
    [TestClass]
    public class ReadAsyncTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("url")]
        public async Task ReadAsync_WhenUrlIsNull_ThrowsArgumentNullException()
        {
            IHttpHelper sut = CreateSut();

            await sut.ReadAsync(null);
        }

        [TestMethod]
        public async Task ReadAsync_WhenCalled_ThrowsHttpRequestException()
        {
            Uri url = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IHttpHelper sut = CreateSut();

            try
            {
                await sut.ReadAsync(url);

                Assert.Fail("An HttpRequestException was expected.");
            }
            catch (HttpRequestException)
            {
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex.Message);
                Assert.AreEqual($"Unable to perform the operation ({url.AbsoluteUri}): Not Found", ex.Message);
            }
        }

        private IHttpHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HttpHelper();
        }
    }
}