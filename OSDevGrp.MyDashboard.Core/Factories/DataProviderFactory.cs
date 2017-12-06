using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Models;

namespace OSDevGrp.MyDashboard.Core.Factories
{
    public class DataProviderFactory : IDataProviderFactory
    {
        #region Methods

        public Task<IEnumerable<INewsProvider>> BuildNewsProvidersAsync()
        {
            IEnumerable<INewsProvider> newsProviders = new List<INewsProvider>
            {
                new NewsProvider("DR", new Uri("http://www.dr.dk/nyheder/service/feeds/allenyheder")),
                new NewsProvider("TV 2", new Uri("http://feeds.tv2.dk/nyheder/rss")),
                new NewsProvider("Børsen", new Uri("http://borsen.dk/rss")),
                new NewsProvider("Computerworld", new Uri("https://www.computerworld.dk/rss/all")),
                new NewsProvider("Version2", new Uri("https://www.version2.dk/it-nyheder/rss"))
            };

            return Task.Run(() => newsProviders);
        }

        public Task<Uri> AcquireRedditAccessTokenAsync(string clientId, string state, Uri redirectUri)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            throw new NotImplementedException();
        }

        #endregion 
    }
}