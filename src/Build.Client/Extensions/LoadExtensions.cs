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

        public static bool SaveProject(this BaseLoadTask baseTask, ProjectConfig projectConfig)
        {
            baseTask.LogDebug("Saving project.config file");

            try
            {
                var projectConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);

                baseTask.Log.LogMessage("Saving project config at {0}", projectConfigPath);
                var json = JsonConvert.SerializeObject(projectConfig, Formatting.Indented);
                File.WriteAllText(projectConfigPath, json);

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return true;
        }

        public static ITaskItem GetAssetCatalogueName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            var assetCatalogue = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAssetCatalogueName.Value);
            if (assetCatalogue == null || String.IsNullOrEmpty(assetCatalogue.Value))
            {
                baseTask.Log.LogError("Asset catalogue undefined");
            }

            baseTask.Log.LogMessage("AssetCatalogue name {0}", assetCatalogue.Value.ApplyXcAssetsExt());
            return new TaskItem(assetCatalogue.Value.ApplyXcAssetsExt());
        }

        public static ITaskItem GetAppIconCatalogueName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {

            var appIconName = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAppIconXcAssetsName.Value);
            if (appIconName == null || String.IsNullOrEmpty(appIconName.Value))
            {
                baseTask.Log.LogError("AppIconSet catalogue name undefined");
            }
            baseTask.Log.LogMessage("AppIconCatalogue name {0}", appIconName.Value.ApplyAppiconsetExt());
            return new TaskItem(appIconName.Value.ApplyAppiconsetExt());
        }
    }
}
