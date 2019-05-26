using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using TraceIp.Model;
using System.Linq;

namespace TraceIpWebApi.Repositories
{
    /**
     * Repositorie used to store and query statistics of the API
     */
    public class StatsRepositorie : IStatsRepositorie
    {
        private const int STATS_CACHE_DB = 1;
        private const string DISTANCE_SET = "distance";
        private const string AVERAGE_KEY = "average";
        private readonly IConnectionMultiplexer _redis;

        public StatsRepositorie(IConnectionMultiplexer redis)
        {
            this._redis = redis;
        }

        private IDatabase GetDatabase()
        {
            return this._redis.GetDatabase(STATS_CACHE_DB);
        }

        public string GetFarestCountryCode()
        {
            RedisValue[] values = GetFirstReportByRank(Order.Descending);
            if (values != null)
            {
                return values[0];
            }
            else
            {
                return null;
            }
        }

        public string GetNearestCountryCode()
        {
            RedisValue[] values = GetFirstReportByRank(Order.Ascending);
            if (values == null)
            {
                return null;
            }

            // If the nearest country found is Argentina, we filter it and choose the next nearest one.
            string countryCode = values[0];
            if (!String.IsNullOrEmpty(countryCode) && countryCode == "AR")
            {
                return values.Length > 1 ? (string)values[1] : null;
            }
            else
            {
                return countryCode;
            }
        
        }
            
        public void AddCountryByDistance(string key, double score)
        {
            GetDatabase().SortedSetAdd(DISTANCE_SET, key, score);
        }

        public Task AddCountryByDistanceAsync(string key, double score)
        {
            return GetDatabase().SortedSetAddAsync(DISTANCE_SET, key, score);
        }

        private RedisValue[] GetFirstReportByRank(Order order)
        {
            var reportsByRank = GetDatabase().SortedSetRangeByRank(DISTANCE_SET, 0, 1, order);
            if (reportsByRank != null && reportsByRank.Length > 0)
            {
                return reportsByRank;
            }
            return null;
        }

        public long? GetCountryDistance(string countryCode)
        {
            return GetDatabase().SortedSetRank(DISTANCE_SET, countryCode);
        }

        public void UpdateAverageDistance(long average)
        {
            GetDatabase().StringSet(AVERAGE_KEY, average.ToString());
        }

        public long? GetAverageDistance()
        {
            string average = GetDatabase().StringGet(AVERAGE_KEY);
            if (!String.IsNullOrEmpty(average))
                return Convert.ToInt64(average);
            return null;
        }
    }
}
