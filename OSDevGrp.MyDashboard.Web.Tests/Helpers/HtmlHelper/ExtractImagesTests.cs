using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Tests.Helpers.HtmlHelper
{
    [TestClass]
    public class ExtractImagesTests
    {
        [TestMethod]
        public void ExtractImages_WhenValueIsNullEmptyOrWhiteSpace_ExpectEmptyImageUrlCollection()
        {
            foreach (string testValue in new[] {null, string.Empty, " ", "  ", "   "})
            {
                IList<Uri> imageUrlCollection = null;

                IHtmlHelper sut = CreateSut();
                
                sut.ExtractImages(testValue, out imageUrlCollection);

                Assert.IsNotNull(imageUrlCollection);
                Assert.AreEqual(0, imageUrlCollection.Count);
            }
        }

        [TestMethod]
        public void ExtractImages_WhenValueIsNullEmptyOrWhiteSpace_ReturnsValue()
        {
            foreach (string testValue in new[] {null, string.Empty, " ", "  ", "   "})
            {
                IList<Uri> imageUrlCollection = null;

                IHtmlHelper sut = CreateSut();
                
                string result = sut.ExtractImages(testValue, out imageUrlCollection);

                Assert.AreEqual(testValue, result);
            }
        }

        private IHtmlHelper CreateSut()
        {
            return new OSDevGrp.MyDashboard.Web.Helpers.HtmlHelper();
        }
    }
}