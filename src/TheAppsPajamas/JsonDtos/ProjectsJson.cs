using System;
using System.Collections.Generic;

namespace TheAppsPajamas.JsonDtos
{
    public class ProjectsJson
    {
        public IList<ProjectJson> Projects { get; set; } = new List<ProjectJson>();
    }
}
