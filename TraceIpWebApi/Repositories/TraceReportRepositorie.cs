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
        private IConnectionMultiplexer _redis;
        private const int TRACE_CACHE_DB = 0;

        public TraceReportRepositorie(IConnectionMultiplexer redis)
        {
            this._redis = redis;
        }

        public TraceIpReport GetTraceReport(string key)
        {
            string report = this._redis.GetDatabase(TRACE_CACHE_DB).StringGet(key);
            return !String.IsNullOrEmpty(report) ? JsonConvert.DeserializeObject<TraceIpReport>(report) : null;
        }

        public Task GetTraceReportAsync(string key)
        {
            return this._redis.GetDatabase(TRACE_CACHE_DB).StringGetAsync(key);
        }

        public void AddTraceReport(string key, string value)
        {
            this._redis.GetDatabase(TRACE_CACHE_DB).StringSet(key, value);
        }

        public Task AddTraceReportAsync(string key, string value)
        {
            return this._redis.GetDatabase(TRACE_CACHE_DB).StringSetAsync(key, value);
        }

        public List<String> GetCountries()
        {
            EndPoint[] endpoints = this._redis.GetEndPoints();
            List<RedisKey> keys = this._redis.GetServer(endpoints[0]).Keys().ToList();
            return keys.Select(k => (string)k).ToList();
        }
    }
}
