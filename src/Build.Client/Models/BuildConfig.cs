using System;
namespace Build.Client.Models
{
    public class BuildConfig
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public string TargetFrameworkIdentifier { get; set; }
        public bool Disabled { get; set; } = false;

        public BuildConfig(string projectName, string buildConfiguration, string targetFrameworkIdentifier)
        {
            ProjectName = projectName;
            BuildConfiguration = buildConfiguration;
            TargetFrameworkIdentifier = targetFrameworkIdentifier;
            Disabled = false;
        }
    }
}
