using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TraceIp.Model;

namespace TraceIp.Api
{
    public class MeliTraceIpApi : ITraceIpApi, IDisposable
    {
        private readonly HttpClient client = new HttpClient();

        private readonly string IP2COUNTRY_API = "https://api.ip2country.info/ip";
        private readonly string RESTCOUNTRIES_API = "https://restcountries.eu/rest/v2/alpha";
        private readonly string EXCHANGERATE_API = "https://api.exchangeratesapi.io/latest";

        public MeliTraceIpApi()
        {
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Ip2Country> Ip2Country(string ip)
        {
            String apiUrl = String.Format("{0}?{1}", IP2COUNTRY_API, ip);
            return await CallApi<Ip2Country>(apiUrl);
        }

        public async Task<CountryInfo> RestCountries(string countryCode)
        {
            string filterFields = "name;alpha2Code;latlng;timezones;nativeName;currencies;languages";
            string apiUrl = String.Format("{0}/{1}?fields={2}", RESTCOUNTRIES_API, countryCode, filterFields);
            return await CallApi<CountryInfo>(apiUrl);
        }

        public async Task<CurrencyInfo> ExchangeRate(string currencyCode)
        {
            String apiUrl = String.Format("{0}?base={1}&symbols=USD",EXCHANGERATE_API, currencyCode);
            CurrencyInfo currencyInfo = await CallApi<CurrencyInfo>(apiUrl);
            if(currencyInfo != null)
                currencyInfo.Code = currencyCode;
            return currencyInfo;
        }

        private async Task<T> CallApi<T>(string url) where T : class
        {
            if (!String.IsNullOrEmpty(url))
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(data);
                }
            }
            return null;
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
