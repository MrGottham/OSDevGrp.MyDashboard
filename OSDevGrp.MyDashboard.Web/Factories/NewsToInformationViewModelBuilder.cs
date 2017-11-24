using System;
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
            IAuthor author = news.Author;
            Uri link = news.Link;

            return new InformationViewModel
            {
                InformationIdentifier = news.Identifier,
                Timestamp = news.Timestamp,
                Header = _htmlHelper.ConvertNewLines(news.Information),
                Details = _htmlHelper.ConvertNewLines(news.Details),
                Provider = _htmlHelper.ConvertNewLines(news.Provider.Name),
                Author = author != null ? _htmlHelper.ConvertNewLines(author.Name) : null,
                ExternalUrl = link != null ? link.AbsoluteUri : "#"
            };
        }

        #endregion
    }
}