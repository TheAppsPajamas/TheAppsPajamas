using System;
using System.Collections.Generic;
using System.IO;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Build.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class ConfigExtensions
    {
        public static string GetBuildResourceDir(this BaseTask baseTask){
            baseTask.LogDebug("PackagesDir located at '{0}'", baseTask.PackagesDir);

            try
            {
                var buildResourceDir = Path.Combine(baseTask.PackagesDir, Consts.BuildResourcesDir);
                if (!Directory.Exists(buildResourceDir))
                {
                    baseTask.LogDebug("Created Build-Resources folder at '{0}'", buildResourceDir);
                    Directory.CreateDirectory(buildResourceDir);
                }
                else
                {
                    baseTask.LogDebug("Build-Resources folder location '{0}'", buildResourceDir);
                }
                var directoryInfo = new DirectoryInfo(buildResourceDir);
                return directoryInfo.FullName;
            } catch (Exception ex){
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static BuildResourcesConfig GetResourceConfig(this BaseLoadTask baseTask){
            baseTask.LogDebug("Loading build-resources.config file");

            try
            {
                var buildResourcesConfigPath = Path.Combine(Directory.GetParent(baseTask.BuildResourceDir).ToString(), "build-resources.config");
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
                itemMetadata.Add("LogicalName", field.Value);
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
                itemMetadata.Add("Value", field.Value);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.LogDebug("Generated {0} Splash TaskItems", output.Count);
            return output.ToArray();
        }


    }
}
