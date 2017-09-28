using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.NewsLogic
{
    [TestClass]
    public class GetNewsAsyncTests
    {
        #region Private variables

        private Mock<INewsRepository> _newRepositoryMock;
        private Mock<IExceptionHandler> _exceptionHandlerMock;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _newRepositoryMock = new Mock<INewsRepository>();
            _exceptionHandlerMock = new Mock<IExceptionHandler>();
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_ExpectNoError()
        {
            INewsLogic sut = CreateSut();

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_AssertGetNewsAsyncWasCalledOnNewRepository()
        {
            INewsLogic sut = CreateSut();

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            _newRepositoryMock.Verify(m => m.GetNewsAsync(), Times.Once);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsAsyncOnNewsRepositoryReturnsNull_ReturnsEmptyCollection()
        {
            INewsLogic sut = CreateSut(getNewsAsyncReturnsNull: true);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsAsyncOnNewsRepositoryReturnsEmptyCollection_ReturnsEmptyCollection()
        {
            IEnumerable<INews> news = BuildNews(numberOfNews: 0);
            INewsLogic sut = CreateSut(news: news);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsAsyncOnNewsRepositoryReturnsLessThanQueried_ReturnsCollectionWithAllElements()
        {
            IEnumerable<INews> news = BuildNews(numberOfNews: 24);
            INewsLogic sut = CreateSut(news: news);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(24, result.Count());
            Assert.IsTrue(result.All(item => news.Contains(item)));
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsAsyncOnNewsRepositoryReturnsEqualToQueried_ReturnsCollectionWithAllElements()
        {
            IEnumerable<INews> news = BuildNews(numberOfNews: 25);
            INewsLogic sut = CreateSut(news: news);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(25, result.Count());
            Assert.IsTrue(result.All(item => news.Contains(item)));
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndGetNewsAsyncOnNewsRepositoryReturnsMoreThanQueried_ReturnsCollectionWithQueriedElements()
        {
            IEnumerable<INews> news = BuildNews(numberOfNews: 26);
            INewsLogic sut = CreateSut(news: news);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(25, result.Count());
            Assert.IsTrue(result.All(item => news.Contains(item)));
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();
            INewsLogic sut = CreateSut(provokedException: exception);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)), Times.Once);
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndExceptionOccurs_ReturnsEmptyCollection()
        {
            Exception exception = new Exception();
            INewsLogic sut = CreateSut(provokedException: exception);

            Task<IEnumerable<INews>> getNewsTask = sut.GetNewsAsync(25);
            getNewsTask.Wait();

            IEnumerable<INews> result = getNewsTask.Result;
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        private INewsLogic CreateSut(IEnumerable<INews> news = null, bool getNewsAsyncReturnsNull = false, Exception provokedException = null)
        {
            if (provokedException == null)
            {
                _newRepositoryMock.Setup(m => m.GetNewsAsync())
                    .Returns(Task.Run<IEnumerable<INews>>(() => news ?? BuildNews(getNewsAsyncReturnsNull)));
            }
            else
            {
                _newRepositoryMock.Setup(m => m.GetNewsAsync())
                    .Throws(provokedException);
            }
            
            _exceptionHandlerMock.Setup(m => m.HandleAsync(It.IsAny<Exception>()))
                .Returns(Task.Run(() => { }));

            return new OSDevGrp.MyDashboard.Core.Logic.NewsLogic(
                _newRepositoryMock.Object,
                _exceptionHandlerMock.Object);
        }

        private IEnumerable<INews> BuildNews(bool shouldReturnNull = false, int numberOfNews = 7)
        {
            if (shouldReturnNull)
            {
                return null;
            }

            IList<INews> news = new List<INews>(numberOfNews);
            while (news.Count < numberOfNews)
            {
                Mock<INews> newsMock = new Mock<INews>();
                newsMock.Setup(m => m.Timestamp)
                    .Returns(DateTime.Now);
                news.Add(newsMock.Object);
            }
            return news;
        }
    }
}