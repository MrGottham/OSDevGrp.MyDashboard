namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IInformationItem : IDashboardItem
    {
        string Information { get; }

        string Details { get; } 
    }
}