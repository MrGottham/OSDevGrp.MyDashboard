namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditAuthenticatedUser : IRedditUser
    {
        bool HasUnreadMail { get; }

        bool HasUnreadModMail { get; }

        int InboxCount { get; }
    }
}