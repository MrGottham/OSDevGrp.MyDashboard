using System;
using System.Collections.Generic;
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
            
            Task getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalled_AssertGetNewsProvidersAsyncWasCalledOnDataProviderFactory()
        {
            INewsRepository sut = CreateSut();
            
            Task getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _dataProviderFactoryMock.Verify(m => m.GetNewsProvidersAsync());
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndAggregateExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            AggregateException aggregateException = new AggregateException();

            INewsRepository sut = CreateSut(aggregateException);
            
            Task getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<AggregateException>(ex => ex == aggregateException)));
        }

        [TestMethod]
        public void GetNewsAsync_WhenCalledAndExceptionOccurs_AssertHandleAsyncWasCalledOnExceptionHandler()
        {
            Exception exception = new Exception();

            INewsRepository sut = CreateSut(exception);
            
            Task getNewsTask = sut.GetNewsAsync();
            getNewsTask.Wait();

            _exceptionHandlerMock.Verify(m => m.HandleAsync(It.Is<Exception>(ex => ex == exception)));
        }

        private INewsRepository CreateSut(Exception provokeException = null)
        {
            INewsProvider newsProvider = BuildNewsProvider("DR", "http://www.dr.dk/nyheder/service/feeds/allenyheder");

            if (provokeException != null)
            {
                _dataProviderFactoryMock.Setup(m => m.GetNewsProvidersAsync())
                    .Throws(provokeException);
            }
            else
            {
                _dataProviderFactoryMock.Setup(m => m.GetNewsProvidersAsync())
                    .Returns(Task.Run<IEnumerable<INewsProvider>>(() => new List<INewsProvider> {newsProvider}));
            }

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
