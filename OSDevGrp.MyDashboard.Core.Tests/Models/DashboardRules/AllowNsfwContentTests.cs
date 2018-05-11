using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.DashboardRules
{
    [TestClass]
    public class AllowNsfwContentTests
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
        public void AllowNsfwContent_WhenRedditAuthenticatedUserIsNull_ReturnsFalse()
        {
            const IRedditAuthenticatedUser redditAuthenticatedUser = null;

            IDashboardRules sut = CreateSut(redditAuthenticatedUser);
            
            Assert.IsFalse(sut.AllowNsfwContent);
        }

        [TestMethod]
        public void AllowNsfwContent_WhenRedditAuthenticatedUserIsNotNull_AssertOver18WasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();

            IDashboardRules sut = CreateSut(redditAuthenticatedUserMock.Object);
            bool result = sut.AllowNsfwContent;

            redditAuthenticatedUserMock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public void AllowNsfwContent_WhenRedditAuthenticatedUserIsNotOver18_ReturnsFalse()
        {
            const bool over18 = false;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);

            IDashboardRules sut = CreateSut(redditAuthenticatedUser);
            
            Assert.IsFalse(sut.AllowNsfwContent);
        }

        [TestMethod]
        public void AllowNsfwContent_WhenRedditAuthenticatedUserIsOver18_ReturnsTrue()
        {
            const bool over18 = true;
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: over18);

            IDashboardRules sut = CreateSut(redditAuthenticatedUser);
            
            Assert.IsTrue(sut.AllowNsfwContent);
        }

        private IDashboardRules CreateSut(IRedditAuthenticatedUser redditAuthenticatedUser)
        {
            return new OSDevGrp.MyDashboard.Core.Models.DashboardRules(redditAuthenticatedUser);
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser(bool? over18 = null)
        {
            return CreateRedditAuthenticatedUserMock(over18).Object;
        }

        private Mock<IRedditAuthenticatedUser> CreateRedditAuthenticatedUserMock(bool? over18 = null)
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.Over18)
                .Returns(over18 ?? _random.Next(1, 100) > 50);
            return redditAuthenticatedUserMock;
        }
   }
}