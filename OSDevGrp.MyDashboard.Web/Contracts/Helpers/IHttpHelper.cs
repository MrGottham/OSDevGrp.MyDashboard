using System;
using System.Threading.Tasks;

namespace OSDevGrp.MyDashboard.Web.Contracts.Helpers
{
    public interface IHttpHelper
    {
        Task<byte[]> ReadAsync(Uri url);
    }
}