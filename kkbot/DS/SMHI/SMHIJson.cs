﻿using System;
using System.Collections.Generic;
using System.Globalization;


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var smjson = SMHIJson.FromJson(jsonString);

namespace kkbot.DS.SMHI
{

    // Jag använde mig av " https://app.quicktype.io/?l=csharp " 
    // Klippte ut första ValidTime så det inte blev alldeles för mycket data, 
    // Pejstade in det, och fick massa goa strukturer

    public partial class SMHIJson
    {
        [JsonProperty("approvedTime")]
        public DateTimeOffset ApprovedTime { get; set; }

        [JsonProperty("referenceTime")]
        public DateTimeOffset ReferenceTime { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("timeSeries")]
        public TimeSery[] TimeSeries { get; set; }
    }

    public partial class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public double[][] Coordinates { get; set; }
    }

    public partial class TimeSery
    {
        [JsonProperty("validTime")]
        public DateTimeOffset ValidTime { get; set; }

        [JsonProperty("parameters")]
        public Parameter[] Parameters { get; set; }
    }

    public partial class Parameter
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("levelType")]
        public LevelType LevelType { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("values")]
        public double[] Values { get; set; }
    }

    public enum LevelType { Hl, Hmsl };

    public partial class SMHIJson
    {
        public static SMHIJson FromJson(string json) => JsonConvert.DeserializeObject<SMHIJson>(json, kkbot.DS.SMHI.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SMHIJson self) => JsonConvert.SerializeObject(self, kkbot.DS.SMHI.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                LevelTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class LevelTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(LevelType) || t == typeof(LevelType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "hl":
                    return LevelType.Hl;
                case "hmsl":
                    return LevelType.Hmsl;
            }
            throw new Exception("Cannot unmarshal type LevelType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (LevelType)untypedValue;
            switch (value)
            {
                case LevelType.Hl:
                    serializer.Serialize(writer, "hl");
                    return;
                case LevelType.Hmsl:
                    serializer.Serialize(writer, "hmsl");
                    return;
            }
            throw new Exception("Cannot marshal type LevelType");
        }

        public static readonly LevelTypeConverter Singleton = new LevelTypeConverter();
    }
}

