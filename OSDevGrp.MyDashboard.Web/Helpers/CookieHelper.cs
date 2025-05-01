using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;
using System;

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
                _contentHelper.ToBase64String,
                (viewModel, secureHttpRequest) => 
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
                        Secure = secureHttpRequest,
                        SameSite = SameSiteMode.Strict
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
                    string memoryCacheKey = $"{DashboardViewModel.CookieName}.{Guid.NewGuid():D}";

                    using (ICacheEntry cacheEntry = _memoryCache.CreateEntry(memoryCacheKey))
                    {
                        cacheEntry.Value = viewModel;
                        cacheEntry.AbsoluteExpiration= expires;
                    }

                    return _contentHelper.ToBase64String(memoryCacheKey);
                },
                (_, secureHttpRequest) => new CookieOptions {Expires = expires, Secure = secureHttpRequest, SameSite = SameSiteMode.Strict});
        }

        public DashboardSettingsViewModel ToDashboardSettingsViewModel()
        {
            return RestoreFromCookie(DashboardSettingsViewModel.CookieName, _contentHelper.ToDashboardSettingsViewModel);
        }

        public DashboardViewModel ToDashboardViewModel()
        {
            return RestoreFromCookie(DashboardViewModel.CookieName, cookieValue => 
            {
                string memoryCacheKey = _contentHelper.ToValue(cookieValue);
                if (string.IsNullOrWhiteSpace(memoryCacheKey))
                {
                    return null;
                }

                if (_memoryCache.TryGetValue(memoryCacheKey, out DashboardViewModel memoryCacheValue) == false)
                {
                    return null;
                }

                return memoryCacheValue;
            });
        }

        private void StoreCookie<T>(string cookieName, T viewModel, Func<T, string> cookieValueGetter, Func<T, bool, CookieOptions> cookieOptionsGetter) where T : IViewModel
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

            bool secureHttpRequest = IsHttpRequestSecure();

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

            responseCookies.Append(cookieName, cookieValue, cookieOptionsGetter(viewModel, secureHttpRequest));
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

        private bool IsHttpRequestSecure()
        {
            HttpRequest httpRequest = ResolveHttpRequest();
            if (httpRequest == null)
            {
                return false;
            }

            bool isHttps = httpRequest.IsHttps;
            string scheme = httpRequest.Scheme;

            if (string.IsNullOrWhiteSpace(scheme))
            {
                return isHttps;
            }

            return isHttps || scheme.ToLower().EndsWith("s");
        }

        private IRequestCookieCollection ResolveRequestCookies()
        {
            return ResolveHttpRequest().Cookies;
        }

        private IResponseCookies ResolveResponseCookies()
        {
            return ResolveHttpContext().Response.Cookies;
        }

        private HttpRequest ResolveHttpRequest()
        {
            return ResolveHttpContext().Request;
        }

        private HttpContext ResolveHttpContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        #endregion
    }
}