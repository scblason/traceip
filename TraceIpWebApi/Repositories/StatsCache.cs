using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using TraceIp.Model;
using System.Linq;

namespace TraceIpWebApi.Repositories
{
    public class StatsCache : IStatsCache
    {
        private const int STATS_CACHE_DB = 1;
        private const string DISTANCE_SET = "distance";
        private const string AVERAGE_KEY = "average";
        private readonly IConnectionMultiplexer cacheStats;

        public StatsCache(IConnectionMultiplexer cache)
        {
            this.cacheStats = cache;
        }

        public string GetFarestCountryCode()
        {
            return GetFirstReportByRank(Order.Descending);
        }

        public string GetNearestCountryCode()
        {
            return GetFirstReportByRank(Order.Ascending);
        }
            
        public void AddCountryByDistance(string key, double score)
        {
            this.cacheStats.GetDatabase(STATS_CACHE_DB).SortedSetAdd(DISTANCE_SET, key, score);
        }

        public Task AddCountryByDistanceAsync(string key, double score)
        {
            return this.cacheStats.GetDatabase(STATS_CACHE_DB).SortedSetAddAsync(DISTANCE_SET, key, score);
        }

        private string GetFirstReportByRank(Order order)
        {
            var reportsByRank = this.cacheStats.GetDatabase(STATS_CACHE_DB).SortedSetRangeByRank(DISTANCE_SET, 0, 0, order);
            if (reportsByRank != null && reportsByRank.Length > 0)
            {
                return reportsByRank[0];
            }
            return null;
        }

        public long? GetCountryDistance(string countryCode)
        {
            return this.cacheStats.GetDatabase(STATS_CACHE_DB).SortedSetRank(DISTANCE_SET, countryCode);
        }

        public void UpdateAverageDistance(long average)
        {
            this.cacheStats.GetDatabase(STATS_CACHE_DB).StringSet(AVERAGE_KEY, average.ToString());
        }

        public long GetAverageDistance()
        {
            string average = this.cacheStats.GetDatabase(STATS_CACHE_DB).StringGet(AVERAGE_KEY);
            if (!String.IsNullOrEmpty(average))
                return Convert.ToInt64(average);
            return 0;
        }
    }
}
