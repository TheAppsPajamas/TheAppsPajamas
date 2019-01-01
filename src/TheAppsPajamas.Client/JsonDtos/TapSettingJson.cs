using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.JsonDtos
{
    public class TapSettingJson
    {
        [JsonProperty(Order = 1)]
        public string TapAppId { get; set; }

        [JsonProperty("BuildConfigurations", Order = 2)]
        public IList<BuildConfigJson> BuildConfigs = new List<BuildConfigJson>();

        //{
        //     "LogLevel": "Verbose", "Debug", "Information", "Warn"
        //     "Endpoint": "http://buildapidebug.me"
        //}

        [JsonProperty("LogLevel", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string TapLogLevel { get; set; }

        [JsonProperty("TapEndpoint", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string TapEndpoint { get; set; }

        [JsonProperty("MediaEndpoint", NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public string MediaEndpoint { get; set; }
    }
}
