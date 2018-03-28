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
        [ExpectedArgumentNullException("subredditCollection")]
        public void RemoveUserBannedContentAsync_WhenSubredditCollectionIsNull_ThrowsArgumentNullException()
        {
            IRedditFilterLogic sut = CreateSut();

            sut.RemoveUserBannedContentAsync(null);
        }

        [TestMethod]
        public void RemoveUserBannedContentAsync_WhenCalled_AssertUserIsBannedWasCalledOnEachSubredditInCollection()
        {
            Mock<IRedditSubreddit> subreddit1Mock = CreateSubredditMock();
            Mock<IRedditSubreddit> subreddit2Mock = CreateSubredditMock();
            Mock<IRedditSubreddit> subreddit3Mock = CreateSubredditMock();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(
                subreddit1Mock.Object,
                subreddit2Mock.Object,
                subreddit3Mock.Object);
            
            IRedditFilterLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> removeUserBannedContentTask = sut.RemoveUserBannedContentAsync(subredditCollection);
            removeUserBannedContentTask.Wait();

            subreddit1Mock.Verify(m => m.UserIsBanned, Times.Once);
            subreddit2Mock.Verify(m => m.UserIsBanned, Times.Once);
            subreddit3Mock.Verify(m => m.UserIsBanned, Times.Once);
        }

        [TestMethod]
        public void RemoveUserBannedContentAsync_WhenCalled_ReturnsFilteredCollection()
        {
            bool userIsBannedOnSubreddit1 = _random.Next(1, 100) > 50;
            bool userIsBannedOnSubreddit2 = _random.Next(1, 100) > 50;
            bool userIsBannedOnSubreddit3 = _random.Next(1, 100) > 50;
            IRedditSubreddit subreddit1 = CreateSubreddit(userIsBanned: userIsBannedOnSubreddit1);
            IRedditSubreddit subreddit2 = CreateSubreddit(userIsBanned: userIsBannedOnSubreddit2);
            IRedditSubreddit subreddit3 = CreateSubreddit(userIsBanned: userIsBannedOnSubreddit3);
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(
                subreddit1,
                subreddit2,
                subreddit3);
            
            IRedditFilterLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> removeUserBannedContentTask = sut.RemoveUserBannedContentAsync(subredditCollection);
            removeUserBannedContentTask.Wait();

            IEnumerable<IRedditSubreddit> result = removeUserBannedContentTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(Convert.ToInt32(!userIsBannedOnSubreddit1) + Convert.ToInt32(!userIsBannedOnSubreddit2) + Convert.ToInt32(!userIsBannedOnSubreddit3), result.Count());
            Assert.AreNotEqual(userIsBannedOnSubreddit1, result.Contains(subreddit1));
            Assert.AreNotEqual(userIsBannedOnSubreddit2, result.Contains(subreddit2));
            Assert.AreNotEqual(userIsBannedOnSubreddit3, result.Contains(subreddit3));
        }

        private IRedditFilterLogic CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditFilterLogic();
        }

        private IEnumerable<IRedditSubreddit> CreateSubredditCollection(params IRedditSubreddit[] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source;
        }

        private IRedditSubreddit CreateSubreddit(bool? userIsBanned = null)
        {
            return CreateSubredditMock(userIsBanned).Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock(bool? userIsBanned = null)
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            subredditMock.Setup(m => m.UserIsBanned)
                .Returns(userIsBanned ?? _random.Next(1, 100) > 50);
            return subredditMock;
        }
    }
}