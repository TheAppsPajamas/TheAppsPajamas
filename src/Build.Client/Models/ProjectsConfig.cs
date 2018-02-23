using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Client.Models
{
    public class ProjectsConfig
    {
        public IList<ProjectConfig> Projects { get; set; } = new List<ProjectConfig>();
    }
}
