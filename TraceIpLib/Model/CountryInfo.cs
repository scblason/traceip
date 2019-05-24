using System;
using System.Collections.Generic;
using System.Text;

namespace TraceIp.Model
{
    public class CountryInfo
    {
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public List<long> Latlng { get; set; }
        public List<string> Timezones { get; set; }
        public string NativeName { get; set; }
        public List<Currency> Currencies { get; set; }
        public List<Language> Languages { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Country: ").Append(Name).AppendLine();
            builder.Append("ISO Code: ").Append(Alpha2Code).AppendLine();

            return builder.ToString();
        }

    }

    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Language
    {
        public string Iso639_1 { get; set; }
        public string Iso639_2 { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
    }
}
