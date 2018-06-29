using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;

namespace OSDevGrp.MyDashboard.Web.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        #region Methods

        public Task<byte[]> ReadAsync(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            return Task.Run<byte[]>(async () => {
                using (HttpClient httpClient = new HttpClient())
                {
                    using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                        {
                            if (httpResponseMessage.IsSuccessStatusCode == false)
                            {
                                switch (httpResponseMessage.StatusCode)
                                {
                                    case HttpStatusCode.Unauthorized:
                                        throw new UnauthorizedAccessException($"You are not authorized to perform the operation: {httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri}");
                                    
                                    default:
                                        throw new Exception($"Unable to perform the operation ({httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri}): {httpResponseMessage.ReasonPhrase}");
                                }
                            }
                            return await httpResponseMessage.Content.ReadAsByteArrayAsync();
                        }
                    }
                }
            });
        }

        #endregion
    }
}