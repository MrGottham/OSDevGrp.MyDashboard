using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.HtmlHelper
{
    [TestClass]
    public class ExtractImagesTests
    {
        [TestMethod]
        public void ExtractImages_WhenCalledWithNull_ExpectEmptyImageUrlCollection()
        {
            const string input = null;
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(0, imageUrlCollection.Count);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithNull_ReturnsNull()
        {
            const string input = null;
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithEmpty_ExpectEmptyImageUrlCollection()
        {
            string input = string.Empty;
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(0, imageUrlCollection.Count);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithEmpty_ReturnsEmpty()
        {
            string input = string.Empty;
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithWhitespace_ExpectEmptyImageUrlCollection()
        {
            const string input = " ";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(0, imageUrlCollection.Count);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithWhitespace_ReturnsWhitespace()
        {
            const string input = " ";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithWhitespaces_ExpectEmptyImageUrlCollection()
        {
            const string input = "  ";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(0, imageUrlCollection.Count);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithWhitespaces_ReturnsWhitespaces()
        {
            const string input = "  ";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithoutAnyImages_ExpectEmptyImageUrlCollection()
        {
            string input = Guid.NewGuid().ToString("D");
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(0, imageUrlCollection.Count);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithoutAnyImages_ReturnsSameValue()
        {
            string input = Guid.NewGuid().ToString("D");
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithOneImage_ExpectNonEmptyImageUrlCollection()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection1 = Guid.NewGuid().ToString("D");
            string inputSection2 = Guid.NewGuid().ToString("D");
            string input = $"{inputSection1}<img src=\"{imageUrl}\" />{inputSection2}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(1, imageUrlCollection.Count);
            Assert.AreEqual(imageUrl, imageUrlCollection.ElementAt(0).AbsoluteUri);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithOneImage_ReturnsStringWithoutImageHtml()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection1 = Guid.NewGuid().ToString("D");
            string inputSection2 = Guid.NewGuid().ToString("D");
            string input = $"{inputSection1}<img src=\"{imageUrl}\" />{inputSection2}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual($"{inputSection1}{inputSection2}", result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithTwoImage_ExpectNonEmptyImageUrlCollection()
        {
            string imageUrl1 = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string imageUrl2 = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection1 = Guid.NewGuid().ToString("D");
            string inputSection2 = Guid.NewGuid().ToString("D");
            string inputSection3 = Guid.NewGuid().ToString("D");
            string input = $"{inputSection1}<img src=\"{imageUrl1}\" />{inputSection2}<img src=\"{imageUrl2}\" />{inputSection3}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(2, imageUrlCollection.Count);
            Assert.AreEqual(imageUrl1, imageUrlCollection.ElementAt(0).AbsoluteUri);
            Assert.AreEqual(imageUrl2, imageUrlCollection.ElementAt(1).AbsoluteUri);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledWithTwoImage_ReturnsStringWithoutImageHtml()
        {
            string imageUrl1 = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string imageUrl2 = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection1 = Guid.NewGuid().ToString("D");
            string inputSection2 = Guid.NewGuid().ToString("D");
            string inputSection3 = Guid.NewGuid().ToString("D");
            string input = $"{inputSection1}<img src=\"{imageUrl1}\" />{inputSection2}<img src=\"{imageUrl2}\" />{inputSection3}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual($"{inputSection1}{inputSection2}{inputSection3}", result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledImageAtStart_ExpectNonEmptyImageUrlCollection()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection = Guid.NewGuid().ToString("D");
            string input = $"<img src=\"{imageUrl}\" />{inputSection}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(1, imageUrlCollection.Count);
            Assert.AreEqual(imageUrl, imageUrlCollection.ElementAt(0).AbsoluteUri);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledImageAtStart_ReturnsStringWithoutImageHtml()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection = Guid.NewGuid().ToString("D");
            string input = $"<img src=\"{imageUrl}\" />{inputSection}";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(inputSection, result);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledImageAtEnd_ExpectNonEmptyImageUrlCollection()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection = Guid.NewGuid().ToString("D");
            string input = $"{inputSection}<img src=\"{imageUrl}\" />";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            sut.ExtractImages(input, out imageUrlCollection);

            Assert.IsNotNull(imageUrlCollection);
            Assert.AreEqual(1, imageUrlCollection.Count);
            Assert.AreEqual(imageUrl, imageUrlCollection.ElementAt(0).AbsoluteUri);
        }

        [TestMethod]
        public void ExtractImages_WhenCalledImageAtEnd_ReturnsStringWithoutImageHtml()
        {
            string imageUrl = $"http://localhost/{Guid.NewGuid().ToString("D")}.png";
            string inputSection = Guid.NewGuid().ToString("D");
            string input = $"{inputSection}<img src=\"{imageUrl}\" />";
            IList<Uri> imageUrlCollection = null;

            IHtmlHelper sut = CreateSut();
                
            string result = sut.ExtractImages(input, out imageUrlCollection);

            Assert.AreEqual(inputSection, result);
        }

        private IHtmlHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HtmlHelper();
        }
    }
}