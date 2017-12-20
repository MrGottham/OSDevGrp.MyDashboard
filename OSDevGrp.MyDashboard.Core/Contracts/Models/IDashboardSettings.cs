namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IDashboardSettings
    {
        int NumberOfNews { get; set; }

        bool UseReddit { get; set; }

        IRedditAccessToken RedditAccessToken { get; set; }
    }
}