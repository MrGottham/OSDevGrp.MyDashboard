namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditThing : IRedditObject
    {
        string Identifier { get; }

        string FullName { get; }

        string Kind { get; }

        IRedditObject Data { get; }
    }
}