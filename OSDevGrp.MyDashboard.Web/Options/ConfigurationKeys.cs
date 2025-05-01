namespace OSDevGrp.MyDashboard.Web.Options;

internal static class ConfigurationKeys
{
    internal static string AuthenticationSectionName => "Authentication";
    internal static string RedditSectionName => "Reddit";

    internal static string RedditClientIdKey => $"{AuthenticationSectionName}:{RedditSectionName}:ClientId";
    internal static string RedditClientSecretKey => $"{AuthenticationSectionName}:{RedditSectionName}:ClientSecret";
}