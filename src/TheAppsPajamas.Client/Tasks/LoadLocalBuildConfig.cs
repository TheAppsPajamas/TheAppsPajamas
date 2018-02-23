using System;
using System.Collections.Generic;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Extensions;
using TheAppsPajamas.Client.Models;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TheAppsPajamas.Client.Tasks
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
                LogInformation($"{Consts.TapResourcesDir} folder not found, forcing remote load");
                return true;
            }


            //this is not quite identical in base

            var projectConfigs = this.GetProjectsConfig();
            var projectConfig = projectConfigs.Projects.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration);

            if (projectConfig == null || projectConfig.ClientConfig == null){
                LogInformation($"{BuildConfiguration} configuration not found for project {ProjectName} in {Consts.ProjectsConfig}, forcing remote load");
                NeedsLoadRemote = true;
                return true;
            }

            //this is identical in base

            AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig, TargetFrameworkIdentifier);

            AppIconFieldOutput = this.GetMediaFieldOutput(projectConfig.ClientConfig.AppIcon.Fields, AssetCatalogueName, projectConfig.ClientConfig);

            SplashFieldOutput = this.GetMediaFieldOutput(projectConfig.ClientConfig.Splash.Fields, AssetCatalogueName, projectConfig.ClientConfig);

            PackagingFieldOutput = this.GetStringFieldOutput(projectConfig.ClientConfig.Packaging.Fields);

            return true;
        }
    }
}
