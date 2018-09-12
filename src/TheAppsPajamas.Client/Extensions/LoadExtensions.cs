using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Models;
using TheAppsPajamas.Client.JsonDtos;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.Extensions
{
    public static class LoadExtensions
    {
        public static string GetAssetDir(this BaseTask baseTask)
        {
            baseTask.LogDebug("ProjectDir located at {0}", baseTask.ProjectDir);

            try
            {
                var tapAssetDir = Path.Combine(baseTask.ProjectDir, Consts.TapAssetsDir);
                if (!Directory.Exists(tapAssetDir))
                {
                    baseTask.LogDebug($"Created {Consts.TapAssetsDir} folder at {tapAssetDir}");
                    Directory.CreateDirectory(tapAssetDir);
                }
                else
                {
                    baseTask.LogDebug($"{Consts.TapAssetsDir} folder location {tapAssetDir}");
                }
                var directoryInfo = new DirectoryInfo(tapAssetDir);
                return directoryInfo.FullName;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ProjectsJson GetProjectsConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug($"Loading {Consts.ProjectsFile} file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.TapAssetDir, Consts.ProjectsFile);
                ProjectsJson projectConfigs = null;
                if (!File.Exists(projectsConfigPath))
                {
                    baseTask.LogInformation($"{Consts.ProjectsFile} file file not found, created at {projectsConfigPath}");
                    projectConfigs = new ProjectsJson();
                    var json = JsonConvert.SerializeObject(projectConfigs, Formatting.Indented);
                    File.WriteAllText(projectsConfigPath, json);
                    return projectConfigs;
                }
                else
                {
                    var json = File.ReadAllText(projectsConfigPath);
                    projectConfigs = JsonConvert.DeserializeObject<ProjectsJson>(json);
                    return projectConfigs;

                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }


        public static bool SaveProjects(this BaseLoadTask baseTask, ProjectsJson projectsConfig)
        {
            baseTask.LogDebug($"Saving {Consts.ProjectsFile} file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.TapAssetDir, Consts.ProjectsFile);

                baseTask.LogInformation($"Saving {Consts.ProjectsFile} at {projectsConfigPath}");
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
