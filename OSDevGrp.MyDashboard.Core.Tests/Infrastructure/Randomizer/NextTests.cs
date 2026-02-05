using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;

namespace OSDevGrp.MyDashboard.Core.Tests.Infrastructure.Randomizer
{
    [TestClass]
    public class NextTests
    {
        #region Private variables

        private Mock<ISeedGenerator> _seedGeneratorMock;
        private Random _random;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _seedGeneratorMock = new Mock<ISeedGenerator>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        [TestMethod]
        public void Next_WhenMaxValueLowerThanMinValue_ThrowsArgumentException()
        {
            int minValue = _random.Next(1, 100);
            int maxValue = minValue - _random.Next(1, 100);

            IRandomizer sut = CreateSut();

            ArgumentException result = Assert.Throws<ArgumentException>(() => sut.Next(minValue, maxValue));

            Assert.StartsWith("The maximum value cannot be lower than the minimum value.", result.Message);
            Assert.AreEqual("maxValue", result.ParamName);
        }

        [TestMethod]
        public void Next_WhenCalled_ReturnsValueInInterval()
        {
            int minValue = _random.Next(1, 100);
            int maxValue = minValue + _random.Next(1, 100);

            IRandomizer sut = CreateSut();

            int result = sut.Next(minValue, maxValue);

            Assert.IsTrue(result >= minValue && result <= maxValue);
        }

        private IRandomizer CreateSut()
        {
            _seedGeneratorMock.Setup(m => m.Generate())
                .Returns(_random.Next(100));

            return new Core.Infrastructure.Randomizer(_seedGeneratorMock.Object);
        }
    }
}