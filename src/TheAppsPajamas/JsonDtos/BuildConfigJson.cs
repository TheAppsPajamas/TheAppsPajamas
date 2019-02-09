using System;
using Newtonsoft.Json;
namespace TheAppsPajamas.JsonDtos
{
    public class BuildConfigJson
    {
        [JsonProperty(Order = 1)]
        public string ProjectName { get; set; }

        [JsonProperty(Order = 2)]
        public string BuildConfiguration { get; set; }

        [JsonProperty(Order = 3)]
        public bool Disabled { get; set; } = false;

        public BuildConfigJson(string projectName, string buildConfiguration)
        {
            ProjectName = projectName;
            BuildConfiguration = buildConfiguration;
            Disabled = false;
        }
    }
}
