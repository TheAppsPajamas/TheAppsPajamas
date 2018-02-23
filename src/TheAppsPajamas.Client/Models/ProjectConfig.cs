using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Client.Models
{
    public class ProjectConfig
    {
        //public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public ClientConfigDto ClientConfig { get; set; } = new ClientConfigDto();
    }
}
