using System;
using System.Collections.Generic;
using System.Text;
using TraceIp.Model;

namespace TraceIp.Builder
{
    /**
     * Report generator class, used to build the TraceIpReport with all the information 
     * from the country.
     * It uses a Builder pattern to build all the parts of the report.
     */
    public class MeliTraceIpBuilder : ITraceIpBuilder
    {
        protected TraceIpReport traceReport;

        public TraceIpReport TraceIpReport
        {
            get { return traceReport; }
        }

        public MeliTraceIpBuilder()
        {
            this.traceReport = new TraceIpReport();
        }

        public ITraceIpBuilder AppendCountry(string name, string nativeName, string code)
        {
            this.traceReport.CountryName = name;
            this.traceReport.CountryNativeName = nativeName;
            this.traceReport.CountryCode = code;
            return this;
        }

        public ITraceIpBuilder AppendCurrencyRate(string currenciesRate)
        {
            if (String.IsNullOrEmpty(currenciesRate))
            {
                this.traceReport.CurrenciesRate = "Currency data unavailable";
            }
            else
            {
                this.traceReport.CurrenciesRate = currenciesRate;
            }
            return this;
        }

        public ITraceIpBuilder AppendDistance(double distance)
        {
            this.traceReport.Distance = distance;
            return this;
        }

        public ITraceIpBuilder AppendLanguage(List<Language> languages)
        {
            StringBuilder builder = new StringBuilder();
            if (languages != null && languages.Count > 0)
            {
                foreach (var lang in languages)
                {
                    builder.Append(lang.Name).Append(" (").Append(lang.Iso639_1).Append("), ");
                }
            }
            else
            {
                builder.Append("Language data unavailable");
            }
            this.traceReport.Languages = builder.ToString();
            return this;
        }

        public ITraceIpBuilder AppendTimeZone(List<string> timeZones)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var timeZone in timeZones) // i.e: "UTC-hh:mm"
            {
                string timeZoneAux = timeZone.Replace("UTC", "");
                DateTime dateTimeTZ = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(timeZoneAux))
                {
                    string[] hhmm = timeZoneAux.Split(":");
                    dateTimeTZ = DateTime.UtcNow.AddHours(Convert.ToInt32(hhmm[0])).AddMinutes(Convert.ToInt32(hhmm[1]));
                }
                builder.Append(dateTimeTZ.ToShortTimeString()).AppendFormat(" ({0}), ", timeZone);
            }
            this.traceReport.Timezones = builder.ToString();
            return this;
        }
    }
}
