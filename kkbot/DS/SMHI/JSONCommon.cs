using System;
using System.Collections.Generic;
using System.Globalization;    
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;  

namespace kkbot.DS.SMHI
{
    class JSONCommon
    {
    }

    public static class Serialize
    {
        public static string ToJson(this SMHIStationJSon self) => JsonConvert.SerializeObject(self, kkbot.DS.SMHI.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                RelConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }


}
