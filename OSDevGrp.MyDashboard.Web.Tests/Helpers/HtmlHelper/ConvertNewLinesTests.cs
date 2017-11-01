using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.HtmlHelper
{
    [TestClass]
    public class ConvertNewLinesTests
    {
        [TestMethod]
        public void ConvertNewLines_WhenCalledWithNull_ReturnsNull()
        {
            const string input = null;

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertNewLines_WhenCalledWithEmpty_ReturnsEmpty()
        {
            string input = string.Empty;

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ConvertNewLines_WhenCalledWithWhitespace_ReturnsWhitespace()
        {
            const string input = " ";

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ConvertNewLines_WhenCalledWithWhitespaces_ReturnsWhitespaces()
        {
            const string input = "  ";

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ConvertNewLines_WhenCalledWithoutNewLines_ReturnsUnconvertedValue()
        {
            string input = Guid.NewGuid().ToString("D").Replace(Environment.NewLine, null);

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ConvertNewLines_WhenCalledWithNewLines_ReturnsConvertedValue()
        {
            string input = $"{Guid.NewGuid().ToString("D")}{Environment.NewLine}{Guid.NewGuid().ToString("D")}{Environment.NewLine}{Guid.NewGuid().ToString("D")}";

            IHtmlHelper sut = CreateSut();

            string result = sut.ConvertNewLines(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input.Replace(Environment.NewLine, "<br />"), result);
        }

        private IHtmlHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HtmlHelper();
        }
    }
}