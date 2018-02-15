using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class BuildResourcesConfig
    {
        public int TapAppId { get; set; } = 0;
        public IList<BuildConfig> BuildConfigs = new List<BuildConfig>();
    }
}
