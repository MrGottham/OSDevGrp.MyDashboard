using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using System;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ResetRulesTests
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
        [ExpectedArgumentNullException("cookieHelper")]
        public void ResetRules_WhenCookieHelperIsNull_ThrowsArgumentNullException()
        {
            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ResetRules(null);
        }

        [TestMethod]
        public void ResetRules_WhenCalled_ExpectRulesHasBeenReset()
        {
            const bool allowNsfwContent = true;
            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            Web.Models.DashboardSettingsViewModel sut = CreateSut(allowNsfwContent: allowNsfwContent, includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ResetRules(_cookieHelperMock.Object);

            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
        }

        [TestMethod]
        public void ResetRules_WhenCalled_AssertToCookieWasCalledOnCookieHelperWithDashboardSettingsViewModel()
        {
            Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ResetRules( _cookieHelperMock.Object);

            _cookieHelperMock.Verify(m => m.ToCookie(It.Is<Web.Models.DashboardSettingsViewModel>(value => value == sut)), Times.Once);
        }

        private Web.Models.DashboardSettingsViewModel CreateSut(bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null)
        {
            return new Web.Models.DashboardSettingsViewModel
            {
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent
            };
        }
    }
}