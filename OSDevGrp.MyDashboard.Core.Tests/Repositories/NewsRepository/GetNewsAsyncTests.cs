using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Repositories.ExceptionRepository
{
    [TestClass]
    public class GetNewsAsyncTests
    {
        #region Private variables

        private Mock<IDataProviderFactory> _dataProviderFactoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _dataProviderFactoryMock = new Mock<IDataProviderFactory>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_ExpectNoError()
        {
            INewsRepository sut = CreateSut();
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<AggregateException>()), Times.Never);
            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_ReturnsNonEmptyCollection()
        {
            INewsRepository sut = CreateSut();
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_AssertBuildNewsProvidersAsyncWasCalledOnDataProviderFactory()
        {
            INewsRepository sut = CreateSut();
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _dataProviderFactoryMock.Verify(m => m.BuildNewsProvidersAsync(), Times.Once);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsProvidersReturnsNull_ReturnEmptyCollection()
        {
            INewsRepository sut = CreateSut(newsProvidersIsNull: true);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsProvidersReturnsEmptyCollection_ReturnEmptyCollection()
        {
            INewsRepository sut = CreateSut(hasNewsProviders: false);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            AggregateException aggregateException = new AggregateException();

            INewsRepository sut = CreateSut(provokeException: aggregateException);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(ex => ex == aggregateException)), Times.Once);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndAggregateExceptionOccurs_ReturnEmptyCollection()
        {
            AggregateException aggregateException = new AggregateException();

            INewsRepository sut = CreateSut(provokeException: aggregateException);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();

            INewsRepository sut = CreateSut(provokeException: exception);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndExceptionOccurs_ReturnEmptyCollection()
        {
            Exception exception = new Exception();

            INewsRepository sut = CreateSut(provokeException: exception);
            
            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        private INewsRepository CreateSut(bool newsProvidersIsNull = false, bool hasNewsProviders = true, Exception provokeException = null)
        {
            IList<INewsProvider> newsProviders = null;
            if (newsProvidersIsNull == false)
            {
                newsProviders = new List<INewsProvider>();
                if (hasNewsProviders)
                {
                    newsProviders.Add(BuildNewsProvider("DR", "http://www.dr.dk/nyheder/service/feeds/allenyheder"));
                }
            }

            if (provokeException != null)
            {
                _dataProviderFactoryMock.Setup(m => m.BuildNewsProvidersAsync())
                    .Throws(provokeException);
            }
            else
            {
                _dataProviderFactoryMock.Setup(m => m.BuildNewsProvidersAsync())
                    .Returns(Task.Run<IEnumerable<INewsProvider>>(() => newsProviders));
            }

            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<AggregateException>()))
                .Returns(Task.Run(() => { }));
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Repositories.NewsRepository(
                _dataProviderFactoryMock.Object,
                _exceptionHandlerMock.Object);
        }

        private INewsProvider BuildNewsProvider(string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Mock<INewsProvider> newsProviderMock = new Mock<INewsProvider>();
            newsProviderMock.Setup(m => m.Name)
                .Returns(name);
            newsProviderMock.Setup(m => m.Uri)
                .Returns(new Uri(url));
            return newsProviderMock.Object;
        }
    }
}
