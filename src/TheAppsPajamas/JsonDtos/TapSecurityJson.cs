using System;
using Newtonsoft.Json;

namespace TheAppsPajamas.JsonDtos
{
    public class TapSecurityJson
    {
        [JsonProperty(Order = 1)]
        public string Username { get; set; } = String.Empty;


        [JsonProperty(Order = 2)]
        public string Password { get; set; } = String.Empty;


        [JsonProperty(Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceUserAccessKey { get; set; } = String.Empty;
    }
}
