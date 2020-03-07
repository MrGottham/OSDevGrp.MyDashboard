using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditFilterLogic
{
    [TestClass]
    public class RemoveNsfwContentAsyncTests
    {
        #region Private variables

        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("filterableCollection")]
        public async Task RemoveNsfwContentAsync_WhenFilterableCollectionIsNull_ThrowsArgumentNullException()
        {
            IRedditFilterLogic sut = CreateSut();

            await sut.RemoveNsfwContentAsync((IEnumerable<IRedditFilterable>) null);
        }

        [TestMethod]
        public async Task RemoveNsfwContentAsync_WhenCalled_AssertOver18WasCalledOnEachFilterableInCollection()
        {
            Mock<IRedditFilterable> filterable1Mock = CreateFilterableMock();
            Mock<IRedditFilterable> filterable2Mock = CreateFilterableMock();
            Mock<IRedditFilterable> filterable3Mock = CreateFilterableMock();
            IEnumerable<IRedditFilterable> filterableCollection = CreateFilterableCollection(
                filterable1Mock.Object,
                filterable2Mock.Object,
                filterable3Mock.Object);
            
            IRedditFilterLogic sut = CreateSut();

            await sut.RemoveNsfwContentAsync(filterableCollection);

            filterable1Mock.Verify(m => m.Over18, Times.Once);
            filterable2Mock.Verify(m => m.Over18, Times.Once);
            filterable3Mock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public async Task RemoveNsfwContentAsync_WhenCalled_ReturnsFilteredCollection()
        {
            bool over18OnFilterable1 = _random.Next(1, 100) > 50;
            bool over18OnFilterable2 = _random.Next(1, 100) > 50;
            bool over18OnFilterable3 = _random.Next(1, 100) > 50;
            IRedditFilterable filterable1 = CreateFilterable(over18: over18OnFilterable1);
            IRedditFilterable filterable2 = CreateFilterable(over18: over18OnFilterable2);
            IRedditFilterable filterable3 = CreateFilterable(over18: over18OnFilterable3);
            IEnumerable<IRedditFilterable> filterableCollection = CreateFilterableCollection(
                filterable1,
                filterable2,
                filterable3);
            
            IRedditFilterLogic sut = CreateSut();

            IEnumerable<IRedditFilterable> result = await sut.RemoveNsfwContentAsync(filterableCollection);

            Assert.IsNotNull(result);
            Assert.AreEqual(Convert.ToInt32(!over18OnFilterable1) + Convert.ToInt32(!over18OnFilterable2) + Convert.ToInt32(!over18OnFilterable3), result.Count());
            Assert.AreNotEqual(over18OnFilterable1, result.Contains(filterable1));
            Assert.AreNotEqual(over18OnFilterable2, result.Contains(filterable2));
            Assert.AreNotEqual(over18OnFilterable3, result.Contains(filterable3));
        }

        private IRedditFilterLogic CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditFilterLogic();
        }

        private IEnumerable<IRedditFilterable> CreateFilterableCollection(params IRedditFilterable[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source;
        }

        private IRedditFilterable CreateFilterable(bool? over18 = null)
        {
            return CreateFilterableMock(over18).Object;
        }

        private Mock<IRedditFilterable> CreateFilterableMock(bool? over18 = null)
        {
            Mock<IRedditFilterable> filterableMock = new Mock<IRedditFilterable>();
            filterableMock.Setup(m => m.Over18)
                .Returns(over18 ?? _random.Next(1, 100) > 50);
            return filterableMock;
        }
    }
}