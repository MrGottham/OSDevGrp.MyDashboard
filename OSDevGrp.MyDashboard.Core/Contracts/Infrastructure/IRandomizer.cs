namespace OSDevGrp.MyDashboard.Core.Contracts.Infrastructure
{
    public interface IRandomizer
    {
        int Next(int minValue, int maxValue);
    }
}