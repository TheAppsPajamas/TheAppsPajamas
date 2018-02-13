using System;
using System.Linq;
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
            Log.LogMessage("Running LoadLocalBuildConfig");

            LogDebug("Project name '{0}'", ProjectName);
            LogDebug("Build configuration '{0}'", BuildConfiguration);

            BuildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(BuildResourceDir)){
                NeedsLoadRemote = true;
                Log.LogMessage("Build Resource folder not found, forcing remote load");
                return true;
            }

            var projectConfigs = this.GetProjectsConfig();
            var projectConfig = projectConfigs.Projects.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration);

            if (projectConfig == null || projectConfig.ClientConfig == null){
                Log.LogMessage("{1} configuration not found for project {0} in project.config, forcing remote load", ProjectName, BuildConfiguration);
                NeedsLoadRemote = true;
                return true;
            }
            if (TargetFrameworkIdentifier == "Xamarin.iOS")
            {
                AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig);
                AppIconCatalogueName = this.GetAppIconCatalogueName(projectConfig.ClientConfig);

            } 
            PackagingOutput = this.GetPackagingOutput(projectConfig.ClientConfig);
            AppIconOutput = this.GetAppIconOutput(projectConfig.ClientConfig, AssetCatalogueName, AppIconCatalogueName);
            SplashOutput = this.GetSplashOutput(projectConfig.ClientConfig);




            return true;
        }
    }
}
