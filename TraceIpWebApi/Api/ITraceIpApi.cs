using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TraceIp.Model;

namespace TraceIpWebApi.Api
{
    public interface ITraceIpApi : IDisposable
    {
        Task<Ip2Country> Ip2Country(string ip);
        Task<CountryInfo> RestCountries(string countryCode);
        Task<CurrencyInfo> ExchangeRate(string currencyCode);
    }
}
