using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Helpers
{
    public class CookieHelper : ICookieHelper
    {
        #region Private variables

        private readonly IContentHelper _contentHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        #endregion

        #region Constructor

        public CookieHelper(IContentHelper contentHelper, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            if (contentHelper == null)
            {
                throw new ArgumentNullException(nameof(contentHelper));
            }
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }
            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            _contentHelper = contentHelper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        #endregion

        #region Methods

        public void ToCookie(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            StoreCookie(DashboardSettingsViewModel.CookieName, dashboardSettingsViewModel,
                viewModel => _contentHelper.ToBase64String(viewModel),
                viewModel => 
                {
                    DateTime expires = DateTime.Now.AddHours(8);
                    if (string.IsNullOrWhiteSpace(viewModel.RedditAccessToken) == false)
                    {
                        IRedditAccessToken redditAccessToken = RedditAccessToken.Create(viewModel.RedditAccessToken);
                        if (redditAccessToken != null && redditAccessToken.Expires <= expires)
                        {
                            expires = redditAccessToken.Expires;
                        }
                    }

                    return new CookieOptions
                    {
                        Expires = expires,
                        Secure = true,
                        SameSite = SameSiteMode.None
                    };
                });
        }

        public void ToCookie(DashboardViewModel dashboardViewModel)
        {
            if (dashboardViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardViewModel));
            }

            DateTime expires = DateTime.Now.AddSeconds(30);

            StoreCookie(DashboardViewModel.CookieName, dashboardViewModel,
                viewModel =>
                {
                    byte[] dashboardViewModelAsByteArray = _contentHelper.ToByteArray(viewModel);
                    if (dashboardViewModelAsByteArray == null)
                    {
                        return null;
                    }

                    string memoryCacheKey = $"{DashboardViewModel.CookieName}.{Guid.NewGuid().ToString("D")}";

                    using (ICacheEntry cacheEntry = _memoryCache.CreateEntry(memoryCacheKey))
                    {
                        if (cacheEntry == null)
                        {
                            return null;
                        }
                        
                        cacheEntry.Value = dashboardViewModelAsByteArray;
                        cacheEntry.AbsoluteExpiration = expires;
                    }

                    return _contentHelper.ToBase64String(memoryCacheKey);
                },
                viewModel => new CookieOptions {Expires = expires, Secure = true, SameSite = SameSiteMode.None});
        }

        public DashboardSettingsViewModel ToDashboardSettingsViewModel()
        {
            return RestoreFromCookie<DashboardSettingsViewModel>(DashboardSettingsViewModel.CookieName, cookieValue => _contentHelper.ToDashboardSettingsViewModel(cookieValue));
        }

        public DashboardViewModel ToDashboardViewModel()
        {
            return RestoreFromCookie<DashboardViewModel>(DashboardViewModel.CookieName, cookieValue => 
            {
                string memoryCacheKey = _contentHelper.ToValue(cookieValue);
                if (string.IsNullOrWhiteSpace(memoryCacheKey))
                {
                    return null;
                }

                if (_memoryCache.TryGetValue(memoryCacheKey, out byte[] memoryCacheValue) == false)
                {
                    return null;
                }

                return _contentHelper.ToDashboardViewModel(memoryCacheValue);
            });
        }

        private void StoreCookie<T>(string cookieName, T viewModel, Func<T, string> cookieValueGetter, Func<T, CookieOptions> cookieOptionsGetter) where T : IViewModel
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentNullException(nameof(cookieName));
            }
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            if (cookieValueGetter == null)
            {
                throw new ArgumentNullException(nameof(cookieValueGetter));
            }
            if (cookieOptionsGetter == null)
            {
                throw new ArgumentNullException(nameof(cookieOptionsGetter));
            }

            IResponseCookies responseCookies = ResolveResponseCookies();
            if (responseCookies == null)
            {
                return;
            }

            string cookieValue = cookieValueGetter(viewModel);
            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                return;
            }

            responseCookies.Append(cookieName, cookieValue, cookieOptionsGetter(viewModel));
        }

        private T RestoreFromCookie<T>(string cookieName, Func<string, T> viewModelGetter) where T : IViewModel
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentNullException(nameof(cookieName));
            }
            if (viewModelGetter == null)
            {
                throw new ArgumentNullException(nameof(viewModelGetter));
            }

            string cookieValue = ResolveCookieValue(cookieName);
            if (string.IsNullOrWhiteSpace(cookieValue))
            {
                return default;
            }

            return viewModelGetter(cookieValue);
        }

        private string ResolveCookieValue(string cookieName)
        {
            if (string.IsNullOrWhiteSpace(cookieName))
            {
                throw new ArgumentNullException(nameof(cookieName));
            }

            IRequestCookieCollection requestCookies = ResolveRequestCookies();
            if (requestCookies == null)
            {
                return null;
            }

            if (requestCookies.ContainsKey(cookieName) == false)
            {
                return null;
            }

            return requestCookies[cookieName];
        }

        private IRequestCookieCollection ResolveRequestCookies()
        {
            HttpContext httpContext = ResolveHttpContext();
            if (httpContext == null)
            {
                return null;
            }

            HttpRequest httpRequest = httpContext.Request;
            if (httpRequest == null)
            {
                return null;
            }

            return httpRequest.Cookies;
        }

        private IResponseCookies ResolveResponseCookies()
        {
            HttpContext httpContext = ResolveHttpContext();
            if (httpContext == null)
            {
                return null;
            }

            HttpResponse httpResponse = httpContext.Response;
            if (httpResponse == null)
            {
                return null;
            }

            return httpResponse.Cookies;
        }

        private HttpContext ResolveHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        #endregion
    }
}