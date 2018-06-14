using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Logic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;

namespace OSDevGrp.MyDashboard.Core.Tests.Logic.RedditThingComparer
{
    [TestClass]
    public class EqualsTests
    {
        [TestMethod]
        public void Equals_WhenXIsNullAndYIsNull_ReturnsTrue()
        {
            const IRedditThing x = null;
            const IRedditThing y = null;

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            bool result = sut.Equals(x, y);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNull_AssertFullNameWasNotCalledOnX()
        {
            Mock<IRedditThing> xMock = CreateRedditThingMock();
            const IRedditThing y = null;

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            sut.Equals(xMock.Object, y);

            xMock.Verify(m => m.FullName, Times.Never);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNull_ReturnsFalse()
        {
            IRedditThing x = CreateRedditThing();
            const IRedditThing y = null;

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            bool result = sut.Equals(x, y);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WhenXIsNullAndYIsNotNull_AssertFullNameWasNotCalledOnY()
        {
            const IRedditThing x = null;
            Mock<IRedditThing> yMock = CreateRedditThingMock();

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            sut.Equals(x, yMock.Object);

            yMock.Verify(m => m.FullName, Times.Never);
        }

        [TestMethod]
        public void Equals_WhenXIsNullAndYIsNotNull_ReturnsFalse()
        {
            const IRedditThing x = null;
            IRedditThing y = CreateRedditThing();

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            bool result = sut.Equals(x, y);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNotNull_AssertFullNameWasCalledOnX()
        {
            Mock<IRedditThing> xMock = CreateRedditThingMock();
            IRedditThing y = CreateRedditThing();

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            sut.Equals(xMock.Object, y);

            xMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNotNull_AssertFullNameWasCalledOnY()
        {
            IRedditThing x = CreateRedditThing();
            Mock<IRedditThing> yMock = CreateRedditThingMock();

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            sut.Equals(x, yMock.Object);

            yMock.Verify(m => m.FullName, Times.Once);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNotNullAndFullNameAreNotEqual_ReturnFalse()
        {
            string fullnameX = Guid.NewGuid().ToString("D");
            string fullnameY = Guid.NewGuid().ToString("D");
            IRedditThing x = CreateRedditThing(fullName: fullnameX);
            IRedditThing y = CreateRedditThing(fullName: fullnameY);

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            bool result = sut.Equals(x, y);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_WhenXIsNotNullAndYIsNotNullAndFullNameAreEqual_ReturnTrue()
        {
            string fullnameX = Guid.NewGuid().ToString("D");
            string fullnameY = fullnameX;
            IRedditThing x = CreateRedditThing(fullName: fullnameX);
            IRedditThing y = CreateRedditThing(fullName: fullnameY);

            IRedditThingComparer<IRedditThing> sut = CreateSut();

            bool result = sut.Equals(x, y);

            Assert.IsTrue(result);
        }

        private IRedditThingComparer<IRedditThing> CreateSut()
        {
            return new OSDevGrp.MyDashboard.Core.Logic.RedditThingComparer<IRedditThing>();
        }

        private IRedditThing CreateRedditThing(string fullName = null)
        {
            return CreateRedditThingMock(fullName).Object;
        }
        
        private Mock<IRedditThing> CreateRedditThingMock(string fullName = null)
        {
            Mock<IRedditThing> redditThingMock = new Mock<IRedditThing>();
            redditThingMock.Setup(m => m.FullName)
                .Returns(fullName ?? Guid.NewGuid().ToString("D"));
            return redditThingMock;
        }
    }
}