using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TraceIp.Model;

namespace TraceIpWebApi.Repositories
{
    public interface ITraceReportRepositorie
    {
        List<String> GetCountries();
        TraceIpReport GetTraceReport(string key);
        Task GetTraceReportAsync(string key);
        void AddTraceReport(string key, string value);
        Task AddTraceReportAsync(string key, string value);
    }

}
