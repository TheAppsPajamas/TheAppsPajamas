using System;
using System.Collections.Generic;

namespace Build.Client.Models
{
    public class ProjectsConfig
    {
        public IList<ProjectConfig> Projects { get; set; } = new List<ProjectConfig>();
    }
}
