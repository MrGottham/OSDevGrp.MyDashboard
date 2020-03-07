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
    public class RemoveUserBannedContentAsyncTests
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
        public async Task RemoveUserBannedContentAsync_WhenFilterableCollectionIsNull_ThrowsArgumentNullException()
        {
            IRedditFilterLogic sut = CreateSut();

            await sut.RemoveUserBannedContentAsync((IEnumerable<IRedditFilterable>) null);
        }

        [TestMethod]
        public async Task RemoveUserBannedContentAsync_WhenCalled_AssertUserBannedWasCalledOnEachFilterableInCollection()
        {
            Mock<IRedditFilterable> filterable1Mock = CreateFilterableMock();
            Mock<IRedditFilterable> filterable2Mock = CreateFilterableMock();
            Mock<IRedditFilterable> filterable3Mock = CreateFilterableMock();
            IEnumerable<IRedditFilterable> filterableCollection = CreateFilterableCollection(
                filterable1Mock.Object,
                filterable2Mock.Object,
                filterable3Mock.Object);
            
            IRedditFilterLogic sut = CreateSut();

            await sut.RemoveUserBannedContentAsync(filterableCollection);

            filterable1Mock.Verify(m => m.UserBanned, Times.Once);
            filterable2Mock.Verify(m => m.UserBanned, Times.Once);
            filterable3Mock.Verify(m => m.UserBanned, Times.Once);
        }

        [TestMethod]
        public async Task RemoveUserBannedContentAsync_WhenCalled_ReturnsFilteredCollection()
        {
            bool userBannedOnFilterable1 = _random.Next(1, 100) > 50;
            bool userBannedOnFilterable2 = _random.Next(1, 100) > 50;
            bool userBannedOnFilterable3 = _random.Next(1, 100) > 50;
            IRedditFilterable filterable1 = CreateFilterable(userBanned: userBannedOnFilterable1);
            IRedditFilterable filterable2 = CreateFilterable(userBanned: userBannedOnFilterable2);
            IRedditFilterable filterable3 = CreateFilterable(userBanned: userBannedOnFilterable3);
            IEnumerable<IRedditFilterable> filterableCollection = CreateFilterableCollection(
                filterable1,
                filterable2,
                filterable3);
            
            IRedditFilterLogic sut = CreateSut();

            IEnumerable<IRedditFilterable> result = await sut.RemoveUserBannedContentAsync(filterableCollection);

            Assert.IsNotNull(result);
            Assert.AreEqual(Convert.ToInt32(!userBannedOnFilterable1) + Convert.ToInt32(!userBannedOnFilterable2) + Convert.ToInt32(!userBannedOnFilterable3), result.Count());
            Assert.AreNotEqual(userBannedOnFilterable1, result.Contains(filterable1));
            Assert.AreNotEqual(userBannedOnFilterable2, result.Contains(filterable2));
            Assert.AreNotEqual(userBannedOnFilterable3, result.Contains(filterable3));
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

        private IRedditFilterable CreateFilterable(bool? userBanned = null)
        {
            return CreateFilterableMock(userBanned).Object;
        }

        private Mock<IRedditFilterable> CreateFilterableMock(bool? userBanned = null)
        {
            Mock<IRedditFilterable> filterableMock = new Mock<IRedditFilterable>();
            filterableMock.Setup(m => m.UserBanned)
                .Returns(userBanned ?? _random.Next(1, 100) > 50);
            return filterableMock;
        }
    }
}