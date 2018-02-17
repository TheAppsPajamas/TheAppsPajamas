using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class ProjectConfig
    {
        //public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public ClientConfigDto ClientConfig { get; set; } = new ClientConfigDto();
    }
}
