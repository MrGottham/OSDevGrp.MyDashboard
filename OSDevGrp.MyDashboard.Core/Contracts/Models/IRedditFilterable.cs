namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditFilterable : IRedditObject
    {
        bool UserBanned { get; }
 
        bool Over18 { get; }
    }
}