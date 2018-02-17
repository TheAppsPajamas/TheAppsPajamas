using System;
namespace Build.Client.Models
{
    public class BuildConfig
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public bool Disabled { get; set; } = false;

        public BuildConfig(string projectName, string buildConfiguration)
        {
            ProjectName = projectName;
            BuildConfiguration = buildConfiguration;
            Disabled = false;
        }
    }
}
