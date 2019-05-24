using System;
using System.Text;

namespace TraceIp.Model
{
    public class Ip2Country
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Country: ").Append(CountryName).AppendLine();
            builder.Append("ISO Code: ").Append(CountryCode).AppendLine();

            return builder.ToString();
        }
    }
}
