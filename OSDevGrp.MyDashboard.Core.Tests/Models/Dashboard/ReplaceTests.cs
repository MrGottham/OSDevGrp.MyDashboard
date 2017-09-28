using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Tests.Helpers.Attributes;

namespace OSDevGrp.MyDashboard.Core.Tests.Models.Dashboard
{
    [TestClass]
    public class ReplaceTests
    {
        [TestMethod]
        [ExpectedArgumentNullException("news")]
        public void Replace_WhenCalledWithNewsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<INews> news = null;

            IDashboard sut = CreateSut();

            sut.Replace(news);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithNewsNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<INews> news = new List<INews>
            {
                new Mock<INews>().Object,
                new Mock<INews>().Object,
                new Mock<INews>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(news);

            Assert.IsNotNull(sut.News);
            Assert.AreEqual(news.Count(), sut.News.Count());
            Assert.IsTrue(sut.News.All(item => news.Contains(item)));
        }
        
        [TestMethod]
        [ExpectedArgumentNullException("systemErrors")]
        public void Replace_WhenCalledWithSystemErrorsEqualToNull_ThrowsArgumentNullExcpetion()
        {
            const IEnumerable<ISystemError> systemErrors = null;

            IDashboard sut = CreateSut();

            sut.Replace(systemErrors);
        }
        
        [TestMethod]
        public void Replace_WhenCalledWithSystemErrorsNotEqualToNull_ExpectedReplacedCollection()
        {
            IEnumerable<ISystemError> systemErrors = new List<ISystemError>
            {
                new Mock<ISystemError>().Object,
                new Mock<ISystemError>().Object,
                new Mock<ISystemError>().Object
            };

            IDashboard sut = CreateSut();

            sut.Replace(systemErrors);

            Assert.IsNotNull(sut.SystemErrors);
            Assert.AreEqual(systemErrors.Count(), sut.SystemErrors.Count());
            Assert.IsTrue(sut.SystemErrors.All(systemError => systemErrors.Contains(systemError)));
        }
        
        private IDashboard CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Models.Dashboard();
        }
   }
}
