using System;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Factories;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class NewsViewModelBuilder : IViewModelBuilder<NewsViewModel, INews>
    {
        #region Methods

        public Task<NewsViewModel> BuildAsync(INews news)
        {
            if (news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }
            
            throw new NotImplementedException();
        }

        #endregion
    }
}