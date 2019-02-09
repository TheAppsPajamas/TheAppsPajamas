using System;
using System.Collections.Generic;
using TheAppsPajamas.Models;

namespace TheAppsPajamas.JsonDtos
{
    public class ProjectJson
    {
        //public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public ClientConfigDto ClientConfig { get; set; } = new ClientConfigDto();
    }
}
