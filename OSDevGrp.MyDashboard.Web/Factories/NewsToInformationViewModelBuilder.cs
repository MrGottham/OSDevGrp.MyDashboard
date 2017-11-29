using System;
using System.Collections.Generic;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class NewsToInformationViewModelBuilder : ViewModelBuilderBase<InformationViewModel, INews>
    {
        #region Private variables

        private readonly IHtmlHelper _htmlHelper;

        #endregion

        #region Constructor

        public NewsToInformationViewModelBuilder(IHtmlHelper htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region Methods

        protected override InformationViewModel Build(INews news)
        {
            List<Uri> imageUrlCollection = new List<Uri>();

            string header = ExtractImages(_htmlHelper.ConvertNewLines(news.Information), imageUrlCollection);
            string details = ExtractImages(_htmlHelper.ConvertNewLines(news.Details), imageUrlCollection);

            IAuthor author = news.Author;
            Uri link = news.Link;

            return new InformationViewModel
            {
                InformationIdentifier = news.Identifier,
                Timestamp = news.Timestamp,
                Header = header,
                Details = details,
                Provider = _htmlHelper.ConvertNewLines(news.Provider.Name),
                Author = author != null ? _htmlHelper.ConvertNewLines(author.Name) : null,
                ExternalUrl = link != null ? link.AbsoluteUri : "#",
                ImageUrl = imageUrlCollection.Count > 0 ? imageUrlCollection[0].AbsoluteUri : null
           };
        }

        private string ExtractImages(string value, List<Uri> imageUrlCollection)
        {
            if (imageUrlCollection == null)
            {
                throw new ArgumentNullException(nameof(imageUrlCollection));
            }

            IList<Uri> imageUrlCollectionForValue = null;
            string result = _htmlHelper.ExtractImages(value, out imageUrlCollectionForValue);
            if (imageUrlCollectionForValue != null && imageUrlCollectionForValue.Count > 0)
            {
                imageUrlCollection.AddRange(imageUrlCollectionForValue);
            }

            return result;
        }

        #endregion
    }
}