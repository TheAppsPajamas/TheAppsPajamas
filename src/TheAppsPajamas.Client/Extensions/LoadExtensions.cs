using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Models;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.Extensions
{
    public static class LoadExtensions
    {
        public static string GetTapResourcesDir(this BaseTask baseTask)
        {
            baseTask.LogDebug("ProjectDir located at {0}", baseTask.ProjectDir);

            try
            {
                var tapResourceDir = Path.Combine(baseTask.ProjectDir, Consts.TapResourcesDir);
                if (!Directory.Exists(tapResourceDir))
                {
                    baseTask.LogDebug($"Created {Consts.TapResourcesDir} folder at {tapResourceDir}");
                    Directory.CreateDirectory(tapResourceDir);
                }
                else
                {
                    baseTask.LogDebug($"{Consts.TapResourcesDir} folder location {tapResourceDir}");
                }
                var directoryInfo = new DirectoryInfo(tapResourceDir);
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
            baseTask.LogDebug($"Loading {Consts.ProjectsConfig} file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.TapResourceDir, Consts.ProjectsConfig);
                ProjectsConfig projectConfigs = null;
                if (!File.Exists(projectsConfigPath))
                {
                    baseTask.LogInformation($"{Consts.ProjectsConfig} file file not found, created at {projectsConfigPath}");
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


        public static bool SaveProjects(this BaseLoadTask baseTask, ProjectsConfig projectsConfig)
        {
            baseTask.LogDebug($"Saving {Consts.ProjectsConfig} file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.TapResourceDir, Consts.ProjectsConfig);

                baseTask.LogInformation($"Saving {Consts.ProjectsConfig} at {projectsConfigPath}");
                var json = JsonConvert.SerializeObject(projectsConfig, Formatting.Indented);
                File.WriteAllText(projectsConfigPath, json);

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return true;
        }

        public static ITaskItem GetAssetCatalogueName(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto, string TargetFrameworkIdentifier)
        {
            if (TargetFrameworkIdentifier == "Xamarin.iOS")
            {
                var field = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAssetCatalogueName.Value);
                if (field == null || String.IsNullOrEmpty(field.Value))
                {
                    baseTask.Log.LogError("Asset catalogue undefined");
                }

                baseTask.LogInformation("AssetCatalogue name {0}", field.Value.ApplyXcAssetsExt());
                var taskItem = new TaskItem(field.Value.ApplyXcAssetsExt());

                var fieldType = FieldType.GetAll().FirstOrDefault(x => x.Value == field.FieldId);
                taskItem.SetDisabledMetadata(baseTask, field.Disabled, fieldType.DisplayName);
                return taskItem;
            } else {
                return null;
            }
        }
    }
}
