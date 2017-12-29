namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditUser : IRedditCreatedThing
    {
        int CommentKarma { get; }

        int LinkKarma { get; }

        string UserName { get; }

        bool Over18 { get; }
    }
}