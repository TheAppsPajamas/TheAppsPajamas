using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.Models
{
    public class TapSetting
    {
        [JsonProperty(Order = 1)]
        public int TapAppId { get; set; } = 0;

        [JsonProperty("BuildConfigurations", Order = 2)]
        public IList<BuildConfig> BuildConfigs = new List<BuildConfig>();

        //{
        //     "TapLogLevel": "Verbose", "Debug", "Information", "Warn"
        //     "Endpoint": "http://buildapidebug.me"
        //}

        [JsonProperty("LogLevel", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string TapLogLevel { get; set; }

        [JsonProperty("Endpoint", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string Endpoint { get; set; }
    }
}
