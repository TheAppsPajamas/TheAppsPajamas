using System;
using Build.Client.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class LoadLocalBuildConfig : BaseLoadTask
    {
        [Output]
        public bool NeedsLoadRemote { get; set; }

        public override bool Execute()
        {
            LogDebug("Running LoadLocalBuildConfig in debug");

            LogDebug("Project name '{0}'", ProjectName);
            LogDebug("Build configuration '{0}'", BuildConfiguration);

            BuildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(BuildResourceDir)){
                NeedsLoadRemote = true;
                Log.LogMessage("Build Resource folder not found, forcing remote load");
                return true;
            }

            //var projectsConfig = this.GetProjectsConfig();

            var projectConfig = this.GetProjectConfigV2();

            if (projectConfig.ClientConfig == null){
                Log.LogMessage("Project {0} in configuration {1} not found, forcing remote load", ProjectName, BuildConfiguration);
                NeedsLoadRemote = true;
                return true;
            }

            PackagingOutput = this.GetPackagingOutput(projectConfig.ClientConfig);
            AppIconOutput = this.GetAppIconOutput(projectConfig.ClientConfig);
            SplashOutput = this.GetSplashOutput(projectConfig.ClientConfig);

            return true;
        }
    }
}
