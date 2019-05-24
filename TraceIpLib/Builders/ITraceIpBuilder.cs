using System;
using System.Collections.Generic;
using TraceIp.Model;

namespace TraceIp.Builders
{

    public interface ITraceIpBuilder
    {
        TraceIpReport TraceIpReport { get; }
        ITraceIpBuilder AppendCountry(string name, string nativeName, string code);
        ITraceIpBuilder AppendCurrencyRate(string currenciesRate);
        ITraceIpBuilder AppendDistance(double distance);
        ITraceIpBuilder AppendTimeZone(List<string> timeZones);
        ITraceIpBuilder AppendLanguage(List<Language> languages);
    }

}
