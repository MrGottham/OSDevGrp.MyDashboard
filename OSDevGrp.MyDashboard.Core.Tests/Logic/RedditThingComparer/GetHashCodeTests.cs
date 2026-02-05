using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditThingComparer
{
    [TestClass]
    public class GetHashCodeTests
    {
        [TestMethod]
        public void GetHashCode_WhenRedditThingIsNull_ThrowsArgumentNullException()
        {
            const IRedditThing redditThing = null;

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => sut.GetHashCode(redditThing));

            Assert.AreEqual("redditThing", result.ParamName);
        }

        [TestMethod]
        public void GetHashCode_WhenCalled_AssertFullNameWasCalledOnRedditThing()
        {
            Mock<IRedditThing> redditThingMock = CreateRedditThingMock();

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            sut.GetHashCode(redditThingMock.Object);

            redditThingMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public void GetHashCode_WhenFullNameOnRedditThingIsNull_ThrowsArgumentException()
        {
            IRedditThing redditThing = CreateRedditThing(fullNameIsNull: true);

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            ArgumentException result = Assert.Throws<ArgumentException>(() => sut.GetHashCode(redditThing));

            Assert.StartsWith("The full name for the Reddit thing has not been initialized.", result.Message);
            Assert.AreEqual("FullName", result.ParamName);
        }

        [TestMethod]
        public void GetHashCode_WhenCalled_ReturnsHashCodeForFullName()
        {
            string fullName = Guid.NewGuid().ToString("D");
            IRedditThing redditThing = CreateRedditThing(fullName: fullName);

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            int result = sut.GetHashCode(redditThing);

            Assert.AreEqual(fullName.GetHashCode(), result);
        }

        private IRedditThingComparer<IRedditThing> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditThing>();
        }

        private IRedditThing CreateRedditThing(bool fullNameIsNull = false, string fullName = null)
        {
            return CreateRedditThingMock(fullNameIsNull, fullName).Object;
        }
        
        private Mock<IRedditThing> CreateRedditThingMock(bool fullNameIsNull = false, string fullName = null)
        {
            Mock<IRedditThing> redditThingMock = new Mock<IRedditThing>();
            redditThingMock.Setup(m => m.FullName)
                .Returns(fullNameIsNull ? null : fullName ?? Guid.NewGuid().ToString("D"));
            return redditThingMock;
        }
    }
}