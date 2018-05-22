using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Web.Tests.Models.DashboardSettingsViewModel
{
    [TestClass]
    public class ApplyRulesTests
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
        [ExpectedArgumentNullException("rules")]
        public void ApplyRules_WhenDashboardRulesIsNull_ThrowsArgumentNullException()
        {
            const IDashboardRules dashboardRules = null;

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRules);
        }

        [TestMethod]
        public void ApplyRules_WhenCalled_AssertAllowNsfwContentWasCalledOnDashboardRules()
        {
            Mock<IDashboardRules> dashboardRulesMock = CreateDashboardRulesMock();

            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut();

            sut.ApplyRules(dashboardRulesMock.Object);

            dashboardRulesMock.Verify(m => m.AllowNsfwContent, Times.Once);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWhereAllowNsfwContentIsFalseInDasboardRules_ExpectRulesHasBeenApplid()
        {
            const bool allowNsfwContent = false;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            bool? includeNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            bool? onlyNsfwContent = _random.Next(100) > 50 ? _random.Next(100) > 50 : (bool?) null;
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(dashboardRules);

            Assert.IsFalse(sut.AllowNsfwContent);
            Assert.IsFalse(sut.IncludeNsfwContent.HasValue);
            Assert.IsNull(sut.IncludeNsfwContent);
            Assert.IsFalse(sut.OnlyNsfwContent.HasValue);
            Assert.IsNull(sut.OnlyNsfwContent);
        }

        [TestMethod]
        public void ApplyRules_WhenCalledWhereAllowNsfwContentIsTrueInDasboardRules_ExpectRulesHasBeenApplid()
        {
            const bool allowNsfwContent = true;
            IDashboardRules dashboardRules = CreateDashboardRules(allowNsfwContent: allowNsfwContent);

            bool includeNsfwContent = _random.Next(100) > 50;
            bool onlyNsfwContent = _random.Next(100) > 50;
            OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel sut = CreateSut(includeNsfwContent: includeNsfwContent, onlyNsfwContent: onlyNsfwContent);

            sut.ApplyRules(dashboardRules);

            Assert.IsTrue(sut.AllowNsfwContent);
            Assert.IsTrue(sut.IncludeNsfwContent.HasValue);
            Assert.AreEqual(includeNsfwContent, sut.IncludeNsfwContent);
            Assert.IsTrue(sut.OnlyNsfwContent.HasValue);
            Assert.AreEqual(onlyNsfwContent, sut.OnlyNsfwContent);
        }

        private OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel CreateSut(bool? allowNsfwContent = null, bool? includeNsfwContent = null, bool? onlyNsfwContent = null)
        {
            return new OSDevGrp.MyDashboard.Web.Models.DashboardSettingsViewModel
            {
                AllowNsfwContent = allowNsfwContent ?? _random.Next(100) > 50,
                IncludeNsfwContent = includeNsfwContent,
                OnlyNsfwContent = onlyNsfwContent
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
    }
}