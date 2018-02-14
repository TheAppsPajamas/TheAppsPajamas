using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Build.Client.Models;
using Build.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class LoadExtensions
    {
        public static string GetBuildResourceDir(this BaseTask baseTask)
        {
            baseTask.LogDebug("ProjectDir located at '{0}'", baseTask.ProjectDir);

            try
            {
                var buildResourceDir = Path.Combine(baseTask.ProjectDir, Consts.TheAppsPajamasResourcesDir);
                if (!Directory.Exists(buildResourceDir))
                {
                    baseTask.LogDebug("Created theappspajamas-resources folder at '{0}'", buildResourceDir);
                    Directory.CreateDirectory(buildResourceDir);
                }
                else
                {
                    baseTask.LogDebug("theappspajamas-resources folder location '{0}'", buildResourceDir);
                }
                var directoryInfo = new DirectoryInfo(buildResourceDir);
                return directoryInfo.FullName;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ProjectsConfig GetProjectsConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading projects.config file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);
                ProjectsConfig projectConfigs = null;
                if (!File.Exists(projectsConfigPath))
                {
                    baseTask.Log.LogMessage("Project config file file not found, created at {0}", projectsConfigPath);
                    projectConfigs = new ProjectsConfig();
                    var json = JsonConvert.SerializeObject(projectConfigs, Formatting.Indented);
                    File.WriteAllText(projectsConfigPath, json);
                    return projectConfigs;
                }
                else
                {
                    var json = File.ReadAllText(projectsConfigPath);
                    projectConfigs = JsonConvert.DeserializeObject<ProjectsConfig>(json);
                    return projectConfigs;

                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ProjectConfig GetProjectConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading project.config file");

            try
            {
                var projectConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);
                ProjectConfig projectConfig = null;
                if (!File.Exists(projectConfigPath))
                {
                    baseTask.Log.LogMessage("Project config file file not found, created at {0}", projectConfigPath);
                    projectConfig = new ProjectConfig();
                    var json = JsonConvert.SerializeObject(projectConfig, Formatting.Indented);
                    File.WriteAllText(projectConfigPath, json);
                    return projectConfig;
                }
                else
                {
                    var json = File.ReadAllText(projectConfigPath);
                    projectConfig = JsonConvert.DeserializeObject<ProjectConfig>(json);
                    return projectConfig;

                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static bool SaveProjects(this BaseLoadTask baseTask, ProjectsConfig projectsConfig)
        {
            baseTask.LogDebug("Saving project.config file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);

                baseTask.Log.LogMessage("Saving project config at {0}", projectsConfigPath);
                var json = JsonConvert.SerializeObject(projectsConfig, Formatting.Indented);
                File.WriteAllText(projectsConfigPath, json);

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return true;
        }

        public static ITaskItem GetAssetCatalogueName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            var assetCatalogueField = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAssetCatalogueName.Value);
            if (assetCatalogueField == null || String.IsNullOrEmpty(assetCatalogueField.Value))
            {
                baseTask.Log.LogError("Asset catalogue undefined");
            }

            baseTask.Log.LogMessage("AssetCatalogue name {0}", assetCatalogueField.Value.ApplyXcAssetsExt());
            return new TaskItem(assetCatalogueField.Value.ApplyXcAssetsExt());
        }

        public static ITaskItem GetAppIconCatalogueSetName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {

            var appIconNameField = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAppIconXcAssetsName.Value);
            if (appIconNameField == null || String.IsNullOrEmpty(appIconNameField.Value))
            {
                baseTask.Log.LogError("AppIconSet catalogue name undefined");
            }
            baseTask.Log.LogMessage("AppIconCatalogue name {0}", appIconNameField.Value.ApplyAppiconsetExt());
            return new TaskItem(appIconNameField.Value.ApplyAppiconsetExt());
        }

        public static ITaskItem GetSplashCatalogueSetName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {

            var launchImageCatalogueSetName = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosLaunchImageXcAssetsName.Value);
            if (launchImageCatalogueSetName == null || String.IsNullOrEmpty(launchImageCatalogueSetName.Value))
            {
                baseTask.Log.LogError("LaunchImageCatalogueSet catalogue name undefined");
            }
            baseTask.Log.LogMessage("LaunchImageCatalogueSet name {0}", launchImageCatalogueSetName.Value.ApplyLaunchimageExt());
            return new TaskItem(launchImageCatalogueSetName.Value.ApplyLaunchimageExt());
        }
    }
}
