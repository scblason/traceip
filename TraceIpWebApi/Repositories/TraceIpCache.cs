using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using TraceIp.Model;
using System.Linq;
using System.Collections.Generic;

namespace TraceIpWebApi.Repositories
{
    public class TraceIpCache : ITraceIpCache
    {
        private IConnectionMultiplexer cacheReport;
        private const int TRACE_CACHE_DB = 0;

        public TraceIpCache(IConnectionMultiplexer cache)
        {
            this.cacheReport = cache;
        }

        public TraceIpReport GetTraceReport(string key)
        {
            string report = this.cacheReport.GetDatabase(TRACE_CACHE_DB).StringGet(key);
            return !String.IsNullOrEmpty(report) ? JsonConvert.DeserializeObject<TraceIpReport>(report) : null;
        }

        public Task GetTraceReportAsync(string key)
        {
            return this.cacheReport.GetDatabase(TRACE_CACHE_DB).StringGetAsync(key);
        }

        public void AddTraceReport(string key, string value)
        {
            this.cacheReport.GetDatabase(TRACE_CACHE_DB).StringSet(key, value);
        }

        public Task AddTraceReportAsync(string key, string value)
        {
            return this.cacheReport.GetDatabase(TRACE_CACHE_DB).StringSetAsync(key, value);
        }

        public long GetTraceAverageByDistanceHits()
        {
            long average = 0;
            long totalHits = 0;

            EndPoint[] endpoints = this.cacheReport.GetEndPoints();
            List<RedisKey> keys = this.cacheReport.GetServer(endpoints[0]).Keys().ToList();

            TraceIpReport report;
            foreach (var key in keys)
            {
                report = GetTraceReport(key);
                totalHits += report.Hits;

                average += (long)report.Distance * report.Hits;
            }

            return totalHits > 0 ? (long)(average / totalHits) : 0;
        }
    }
}
