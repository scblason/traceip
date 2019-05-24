using System;
using Newtonsoft.Json.Linq;

namespace TraceIp.Model
{
    public class CurrencyInfo 
    {
        public JObject Rates { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return Rates.ToString();
        }
    }   
}