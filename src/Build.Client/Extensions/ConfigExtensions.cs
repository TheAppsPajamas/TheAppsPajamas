using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Build.Client.Models;
using Build.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class ConfigExtensions
    {
        public static BuildResourcesConfig GetResourceConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading build-resources.config file");

            try
            {
                var buildResourcesConfigPath = Path.Combine(baseTask.PackagesDir, "build-resources.config");
                BuildResourcesConfig buildResourcesConfig = null;
                if (!File.Exists(buildResourcesConfigPath))
                {
                    baseTask.LogDebug("Creating blank build resources config at {0}", buildResourcesConfigPath);
                    buildResourcesConfig = new BuildResourcesConfig();
                    var json = JsonConvert.SerializeObject(buildResourcesConfig, Formatting.Indented);
                    File.WriteAllText(buildResourcesConfigPath, json);
                    baseTask.Log.LogError("Build resources config file not found, created at {0}. Please complete appId and restart build process", buildResourcesConfigPath);
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(buildResourcesConfigPath);
                    buildResourcesConfig = JsonConvert.DeserializeObject<BuildResourcesConfig>(json);

                    if (buildResourcesConfig.AppId == 0)
                    {
                        baseTask.Log.LogError("Build resources config appId is 0, please complete appId at {0} and restart build process", buildResourcesConfigPath);
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug("Build resources config file read from {1}\nAppId '{0}'\n'", buildResourcesConfig.AppId, buildResourcesConfigPath);
                        return buildResourcesConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static SecurityConfig GetSecurityConfig(this BaseTask baseTask)
        {
            baseTask.LogDebug("Loading build-security.config file");

            try
            {
                var buildSecurityConfigPath = Path.Combine(baseTask.PackagesDir, "build-security.config");
                SecurityConfig buildSecurityConfig = null;
                if (!File.Exists(buildSecurityConfigPath))
                {
                    baseTask.LogDebug("Creating blank build security config at {0}", buildSecurityConfigPath);
                    buildSecurityConfig = new SecurityConfig();
                    var json = JsonConvert.SerializeObject(buildSecurityConfig, Formatting.Indented);
                    File.WriteAllText(buildSecurityConfigPath, json);
                    baseTask.Log.LogError("Build security config file not found, created at {0}. Please complete username, and password and restart build process", buildSecurityConfigPath);
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(buildSecurityConfigPath);
                    buildSecurityConfig = JsonConvert.DeserializeObject<SecurityConfig>(json);

                    if (String.IsNullOrEmpty(buildSecurityConfig.UserName) || String.IsNullOrEmpty(buildSecurityConfig.Password))
                    {
                        baseTask.Log.LogError("Build security config username or password is null, please complete username, and password at {0} and restart build process", buildSecurityConfigPath);
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug("Build security config file read from {1}\nUsername '{0}'", buildSecurityConfig.UserName, buildSecurityConfigPath);
                        return buildSecurityConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ClientConfigDto GetClientConfig(this BaseLoadTask baseTask, string json)
        {
            try
            {
                baseTask.LogDebug("Deserializing ClientConfigDto, length '{0}'", json.Length);
                var clientConfigDto = JsonConvert.DeserializeObject<ClientConfigDto>(json);
                baseTask.LogDebug("Deserialized ClientConfigDto, packagingFields: '{0}', appIconFields: '{1}', splashFields: '{2}'"
                                  , clientConfigDto.PackagingFields.Count, clientConfigDto.AppIconFields.Count, clientConfigDto.SplashFields.Count);
                return clientConfigDto;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ITaskItem[] GetPackagingOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            baseTask.LogDebug("Generating Packaging TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in clientConfigDto.PackagingFields)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("Value", field.Value);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.Log.LogMessage("Generated {0} Packaging TaskItems", output.Count);
            return output.ToArray();
        }

        public static ITaskItem[] GetAppIconOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto, ITaskItem assetCatalogueName, ITaskItem appIconCatalogueName)
        {
            baseTask.LogDebug("Generating AppIcon TaskItems");

            var output = new List<ITaskItem>();

            foreach (var field in clientConfigDto.AppIconFields)
            {
                var itemMetadata = new Dictionary<string, string>();
                var fieldType = FieldType.AppIcons().FirstOrDefault(x => x.Value == field.FieldId);
                //string logicalName = String.Empty;
                //string path = String.Empty;
                //string mediaName = String.Empty;
                if (fieldType.ProjectType == ProjectType.Droid)
                {
                    var packagingIcon = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);
                    if (packagingIcon == null || String.IsNullOrEmpty(packagingIcon.Value))
                    {
                        baseTask.Log.LogError("Icon name undefined");
                    }
                    //these ones are required for both
                    itemMetadata.Add(MetadataType.LogicalName, packagingIcon.Value.ApplyPngExt());
                    itemMetadata.Add(MetadataType.Path, Path.Combine(Consts.DroidResources, fieldType.GetMetadata(MetadataType.Folder)));
                    itemMetadata.Add(MetadataType.MediaName, packagingIcon.Value.ApplyFieldId(field));

                    itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.AndroidResource);
                }
                else if (fieldType.ProjectType == ProjectType.Ios)
                {
                    //do iTunesArtWork
                    if (String.IsNullOrEmpty(fieldType.GetMetadata(MetadataType.Idiom)))
                    {
                        itemMetadata.Add(MetadataType.Path, Consts.iTunesArtworkDir);
                        itemMetadata.Add(MetadataType.MediaName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt().ApplyFieldId(field));
                        itemMetadata.Add(MetadataType.LogicalName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt());

                        itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.iTunesArtwork);
                    }
                    else //do asset catalogue
                    {
                        itemMetadata.Add(MetadataType.Path, Path.Combine(assetCatalogueName.ItemSpec, appIconCatalogueName.ItemSpec));
                        itemMetadata.Add(MetadataType.LogicalName, fieldType.GetMetadata(MetadataType.FileName));
                        itemMetadata.Add(MetadataType.MediaName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt().ApplyFieldId(field));

                        itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.ImageAsset);
                        itemMetadata.Add(MetadataType.Size, fieldType.GetMetadata(MetadataType.Size));
                        itemMetadata.Add(MetadataType.Idiom, fieldType.GetMetadata(MetadataType.Idiom));
                        itemMetadata.Add(MetadataType.Idiom2, fieldType.GetMetadata(MetadataType.Idiom2));
                        itemMetadata.Add(MetadataType.Scale, fieldType.GetMetadata(MetadataType.Scale));
                        itemMetadata.Add(MetadataType.CatalogueName, fieldType.GetMetadata(MetadataType.FileName));
                    }
                }
                itemMetadata.Add(MetadataType.MediaFileId, field.Value);
                itemMetadata.Add(MetadataType.Disabled, field.Disabled.ToString());
                itemMetadata.Add(MetadataType.FieldDescription, fieldType.DisplayName);

                var taskItem = new TaskItem(field.FieldId.ToString(), itemMetadata);
                baseTask.LogDebug(GetDebugStringFromTaskItem(taskItem, itemMetadata));
                output.Add(taskItem);
            }

            baseTask.Log.LogMessage("Generated {0} AppIcon TaskItems", output.Count);
            return output.ToArray();
        }

        public static string GetDebugStringFromTaskItem(ITaskItem taskItem, Dictionary<string, string> itemMetadata)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Adding task item with itemspec {0}", taskItem.ItemSpec);


            foreach (var meta in itemMetadata)
            {
                sb.AppendFormat(", Metadata key {0} : value {1}", meta.Key, meta.Value);
            }

            return sb.ToString();

        }

        public static ITaskItem[] GetSplashOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            baseTask.LogDebug("Generating Splash TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in clientConfigDto.SplashFields)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add(MetadataType.MediaFileId, field.Value);
                var fieldType = FieldType.Splash().FirstOrDefault(x => x.Value == field.FieldId);
                if (fieldType.ProjectType == ProjectType.Droid)
                {
                    var packagingIcon = clientConfigDto.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);

                    itemMetadata.Add(MetadataType.LogicalName, Path.Combine(fieldType.GetMetadata(MetadataType.FileName), packagingIcon.Value.ApplyPngExt()));
                    itemMetadata.Add(MetadataType.MediaName, packagingIcon.Value.ApplyFieldId(field));
                    itemMetadata.Add(MetadataType.Path, Path.Combine(Consts.DroidResources, fieldType.GetMetadata(MetadataType.Folder)));
                }
                itemMetadata.Add(MetadataType.FieldDescription, fieldType.DisplayName);
                itemMetadata.Add(MetadataType.Disabled, field.Disabled.ToString());
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.Log.LogMessage("Generated {0} Splash TaskItems", output.Count);
            return output.ToArray();
        }

    }
}
