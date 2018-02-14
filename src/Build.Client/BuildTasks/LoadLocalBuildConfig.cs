using System;
using System.Linq;
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

            StringFieldClientDto packagingAppIconField = null;
            StringFieldClientDto packagingSplashField = null;

            ITaskItem packagingCatalogueSetName = null;
            if (TargetFrameworkIdentifier == "Xamarin.iOS")
            {
                AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig);
                AppIconCatalogueName = this.GetAppIconCatalogueSetName(projectConfig.ClientConfig);
                packagingCatalogueSetName = this.GetSplashCatalogueSetName(projectConfig.ClientConfig);

            }
            else if (TargetFrameworkIdentifier == "MonoAndroid")
            {
                packagingAppIconField = projectConfig.ClientConfig.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);

                packagingSplashField = projectConfig.ClientConfig.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidSplashName.Value);
            }





            AppIconOutput = this.GetMediaOutput(projectConfig.ClientConfig.AppIconFields, AssetCatalogueName, AppIconCatalogueName, packagingAppIconField);

            SplashOutput = this.GetMediaOutput(projectConfig.ClientConfig.SplashFields, AssetCatalogueName, packagingCatalogueSetName, packagingSplashField);

            PackagingOutput = this.GetFieldTypeOutput(projectConfig.ClientConfig.PackagingFields);

            return true;
        }
    }
}
