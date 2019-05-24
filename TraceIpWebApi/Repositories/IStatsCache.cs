using System;
using System.Threading.Tasks;
using TraceIp.Model;

namespace TraceIpWebApi.Repositories
{
    public interface IStatsCache
    {
        string GetFarestCountryCode();
        string GetNearestCountryCode();
        long? GetCountryDistance(string countryCode);
        void AddCountryByDistance(string key, double score);
        Task AddCountryByDistanceAsync(string key, double score);

        void UpdateAverageDistance(long average);
        long GetAverageDistance();
    }
}
