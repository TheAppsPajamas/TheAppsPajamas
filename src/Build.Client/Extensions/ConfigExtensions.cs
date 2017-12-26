using System;
using System.IO;
using Build.Client.BuildTasks;
using Build.Client.Models;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class ConfigExtensions
    {
        public static string GetBaseDirectory(this BaseTask baseTask){
            baseTask.LogDebug("BuildClientResourceBaseDir located at '{0}'", baseTask.BuildClientResourceBaseDir);


            try
            {
                var buildResourceDir = Path.Combine(baseTask.BuildClientResourceBaseDir, "build-resources");
                if (!Directory.Exists(buildResourceDir))
                {
                    baseTask.LogDebug("Created BuildClientResourceBase at '{0}'", buildResourceDir);
                    Directory.CreateDirectory(buildResourceDir);
                }
                else
                {
                    baseTask.LogDebug("BuildClientResourceDir location at '{0}'", buildResourceDir);
                }
                return buildResourceDir;
            } catch (Exception ex){
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static BuildResourcesConfig GetResourceConfig(this BaseTask baseTask){
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
                        baseTask.LogDebug("Build resources config file read\nAppId '{0}'\nUsername '{1}'", buildResourcesConfig.AppId, buildResourcesConfig.UserName);
                        return buildResourcesConfig;
                    }
                }
            } catch (Exception ex){
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ClientConfigDto GetClientConfig(this BaseTask baseTask, string json){
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
    }
}
