using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.HtmlHelper
{
    [TestClass]
    public class RemoveEndingCommentTests
    {
        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithNull_ReturnsNull()
        {
            const string input = null;

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithEmpty_ReturnsEmpty()
        {
            string input = string.Empty;

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithWhitespace_ReturnsWhitespace()
        {
            const string input = " ";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithWhitespaces_ReturnsWhitespaces()
        {
            const string input = "  ";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithoutEndingComment_ReturnsSameValue()
        {
            string input = Guid.NewGuid().ToString("D");

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithEndingCommentOnly_ReturnsRemovedComment()
        {
            string section1 = Guid.NewGuid().ToString("D");
            string input = $"{section1}<!--";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(section1, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithEndingCommentAndWhitespace_ReturnsRemovedComment()
        {
            string section1 = Guid.NewGuid().ToString("D");
            string input = $"{section1}<!-- ";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(section1, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithEndingCommentAndWhitespaces_ReturnsRemovedComment()
        {
            string section1 = Guid.NewGuid().ToString("D");
            string input = $"{section1}<!--  ";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(section1, result);
        }

        [TestMethod]
        public void RemoveEndingComment_WhenCalledWithEndingCommentAndText_ReturnsRemovedComment()
        {
            string section1 = Guid.NewGuid().ToString("D");
            string section2 = Guid.NewGuid().ToString("D");
            string input = $"{section1}<!--{section2}";

            IHtmlHelper sut = CreateSut();

            string result = sut.RemoveEndingComment(input);
            Assert.IsNotNull(result);
            Assert.AreEqual(section1, result);
        }

        private IHtmlHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HtmlHelper();
        }
    }
}