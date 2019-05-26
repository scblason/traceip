using System;
using System.Collections.Generic;
using System.Text;

namespace TraceIp.Model
{
    /**
     * Represents all the information that we need to return to the user about a 
     * country.    
     */
    public class TraceIpReport
    {
        public string CountryName { get; set; }
        public string CountryNativeName { get; set; }
        public string CountryCode { get; set; }
        public List<long> Latlng { get; set; }
        public string Timezones { get; set; }
        public string Languages { get; set; }
        public double Distance { get; set; }
        public string CurrenciesRate { get; set; }
        public long Hits { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Country: ").Append(CountryName).AppendLine();
            builder.Append("ISO Code: ").Append(CountryCode).AppendLine();
            builder.Append("Language: ").Append(Languages).AppendLine();
            builder.Append("Currency: ").Append(CurrenciesRate).AppendLine();
            builder.Append("Time: ").Append(Timezones).AppendLine();
            builder.Append("Distance: ").Append(Distance).AppendLine();
            builder.Append("Hits: ").Append(Hits).AppendLine();

            return builder.ToString();
        }
    }
}
