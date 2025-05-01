using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using System;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ApplyRulesTests
    {
        #region Private variables

        private Mock<ICookieHelper> _cookieHelperMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _cookieHelperMock = new Mock<ICookieHelper>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        [ExpectedArgumentNullException("rules")]
        public void ApplyRules_WhenDashboardRulesIsNull_ThrowsArgumentNullException()
        {
            const IDashboardRules dashboardRules = null;

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRules, _cookieHelperMock.Object);
        }

        [TestMethod]
        [ExpectedArgumentNullException("cookieHelper")]
        public void ApplyRules_WhenCalledWithDashboardRulesAndCookieHelperIsNull_ThrowsArgumentNullException()
        {
            IDashboardRules dashboardRules = CreateDashboardRules();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRules, null);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithDashboardRules_AssertAllowNsfwContentWasCalledOnDashboardRules()
        {
            Mock<IDashboardRules> dashboardRulesMock = CreateDashboardRulesMock();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRulesMock.Object, _cookieHelperMock.Object);

            dashboardRulesMock.Verify(m => m.AllowNsfwContent, Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithDashboardRulesWhereAllowNsfwContentIsFalse_ExpectRulesHasBeenApplid()
        {
            const bool allowNsfwContent = false;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(dashboardRules, _cookieHelperMock.Object);

            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithDashboardRulesWhereAllowNsfwContentIsFalse_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            const bool allowNsfwContent = false;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRules, _cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithDashboardRulesWhereAllowNsfwContentIsTrue_ExpectRulesHasBeenApplid()
        {
            const bool allowNsfwContent = true;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(dashboardRules, _cookieHelperMock.Object);

            Assert.IsTrue(sut.AllowNsfwContent);
            Assert.IsTrue(sut.IncludeNsfwContent.HasValue);
            Assert.AreEqual(includeNsfwContent, sut.IncludeNsfwContent);
            Assert.IsTrue(sut.OnlyNsfwContent.HasValue);
            Assert.AreEqual(onlyNsfwContent, sut.OnlyNsfwContent);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithDashboardRulesWhereAllowNsfwContentIsTrue_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            const bool allowNsfwContent = true;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRules, _cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redditAuthenticatedUser")]
        public void ApplyRules_WhenRedditAuthenticatedUserIsNull_ThrowsArgumentNullException()
        {
            const IRedditAuthenticatedUser redditAuthenticatedUser = null;
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, _cookieHelperMock.Object);
        }

        [TestMethod]
        [ExpectedArgumentNullException("redditAccessToken")]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserAndRedditAccessTokenIsNull_ThrowsArgumentNullException()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, null, _cookieHelperMock.Object);
        }

        [TestMethod]
        [ExpectedArgumentNullException("cookieHelper")]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserAndCookieHelperIsNull_ThrowsArgumentNullException()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, null);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserAndRedditAccessToken_AssertOver18WasCalledOnRedditAuthenticatedUser()
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = CreateRedditAuthenticatedUserMock();
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUserMock.Object, redditAccessToken, _cookieHelperMock.Object);

            redditAuthenticatedUserMock.Verify(m => m.Over18, Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserAndRedditAccessToken_AssertToBase64WasCalledOnRedditAccessToken()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser();
            Mock<IRedditAccessToken> redditAccessTokenMock = CreateRedditAccessTokenMock();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, redditAccessTokenMock.Object, _cookieHelperMock.Object);

            redditAccessTokenMock.Verify(m => m.ToBase64(), Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserWhereOver18IsTrueAndWithRedditAccessToken_ExpectRulesHasBeenApplid()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: true);

            string asBase64 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(asBase64: asBase64);

            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, _cookieHelperMock.Object);

            Assert.IsTrue(sut.AllowNsfwContent);
            Assert.IsTrue(sut.IncludeNsfwContent.HasValue);
            Assert.AreEqual(includeNsfwContent, sut.IncludeNsfwContent);
            Assert.IsTrue(sut.OnlyNsfwContent.HasValue);
            Assert.AreEqual(onlyNsfwContent, sut.OnlyNsfwContent);
            Assert.IsNotNull(sut.RedditAccessToken);
            Assert.IsNotEmpty(sut.RedditAccessToken);
            Assert.AreEqual(asBase64, sut.RedditAccessToken);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserWhereOver18IsTrueAndWithRedditAccessToken_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: true);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, _cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserWhereOver18IsFalseAndWithRedditAccessToken_ExpectRulesHasBeenApplid()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: false);

            string asBase64 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken(asBase64: asBase64);

            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, _cookieHelperMock.Object);

            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNotNull(sut.RedditAccessToken);
            Assert.IsNotEmpty(sut.RedditAccessToken);
            Assert.AreEqual(asBase64, sut.RedditAccessToken);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWithRedditAuthenticatedUserWhereOver18IsFalseAndWithRedditAccessToken_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            IRedditAuthenticatedUser redditAuthenticatedUser = CreateRedditAuthenticatedUser(over18: false);
            IRedditAccessToken redditAccessToken = CreateRedditAccessToken();

            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(redditAuthenticatedUser, redditAccessToken, _cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        [ExpectedArgumentNullException("cookieHelper")]
        public void ApplyRules_WhenWithOnlyCookieHelperEqualToNull_ThrowsArgumentNullException()
        {
            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(null);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsTrueAndAllowNsfwContentIsTrue_ExpectRulesHasBeenApplid()
        {
            const bool useReddit = true;
            const bool allowNsfwContent = true;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            string redditAccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit, allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent, redditAccessToken: redditAccessToken);

            sut.ApplyRules(_cookieHelperMock.Object);

            Assert.IsTrue(sut.UseReddit);
            Assert.IsTrue(sut.AllowNsfwContent);
            Assert.IsTrue(sut.IncludeNsfwContent.HasValue);
            Assert.AreEqual(includeNsfwContent, sut.IncludeNsfwContent);
            Assert.IsTrue(sut.OnlyNsfwContent.HasValue);
            Assert.AreEqual(onlyNsfwContent, sut.OnlyNsfwContent);
            Assert.IsNotNull(sut.RedditAccessToken);
            Assert.IsNotEmpty(sut.RedditAccessToken);
            Assert.AreEqual(redditAccessToken, sut.RedditAccessToken);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsTrueAndAllowNsfwContentIsTrue_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            const bool useReddit = true;
            const bool allowNsfwContent = true;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit, allowNsfwContent: allowNsfwContent);

            sut.ApplyRules(_cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsTrueAndAllowNsfwContentIsFalse_ExpectRulesHasBeenApplid()
        {
            const bool useReddit = true;
            const bool allowNsfwContent = false;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            string redditAccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit, allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent, redditAccessToken: redditAccessToken);

            sut.ApplyRules(_cookieHelperMock.Object);

            Assert.IsTrue(sut.UseReddit);
            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNotNull(sut.RedditAccessToken);
            Assert.IsNotEmpty(sut.RedditAccessToken);
            Assert.AreEqual(redditAccessToken, sut.RedditAccessToken);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsTrueAndAllowNsfwContentIsFalse_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            const bool useReddit = true;
            const bool allowNsfwContent = false;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit, allowNsfwContent: allowNsfwContent);

            sut.ApplyRules(_cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsFalse_ExpectRulesHasBeenApplid()
        {
            const bool useReddit = false;
            bool allowNsfwContent = _random.Next(100) > 50;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            string redditAccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit, allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent, redditAccessToken: redditAccessToken);

            sut.ApplyRules(_cookieHelperMock.Object);

            Assert.IsFalse(sut.UseReddit);
            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNull(sut.RedditAccessToken);
        }

        [TestMethod]
        public void ApplyRules_WhenWithOnlyCookieHelperAndUseRedditInDashboardSettingsViewModelIsFalse_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            const bool useReddit = false;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(useReddit: useReddit);

            sut.ApplyRules(_cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        private Web.Models.DashboardSettingsViewModel CreateSut(bool? useReddit = null, bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null, string redditAccessToken = null)
        {
            return new Web.Models.DashboardSettingsViewModel
            {
                UseReddit = useReddit ?? _random.Next(100) > 50,
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent,
                RedditAccessToken = redditAccessToken
            };
        }

        private IDashboardRules CreateDashboardRules(bool? allowNsfwContent = null)
        {
            return CreateDashboardRulesMock(allowNsfwContent).Object;
        }

        private Mock<IDashboardRules> CreateDashboardRulesMock(bool? allowNsfwContent = null)
        {
            Mock<IDashboardRules> dashboardRulesMock = new Mock<IDashboardRules>();
            dashboardRulesMock.Setup(m => m.AllowNsfwContent)
                .Returns(allowNsfwContent ?? _random.Next(100) > 50);
            return dashboardRulesMock;
        }

        private IRedditAuthenticatedUser CreateRedditAuthenticatedUser(bool? over18 = null)
        {
            return CreateRedditAuthenticatedUserMock(over18).Object;
        }

        private Mock<IRedditAuthenticatedUser> CreateRedditAuthenticatedUserMock(bool? over18 = null)
        {
            Mock<IRedditAuthenticatedUser> redditAuthenticatedUserMock = new Mock<IRedditAuthenticatedUser>();
            redditAuthenticatedUserMock.Setup(m => m.Over18)
                .Returns(over18 ?? _random.Next(100) > 50);
            return redditAuthenticatedUserMock;
        }

        private static IRedditAccessToken CreateRedditAccessToken(string asBase64 = null)
        {
            return CreateRedditAccessTokenMock(asBase64).Object;
        }

        private static Mock<IRedditAccessToken> CreateRedditAccessTokenMock(string asBase64 = null)
        {
            Mock<IRedditAccessToken> redditAccessTokenMock = new Mock<IRedditAccessToken>();
            redditAccessTokenMock.Setup(m => m.ToBase64())
                .Returns(asBase64 ?? Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
            return redditAccessTokenMock;
        }
    }
}