namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditThing : IRedditObject, IIdentifiable
    {
        string FullName { get; }

        string Kind { get; }

        IRedditObject Data { get; }
    }
}