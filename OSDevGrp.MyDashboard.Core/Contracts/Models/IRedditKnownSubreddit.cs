namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditKnownSubreddit : IRedditObject
    {
        string Name { get; }

        int Rank { get; }
    }
}