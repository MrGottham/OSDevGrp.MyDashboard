using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Contracts.Helpers
{
    public interface IContentHelper
    {
        byte[] ToByteArray(DashboardSettingsViewModel dashboardSettingsViewModel);

        byte[] ToByteArray(string value);

        string ToBase64String(DashboardSettingsViewModel dashboardSettingsViewModel);

        string ToBase64String(string value);

        DashboardSettingsViewModel ToDashboardSettingsViewModel(byte[] byteArray);

        DashboardSettingsViewModel ToDashboardSettingsViewModel(string base64String);

        string ToValue(byte[] byteArray);

        string ToValue(string base64String);

        string AbsoluteUrl(string action, string controller);

        string AbsoluteUrl(string action, string controller, object values);
    }
}