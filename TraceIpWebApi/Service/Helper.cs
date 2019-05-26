using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using TraceIp.Model;
using System.Linq;
using TraceIpWebApi.Api;

namespace TraceIpWebApi.Service
{
    public static class Helper
    {
        private static readonly GeoCoordinate ROOT_COORDINATE = new GeoCoordinate(-34, -64); // Buenos Aires

        /**
         * We return the distance from latLong parameter to ROOT_COORDINATE (by definition, is Buenos Aires)
         */
        public static long CalculateDistance(List<long> latLong)
        {
            GeoCoordinate coordinate = new GeoCoordinate(latLong[0], latLong[1]);
            double distance = ROOT_COORDINATE.GetDistanceTo(coordinate);
            return (long)(distance / 1000); //In kms, and we use the floor to round up
        }

        /**
         * Helper method to get (asynchronously) the current exchange rate from a list of currencies.
         */
        public static async Task<string> CurrencyConverter(ITraceIpApi traceIpApi, List<Currency> currencies)
        {
            string currenciesRates = "";
            List<Task<CurrencyInfo>> tasks = new List<Task<CurrencyInfo>>();

            foreach (Currency currency in currencies)
            {
                Task<CurrencyInfo> currencyInfo = traceIpApi.ExchangeRate(currency.Code);
                tasks.Add(currencyInfo);
            }

            List<CurrencyInfo> responses = (await Task.WhenAll(tasks)).ToList();

            if (responses != null)
            {
                currenciesRates = responses.Aggregate(new StringBuilder(),
                      (sb, currencyInfo) =>
                      {
                          if (currencyInfo != null && currencyInfo.Rates != null)
                              return sb.AppendFormat("{0} (1 {0} = {1} U$S),", currencyInfo.Code, currencyInfo.Rates["USD"]);
                          else
                              return sb.Append("No currency rate availble");
                      },
                      sb => sb.ToString());
            }
            return currenciesRates;
        }

    }
}
