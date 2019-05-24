using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using TraceIp.Api;
using TraceIp.Model;
using System.Linq;

namespace TraceIp
{
    public static class Helper
    {
        private static readonly GeoCoordinate ROOT_COORDINATE = new GeoCoordinate(-34, -64); // Buenos Aires

        public static double CalculateDistance(List<long> latLong)
        {
            GeoCoordinate coordinate = new GeoCoordinate(latLong[0], latLong[1]);
            double distance = ROOT_COORDINATE.GetDistanceTo(coordinate);
            return Math.Floor(distance / 1000); //In kms
        }

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
