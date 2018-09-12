using System;
using System.Collections.Generic;

namespace TheAppsPajamas.Client.JsonDtos
{
    public class ProjectsJson
    {
        public IList<ProjectJson> Projects { get; set; } = new List<ProjectJson>();
    }
}
