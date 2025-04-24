using Microsoft.Extensions.Configuration;

namespace OSDevGrp.MyDashboard.Web.Tests.Options.Configuration;

public abstract class ConfigurationTestBase
{
    #region Methods

    protected static IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets<ConfigurationTestBase>()
            .Build();
    }

    #endregion
}