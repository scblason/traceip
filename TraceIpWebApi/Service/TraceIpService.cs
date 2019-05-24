using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TraceIp;
using TraceIp.Api;
using TraceIp.Builders;
using TraceIp.Model;
using TraceIpWebApi.Repositories;

namespace TraceIpWebApi.Service
{
    public class TraceIpService : ITraceIpService
    {
        ITraceIpCache _traceCache;
        IStatsCache _statsCache;

        public TraceIpService(ITraceIpCache traceCache, IStatsCache statsCache)
        {
            this._traceCache = traceCache;
            this._statsCache = statsCache;
        }

        public async Task<TraceIpReport> GetTraceReport(string ip)
        {
            // Start fetching data
            using (ITraceIpApi traceIpApi = new MeliTraceIpApi())
            {
                // Ip2Country API
                Ip2Country ipCountry = await traceIpApi.Ip2Country(ip);
                if (ipCountry == null)
                {
                    // We return null to signal a problem with the Ip or Country
                    return null;
                }

                TraceIpReport report = GetReportFromCache(ipCountry.CountryCode);
                if (report != null)
                {
                    // We increment the amount of "hits" that we got from the Ip and update the stats
                    report.Hits++;
                }
                else
                {
                    // Get country info  from api
                    CountryInfo countryInfo = await traceIpApi.RestCountries(ipCountry.CountryCode);
                    if (countryInfo == null)
                    {
                        // We return null to signal a problem with the Ip or Country Code
                        return null;
                    }

                    // Currency converter - Asynchronus calling, while we calculate distance data
                    Task<string> currencyTask = Helper.CurrencyConverter(traceIpApi, countryInfo.Currencies);
                    double distance = Helper.CalculateDistance(countryInfo.Latlng); // Distance from Buenos Aires

                    // Wait for task to finish
                    currencyTask.Wait();
                    string currenciesRate = currencyTask.Result;

                    // Build MeliTrace report
                    ITraceIpBuilder builder = new MeliTraceIpBuilder();
                    builder.AppendCountry(countryInfo.Name, countryInfo.NativeName, countryInfo.Alpha2Code).
                        AppendCurrencyRate(currenciesRate).
                        AppendDistance(distance).
                        AppendTimeZone(countryInfo.Timezones).
                        AppendLanguage(countryInfo.Languages);

                    // Print report
                    report = builder.TraceIpReport;
                }

                ////////////////////////////////////////////
                // Save / Update to database (Redis)
                UpdateTraceReport(report);

                // Update stats needed by stats services
                UpdateStats(report);

                return report;
            }
        }

        public TraceIpReport GetReportByNearestCountry()
        {
            string countryCode = this._statsCache.GetNearestCountryCode();
            return this._traceCache.GetTraceReport(countryCode);
        }

        public TraceIpReport GetReportByFarestCountry()
        {
            string countryCode = this._statsCache.GetFarestCountryCode();
            return this._traceCache.GetTraceReport(countryCode);
        }

        public long GetAverageDistance()
        {
            return _statsCache.GetAverageDistance();
        }

        private TraceIpReport GetReportFromCache(string countryCode)
        {
            return this._traceCache.GetTraceReport(countryCode);
        }

        private void UpdateTraceReport(TraceIpReport report)
        {
            string jsonReport = JsonConvert.SerializeObject(report);
            _traceCache.AddTraceReport(report.CountryCode, jsonReport);
        }

        private void UpdateStats(TraceIpReport report)
        {
            long? distance = _statsCache.GetCountryDistance(report.CountryCode);
            if (distance == null)
            {
                _statsCache.AddCountryByDistance(report.CountryCode, report.Distance);
            }

            _statsCache.UpdateAverageDistance(_traceCache.CalculateAverageDistance());
        }

    }
}
