using System;
using System.Collections.Generic;
using System.Linq;
using Build.Client.Constants;
using Build.Client.Extensions;
using Build.Client.Models;
using Build.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class LoadLocalBuildConfig : BaseLoadTask
    {
        [Output]
        public bool NeedsLoadRemote { get; set; }

        public LoadLocalBuildConfig()
        {
            _taskName = "LoadLocalBuildConfig";
        }


        public override bool Execute()
        {
            var baseResult = base.Execute();
            if (baseResult == false)
            {
                return false;
            }


            TapResourceDir = this.GetTapResourcesDir();
            if (String.IsNullOrEmpty(TapResourceDir)){
                NeedsLoadRemote = true;
                Log.LogMessage($"{Consts.TapResourcesDir} folder not found, forcing remote load");
                return true;
            }


            //this is not quite identical in base

            var projectConfigs = this.GetProjectsConfig();
            var projectConfig = projectConfigs.Projects.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration);

            if (projectConfig == null || projectConfig.ClientConfig == null){
                Log.LogMessage($"{BuildConfiguration} configuration not found for project {ProjectName} in {Consts.ProjectsConfig}, forcing remote load");
                NeedsLoadRemote = true;
                return true;
            }

            //this is identical in base

            AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig, TargetFrameworkIdentifier);

            AppIconOutput = this.GetMediaOutput(projectConfig.ClientConfig.AppIcon.Fields, AssetCatalogueName, projectConfig.ClientConfig);

            SplashOutput = this.GetMediaOutput(projectConfig.ClientConfig.Splash.Fields, AssetCatalogueName, projectConfig.ClientConfig);

            PackagingOutput = this.GetFieldTypeOutput(projectConfig.ClientConfig.Packaging.Fields);

            return true;
        }
    }
}
