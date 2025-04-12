using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSDevGrp.MyDashboard.Core.Utilities;
using OSDevGrp.MyDashboard.Web.Contracts.Helpers;
using OSDevGrp.MyDashboard.Web.Models;
using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace OSDevGrp.MyDashboard.Web.Helpers
{
    public class ContentHelper : IContentHelper
    {
        #region Private variables

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;

        #endregion

        #region Constructor

        public ContentHelper(IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            _dataProtectionProvider = dataProtectionProvider;
            _httpContextAccessor = httpContextAccessor;
            _urlHelper = urlHelper;
        }

        #endregion

        #region Methods

        public byte[] ToByteArray(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            IDataProtector dataProtector = _dataProtectionProvider.CreateProtector("SettingsProtection");

            return dataProtector.Protect(JsonSerialization.ToByteArray(dashboardSettingsViewModel));
        }

        public byte[] ToByteArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            IDataProtector dataProtector = _dataProtectionProvider.CreateProtector("ValueProtection");

            return dataProtector.Protect(Encoding.UTF8.GetBytes(value));
        }

        public string ToBase64String(DashboardSettingsViewModel dashboardSettingsViewModel)
        {
            if (dashboardSettingsViewModel == null)
            {
                throw new ArgumentNullException(nameof(dashboardSettingsViewModel));
            }

            return Convert.ToBase64String(ToByteArray(dashboardSettingsViewModel));
        }

        public string ToBase64String(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Convert.ToBase64String(ToByteArray(value));
        }

        public DashboardSettingsViewModel ToDashboardSettingsViewModel(byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }

            IDataProtector dataProtector = _dataProtectionProvider.CreateProtector("SettingsProtection");

            try
            {
                return JsonSerialization.FromByteArray<DashboardSettingsViewModel>(dataProtector.Unprotect(byteArray));
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (SerializationException)
            {
                return null;
            }
        }

        public DashboardSettingsViewModel ToDashboardSettingsViewModel(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            try
            {
                return ToDashboardSettingsViewModel(Convert.FromBase64String(base64String));
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public string ToValue(byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException(nameof(byteArray)); 
            }

            IDataProtector dataProtector = _dataProtectionProvider.CreateProtector("ValueProtection");

            try
            {
                return Encoding.UTF8.GetString(dataProtector.Unprotect(byteArray));
            }
            catch (CryptographicException)
            {
                return null;
            }
        }

        public string ToValue(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            try
            {
                return ToValue(Convert.FromBase64String(base64String));
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public string AbsoluteUrl(string action, string controller)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (string.IsNullOrWhiteSpace(controller))
            {
                throw new ArgumentNullException(nameof(controller));
            }

            string baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            return $"{baseUrl}{GetActionUrl(() => _urlHelper.Action(action, controller))}";
        }

        public string AbsoluteUrl(string action, string controller, object values)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (string.IsNullOrWhiteSpace(controller))
            {
                throw new ArgumentNullException(nameof(controller));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            string baseUrl = GetBaseUrl();
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            return $"{baseUrl}{GetActionUrl(() => _urlHelper.Action(action, controller, values))}";
        }

        private string GetBaseUrl()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            HttpRequest httpRequest = httpContext.Request;
            string scheme = httpRequest.Scheme;
            if (httpRequest.IsHttps && scheme.ToLower().EndsWith("s") == false)
            {
                scheme = scheme + "s";
            }

            return $"{scheme}://{httpRequest.Host}{httpRequest.PathBase}";
        }

        private string GetActionUrl(Func<string> actionUrlGetter)
        {
            if (actionUrlGetter == null)
            {
                throw new ArgumentNullException(nameof(actionUrlGetter));
            }

            string actionUrl = actionUrlGetter();
            if (string.IsNullOrWhiteSpace(actionUrl))
            {
                return null;
            }

            return actionUrl.StartsWith("~/") ? actionUrl.Substring(1) : actionUrl;
        }

        #endregion
    }
}