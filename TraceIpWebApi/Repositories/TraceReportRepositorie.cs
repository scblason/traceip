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
    /**
     * Repositorie to get all information related to the countries.
     */
    public class TraceReportRepositorie : ITraceReportRepositorie
    {
        private readonly IConnectionMultiplexer _redis;
        private const int TRACE_CACHE_DB = 0;

        public TraceReportRepositorie(IConnectionMultiplexer redis)
        {
            this._redis = redis;
        }

        private IDatabase GetDatabase()
        {
            return this._redis.GetDatabase(TRACE_CACHE_DB);
        }

        public TraceIpReport GetTraceReport(string key)
        {
            string report = GetDatabase().StringGet(key);
            return !String.IsNullOrEmpty(report) ? JsonConvert.DeserializeObject<TraceIpReport>(report) : null;
        }

        public Task GetTraceReportAsync(string key)
        {
            return GetDatabase().StringGetAsync(key);
        }

        public void AddTraceReport(string key, string value)
        {
            GetDatabase().StringSet(key, value);
        }

        public Task AddTraceReportAsync(string key, string value)
        {
            return GetDatabase().StringSetAsync(key, value);
        }

        public List<String> GetCountries()
        {
            EndPoint[] endpoints = this._redis.GetEndPoints();
            List<RedisKey> keys = this._redis.GetServer(endpoints[0]).Keys().ToList();
            return keys.Select(k => (string)k).ToList();
        }
    }
}
