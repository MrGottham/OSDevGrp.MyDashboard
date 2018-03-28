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
    public class RemoveNoneNsfwContentAsyncTests
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
        public void RemoveNoneNsfwContentAsync_WhenSubredditCollectionIsNull_ThrowsArgumentNullException()
        {
            IRedditFilterLogic sut = CreateSut();

            sut.RemoveNoneNsfwContentAsync(null);
        }

        [TestMethod]
        public void RemoveNoneNsfwContentAsync_WhenCalled_AssertOver18WasCalledOnEachSubredditInCollection()
        {
            Mock<IRedditSubreddit> subreddit1Mock = CreateSubredditMock();
            Mock<IRedditSubreddit> subreddit2Mock = CreateSubredditMock();
            Mock<IRedditSubreddit> subreddit3Mock = CreateSubredditMock();
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(
                subreddit1Mock.Object,
                subreddit2Mock.Object,
                subreddit3Mock.Object);
            
            IRedditFilterLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> removeNoneNsfwContentTask = sut.RemoveNoneNsfwContentAsync(subredditCollection);
            removeNoneNsfwContentTask.Wait();

            subreddit1Mock.Verify(m => m.Over18, Times.Once);
            subreddit2Mock.Verify(m => m.Over18, Times.Once);
            subreddit3Mock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public void RemoveNoneNsfwContentAsync_WhenCalled_ReturnsFilteredCollection()
        {
            bool over18OnSubreddit1 = _random.Next(1, 100) > 50;
            bool over18OnSubreddit2 = _random.Next(1, 100) > 50;
            bool over18OnSubreddit3 = _random.Next(1, 100) > 50;
            IRedditSubreddit subreddit1 = CreateSubreddit(over18: over18OnSubreddit1);
            IRedditSubreddit subreddit2 = CreateSubreddit(over18: over18OnSubreddit2);
            IRedditSubreddit subreddit3 = CreateSubreddit(over18: over18OnSubreddit3);
            IEnumerable<IRedditSubreddit> subredditCollection = CreateSubredditCollection(
                subreddit1,
                subreddit2,
                subreddit3);
            
            IRedditFilterLogic sut = CreateSut();

            Task<IEnumerable<IRedditSubreddit>> removeNoneNsfwContentTask = sut.RemoveNoneNsfwContentAsync(subredditCollection);
            removeNoneNsfwContentTask.Wait();

            IEnumerable<IRedditSubreddit> result = removeNoneNsfwContentTask.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(Convert.ToInt32(over18OnSubreddit1) + Convert.ToInt32(over18OnSubreddit2) + Convert.ToInt32(over18OnSubreddit3), result.Count());
            Assert.AreEqual(over18OnSubreddit1, result.Contains(subreddit1));
            Assert.AreEqual(over18OnSubreddit2, result.Contains(subreddit2));
            Assert.AreEqual(over18OnSubreddit3, result.Contains(subreddit3));
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

        private IRedditSubreddit CreateSubreddit(bool? over18 = null)
        {
            return CreateSubredditMock(over18).Object;
        }

        private Mock<IRedditSubreddit> CreateSubredditMock(bool? over18 = null)
        {
            Mock<IRedditSubreddit> subredditMock = new Mock<IRedditSubreddit>();
            subredditMock.Setup(m => m.Over18)
                .Returns(over18 ?? _random.Next(1, 100) > 50);
            return subredditMock;
        }
    }
}