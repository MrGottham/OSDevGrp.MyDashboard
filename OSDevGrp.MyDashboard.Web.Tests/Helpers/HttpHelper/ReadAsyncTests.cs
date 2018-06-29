using System;
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
        public void ReadAsync_WhenUrlIsNull_ThrowsArgumentNullException()
        {
            IHttpHelper sut = CreateSut();

            sut.ReadAsync(null);
        }

        [TestMethod]
        public void ReadAsync_WhenCalled_ThrowsAggregateException()
        {
            Uri url = new Uri($"http://localhost/{Guid.NewGuid().ToString("D")}");

            IHttpHelper sut = CreateSut();

            try
            {
                Task<byte[]> readTask = sut.ReadAsync(url);
                readTask.Wait();

                Assert.Fail("An AggregateException was expected.");
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.Handle(ex => {
                    Assert.IsInstanceOfType(ex, typeof(Exception));
                    Assert.IsNotNull(ex.Message);
                    Assert.AreEqual($"Unable to perform the operation ({url.AbsoluteUri}): Not Found", ex.Message);
                    return true;
                });
            }
            catch
            {
                Assert.Fail("An AggregateException was expected.");
            }
        }

        private IHttpHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HttpHelper();
        }
    }
}