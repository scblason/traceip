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
        readonly ITraceReportRepositorie _traceReportRepositorie;
        readonly IStatsRepositorie _statsRepositorie;

        public TraceIpService(ITraceReportRepositorie traceReportRepositorie, IStatsRepositorie statsRepositorie)
        {
            this._traceReportRepositorie = traceReportRepositorie;
            this._statsRepositorie = statsRepositorie;
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
                if (report == null)
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

                // We increment the amount of "hits" that we got from the Ip and update the stats
                report.Hits++;

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
            string countryCode = this._statsRepositorie.GetNearestCountryCode();
            return this._traceReportRepositorie.GetTraceReport(countryCode);
        }

        public TraceIpReport GetReportByFarestCountry()
        {
            string countryCode = this._statsRepositorie.GetFarestCountryCode();
            return this._traceReportRepositorie.GetTraceReport(countryCode);
        }

        public long? GetAverageDistance()
        {
            return _statsRepositorie.GetAverageDistance();
        }

        private TraceIpReport GetReportFromCache(string countryCode)
        {
            return this._traceReportRepositorie.GetTraceReport(countryCode);
        }

        private void UpdateTraceReport(TraceIpReport report)
        {
            string jsonReport = JsonConvert.SerializeObject(report);
            _traceReportRepositorie.AddTraceReport(report.CountryCode, jsonReport);
        }

        private void UpdateStats(TraceIpReport report)
        {
            long? distance = _statsRepositorie.GetCountryDistance(report.CountryCode);
            if (distance == null)
            {
                _statsRepositorie.AddCountryByDistance(report.CountryCode, report.Distance);
            }

            _statsRepositorie.UpdateAverageDistance(_traceReportRepositorie.CalculateAverageDistance());
        }

    }
}
