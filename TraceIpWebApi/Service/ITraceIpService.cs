using System;
using System.Threading.Tasks;
using TraceIp.Model;

namespace TraceIpWebApi.Service
{
    public interface ITraceIpService
    {
        Task<TraceIpReport> GetTraceReport(string ip);
        TraceIpReport GetReportByNearestCountry();
        TraceIpReport GetReportByFarestCountry();
        long GetAverageDistance();
    }
}
