using System;
using System.Collections.Generic;
using TheAppsPajamas.Client.Models;

namespace TheAppsPajamas.Client.JsonDtos
{
    public class ProjectJson
    {
        //public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public ClientConfigDto ClientConfig { get; set; } = new ClientConfigDto();
    }
}
