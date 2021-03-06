using System;

namespace OSDevGrp.MyDashboard.Core.Contracts.Models
{
    public interface IRedditAccessToken : IRedditObject
    {
        string AccessToken { get; }

        string TokenType { get; }

        DateTime Expires { get; }

        string Scope { get; }

        string RefreshToken { get; }

        string ToBase64();
    }
}