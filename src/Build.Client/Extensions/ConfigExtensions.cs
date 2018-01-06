using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Build.Client.Models;
using DAL.Enums;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class ConfigExtensions
    {
        public static BuildResourcesConfig GetResourceConfig(this BaseLoadTask baseTask){
            baseTask.LogDebug("Loading build-resources.config file");

            try
            {
                var buildResourcesConfigPath = Path.Combine(baseTask.PackagesDir, "build-resources.config");
                BuildResourcesConfig buildResourcesConfig = null;
                if (!File.Exists(buildResourcesConfigPath))
                {
                    baseTask.LogDebug("Creating blank build resources config at {0}", buildResourcesConfigPath);
                    buildResourcesConfig = new BuildResourcesConfig();
                    var json = JsonConvert.SerializeObject(buildResourcesConfig);
                    File.WriteAllText(buildResourcesConfigPath, json);
                    baseTask.Log.LogError("Build resources config file not found, created at {0}. Please complete appId, username, and password and restart build process", buildResourcesConfigPath);
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(buildResourcesConfigPath);
                    buildResourcesConfig = JsonConvert.DeserializeObject<BuildResourcesConfig>(json);

                    if (buildResourcesConfig.AppId == null)
                    {
                        baseTask.Log.LogError("Build resources config appId is null, please complete appId, username, and password at {0} and restart build process", buildResourcesConfigPath);
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug("Build resources config file read from {2}\nAppId '{0}'\nUsername '{1}'", buildResourcesConfig.AppId, buildResourcesConfig.UserName, buildResourcesConfigPath);
                        return buildResourcesConfig;
                    }
                }
            } catch (Exception ex){
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ClientConfigDto GetClientConfig(this BaseLoadTask baseTask, string json){
            try{
                baseTask.LogDebug("Deserializing ClientConfigDto, length '{0}'", json.Length);
                var clientConfigDto = JsonConvert.DeserializeObject<ClientConfigDto>(json);
                baseTask.LogDebug("Deserialized ClientConfigDto, packagingFields: '{0}', appIconFields: '{1}', splashFields: '{2}'"
                                  , clientConfigDto.PackagingFields.Count, clientConfigDto.AppIconFields.Count, clientConfigDto.SplashFields.Count);
                return clientConfigDto;
            } catch (Exception ex){
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ITaskItem[] GetPackagingOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto){
            baseTask.LogDebug("Generating Packaging TaskItems");

            var output = new List<ITaskItem>();
            foreach(var field in clientConfigDto.PackagingFields){
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("Value", field.Value);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.LogDebug("Generated {0} Packaging TaskItems",output.Count);
            return output.ToArray();  
        }

        public static ITaskItem[] GetAppIconOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            baseTask.LogDebug("Generating AppIcon TaskItems");

            var output = new List<ITaskItem>();

            foreach (var field in clientConfigDto.AppIconFields)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("MediaFileId", field.Value);
                var fieldType = FieldType.AppIcons().FirstOrDefault(x => x.Value == field.FieldId);
                string logicalName = String.Empty;
                string path = String.Empty;
                string mediaName = String.Empty;
                if (fieldType.ProjectType == ProjectType.Droid){
                    var packagingIcon = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);
                    if (packagingIcon == null || String.IsNullOrEmpty(packagingIcon.Value))
                    {
                        baseTask.Log.LogError("Icon name undefined");
                    }
                    logicalName = Path.Combine(fieldType.OsFileName, packagingIcon.Value.ApplyPngExt());
                    path = Path.Combine(Consts.DroidResources, fieldType.OsFileName);
                    mediaName = String.Concat(packagingIcon.Value, "_", field.Value);
                } else if (fieldType.ProjectType == ProjectType.Ios){
                    var assetCatalogue = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAssetCatalogueName.Value);
                    if (assetCatalogue == null || String.IsNullOrEmpty(assetCatalogue.Value)){
                        baseTask.Log.LogError("Asset catalogue undefined");
                    }
                    var appIconName = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAppIconXcAssetsName.Value);
                    if (appIconName == null || String.IsNullOrEmpty(appIconName.Value))
                    {
                        baseTask.Log.LogError("AppIconSet catalogue name undefined");
                    }
                    path = Path.Combine(assetCatalogue.Value.ApplyXcAssetsExt(), appIconName.Value.ApplyAppiconsetExt());
                    logicalName = Path.Combine(path, fieldType.OsFileName);
                    mediaName = string.Concat(fieldType.OsFileName.RemovePngExt(), "_", field.Value);
                    itemMetadata.Add("AssetCatalogueName", assetCatalogue.Value.ApplyXcAssetsExt());
                    itemMetadata.Add("AppIconSetName", appIconName.Value.ApplyAppiconsetExt());

                    itemMetadata.Add("size", fieldType.GetMetadata("size"));
                    itemMetadata.Add("idiom", fieldType.GetMetadata("idiom"));
                    itemMetadata.Add("scale", fieldType.GetMetadata("scale"));
                    itemMetadata.Add("CatalogueName", fieldType.OsFileName);
                }
                itemMetadata.Add("Path", path);
                itemMetadata.Add("LogicalName", logicalName);

                itemMetadata.Add("MediaName", mediaName);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.LogDebug("Generated {0} AppIcon TaskItems", output.Count);
            return output.ToArray();
        }

        public static ITaskItem[] GetSplashOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            baseTask.LogDebug("Generating Splash TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in clientConfigDto.SplashFields)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("MediaFileId", field.Value);
                var fieldType = FieldType.Splash().FirstOrDefault(x => x.Value == field.FieldId);
                string logicalName = String.Empty; 
                string path = String.Empty;
                string mediaName = String.Empty;
                if (fieldType.ProjectType == ProjectType.Droid)
                {
                    var packagingIcon = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);

                    logicalName = Path.Combine(fieldType.OsFileName, string.Concat(packagingIcon.Value, ".png"));
                    mediaName = String.Concat(packagingIcon.Value, "_", field.Value);
                    path = Path.Combine(Consts.DroidResources, fieldType.OsFileName);
                } 
                itemMetadata.Add("Path", path);
                itemMetadata.Add("LogicalName", logicalName);
                itemMetadata.Add("MediaName", mediaName);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.LogDebug("Generated {0} Splash TaskItems", output.Count);
            return output.ToArray();
        }

    }
}
