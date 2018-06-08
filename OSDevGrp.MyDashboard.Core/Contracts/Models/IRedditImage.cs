using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditImage : IRedditObject
    {
        Uri Url { get; }

        int Width { get; }

        int Height { get; }
    }
}