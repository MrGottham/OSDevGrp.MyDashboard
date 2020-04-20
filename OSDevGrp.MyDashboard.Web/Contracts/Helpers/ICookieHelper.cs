using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Contracts.Helpers
{
    public interface ICookieHelper
    {
        void ToCookie(DashboardSettingsViewModel dashboardSettingsViewModel);

        void ToCookie(DashboardViewModel dashboardViewModel);

        DashboardSettingsViewModel ToDashboardSettingsViewModel();

        DashboardViewModel ToDashboardViewModel();
    }
}