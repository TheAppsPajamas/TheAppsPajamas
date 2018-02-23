using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Models;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.Extensions
{
    public static class ConfigExtensions
    {
        public static ITaskItem GetTapConfig(this BaseTask baseTask)
        {
            //baseTask.Log.LogMessage($"Loading {Consts.TapConfig} file");

            try
            {
                var tapConfigPath = Path.Combine(baseTask.PackagesDir, Consts.TapConfig);
                if (!File.Exists(tapConfigPath))
                {
                    var tapConfigTaskItem = new TaskItem(MetadataType.TapConfig);

                    tapConfigTaskItem.SetMetadata(MetadataType.TapEndpoint, Consts.ClientEndpoint);
                    tapConfigTaskItem.SetMetadata(MetadataType.TapLogLevel, TapLogLevel.Information);
     

                    //baseTask.Log.LogMessage($"Tap log level set to {tapConfigTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                    return tapConfigTaskItem;
                }
                else
                {
                    var json = File.ReadAllText(tapConfigPath);
                    var tapConfig = JsonConvert.DeserializeObject<TapConfig>(json);

                    var tapConfigTaskItem = new TaskItem(MetadataType.TapConfig);

                    tapConfigTaskItem.SetMetadata(MetadataType.TapEndpoint, tapConfig.Endpoint);
                    tapConfigTaskItem.SetMetadata(MetadataType.TapLogLevel, tapConfig.TapLogLevel);

                    baseTask.LogInformation($"Tap log level set to {tapConfigTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                    return tapConfigTaskItem;
                }
            }
            catch (Exception ex)
            {
                var tapConfigTaskItem = new TaskItem(MetadataType.TapConfig);

                tapConfigTaskItem.SetMetadata(MetadataType.TapEndpoint, Consts.ClientEndpoint);
                tapConfigTaskItem.SetMetadata(MetadataType.TapLogLevel, TapLogLevel.Information);
                baseTask.Log.LogWarning($"Error trying to read {Consts.TapConfig}, returning defaults");
                baseTask.LogInformation($"Tap log level set to {tapConfigTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                return tapConfigTaskItem;
            }
        }

        public static BuildResourcesConfig GetResourceConfig(this BaseLoadTask baseTask)
        {
            baseTask.Log.LogMessage($"Loading {Consts.TapResourcesConfig} file");

            try
            {
                var buildResourcesConfigPath = Path.Combine(baseTask.PackagesDir, Consts.TapResourcesConfig);
                BuildResourcesConfig buildResourcesConfig = null;
                if (!File.Exists(buildResourcesConfigPath))
                {
                    baseTask.LogDebug($"Creating blank {Consts.TapResourcesConfig} at {buildResourcesConfigPath}");
                    buildResourcesConfig = new BuildResourcesConfig();
                    var json = JsonConvert.SerializeObject(buildResourcesConfig, Formatting.Indented);
                    File.WriteAllText(buildResourcesConfigPath, json);
                    baseTask.Log.LogError($"{Consts.TapResourcesConfig} file not found, created at solution root. Please complete TapAppId and restart build process");
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(buildResourcesConfigPath);
                    buildResourcesConfig = JsonConvert.DeserializeObject<BuildResourcesConfig>(json);

                    if (buildResourcesConfig.TapAppId == 0)
                    {
                        baseTask.Log.LogError($"{Consts.TapResourcesConfig} TapAppId is 0, please complete TapAppId and restart build process");
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug($"{Consts.TapResourcesConfig} file read, TapAppId '{ buildResourcesConfig.TapAppId}");
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

        public static void SaveResourceConfig(this BaseLoadTask baseTask, BuildResourcesConfig buildResourcesConfig)
        {
            baseTask.LogInformation($"Saving {Consts.TapResourcesConfig} file");

            try
            {
                var buildResourcesConfigPath = Path.Combine(baseTask.PackagesDir, Consts.TapResourcesConfig);

                var json = JsonConvert.SerializeObject(buildResourcesConfig, Formatting.Indented);
                File.WriteAllText(buildResourcesConfigPath, json);
                baseTask.LogDebug($"{Consts.TapResourcesConfig} file saved");

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
        }



        public static SecurityConfig GetSecurityConfig(this BaseTask baseTask)
        {
            baseTask.LogDebug($"Loading {Consts.TapSecurityConfig} file");

            try
            {
                var buildSecurityConfigPath = Path.Combine(baseTask.PackagesDir, Consts.TapSecurityConfig);
                SecurityConfig buildSecurityConfig = null;
                if (!File.Exists(buildSecurityConfigPath))
                {
                    baseTask.LogDebug($"Creating empty {Consts.TapSecurityConfig} at {buildSecurityConfigPath}");
                    buildSecurityConfig = new SecurityConfig();
                    var json = JsonConvert.SerializeObject(buildSecurityConfig, Formatting.Indented);
                    File.WriteAllText(buildSecurityConfigPath, json);
                    baseTask.Log.LogError($"{Consts.TapSecurityConfig} file not found, created at {buildSecurityConfigPath}. Please complete username, and password and restart build process");
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(buildSecurityConfigPath);
                    buildSecurityConfig = JsonConvert.DeserializeObject<SecurityConfig>(json);

                    if (String.IsNullOrEmpty(buildSecurityConfig.UserName) || String.IsNullOrEmpty(buildSecurityConfig.Password))
                    {
                        baseTask.Log.LogError($"{Consts.TapSecurityConfig} username or password is null, please complete username, and password at {buildSecurityConfigPath} and restart build process");
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug($"{Consts.TapSecurityConfig} file read from {buildSecurityConfigPath}Username {buildSecurityConfig.UserName}");
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
   //         //try
            //{
                baseTask.LogDebug("Deserializing ClientConfigDto, length '{0}'", json.Length);
                var clientConfigDto = JsonConvert.DeserializeObject<ClientConfigDto>(json);
                baseTask.LogDebug("Deserialized ClientConfigDto, packagingFields: '{0}', appIconFields: '{1}', splashFields: '{2}'"
                                  , clientConfigDto.Packaging.Fields.Count, clientConfigDto.AppIcon.Fields.Count, clientConfigDto.Splash.Fields.Count);
                return clientConfigDto;
            //}
            //catch (Exception ex)
            //{
            //    baseTask.Log.LogErrorFromException(ex);
            //}
            //return null;
        }

        public static ITaskItem[] GetPackagingOutput(this BaseLoadTask baseTask, ClientConfigDto clientConfigDto)
        {
            baseTask.LogDebug("Generating Packaging TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in clientConfigDto.Packaging.Fields)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("Value", field.Value);
                output.Add(new TaskItem(field.FieldId.ToString(), itemMetadata));
            }

            baseTask.LogInformation("Generated {0} Packaging TaskItems", output.Count);
            return output.ToArray();
        }

        public static ITaskItem GetHolderOutput(this BaseLoadTask baseTask, IBaseHolderClientDto holder, string description)
        {
            baseTask.LogDebug($"Generating Holder Output for {description}");

            var taskItem = new TaskItem(MetadataType.FieldHolder);
            taskItem.SetDisabledMetadata(baseTask, holder.Disabled, description);
            return taskItem;
        }
        
        public static ITaskItem[] GetStringFieldOutput<TFieldClientDto>(this BaseLoadTask baseTask, IList<TFieldClientDto> fieldsDto)
            where TFieldClientDto : BaseFieldClientDto
        {
            baseTask.LogDebug("Generating String Field Output TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in fieldsDto)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add(MetadataType.Value, field.Value);
                var taskItem = new TaskItem(field.FieldId.ToString(), itemMetadata);

                var fieldType = FieldType.GetAll().FirstOrDefault(x => x.Value == field.FieldId);
                taskItem.SetDisabledMetadata(baseTask, field.Disabled, fieldType.DisplayName);
                output.Add(taskItem);
            }

            baseTask.LogInformation($"Generated {output.Count} Field Output TaskItems");
            return output.ToArray();
        }


        public static ITaskItem[] GetMediaFieldOutput<TFieldClientDto>(this BaseLoadTask baseTask
                                                                  , IList<TFieldClientDto> fieldsDto
                                                                  , ITaskItem assetCatalogueName
                                                                  , ClientConfigDto clientConfigDto)
            where TFieldClientDto : BaseFieldClientDto
        {
            baseTask.LogDebug("Generating Media Field TaskItems");

            var output = new List<ITaskItem>();

            foreach (var field in fieldsDto)
            {
                var itemMetadata = new Dictionary<string, string>();
                var fieldType = FieldType.GetAll().FirstOrDefault(x => x.Value == field.FieldId);

                if (fieldType == null)
                {
                    throw new Exception($"Missing field type {field.FieldId}");
                }
                if (fieldType.ProjectType == ProjectType.Droid)
                {
                    StringFieldClientDto droidNameField = null;
                    if (fieldType is AppIconFieldType)
                    {
                        droidNameField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);

                    }
                    else if (fieldType is SplashFieldType)
                    {
                        droidNameField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidSplashName.Value);

                    }
                    if (droidNameField == null || String.IsNullOrEmpty(droidNameField.Value))
                    {
                        baseTask.Log.LogError("Droid field name undefined");
                    }
                    //these ones are required for both
                    itemMetadata.Add(MetadataType.LogicalName, droidNameField.Value.ApplyPngExt());
                    itemMetadata.Add(MetadataType.Path, Path.Combine(Consts.DroidResources, fieldType.GetMetadata(MetadataType.Folder)));
                    itemMetadata.Add(MetadataType.MediaName, droidNameField.Value.ApplyFieldId(field));

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
                        //need to seperate out image catalogues here, for launchsets and 
                        string catalogueSetName = null;
                        if (fieldType is AppIconFieldType)
                        {
                            var catalogueSetField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosAppIconXcAssetsName.Value);
                            catalogueSetName = catalogueSetField.Value.ApplyAppiconsetExt();

                        }
                        else if (fieldType is SplashFieldType)
                        {
                            var catalogueSetField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == FieldType.PackagingIosLaunchImageXcAssetsName.Value);
                            catalogueSetName = catalogueSetField.Value.ApplyLaunchimageExt();
                        }

                        if (!String.IsNullOrEmpty(fieldType.GetMetadata(MetadataType.CataloguePackagingFieldId)))
                        {
                            var catalogueSetField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == Int32.Parse(fieldType.GetMetadata(MetadataType.CataloguePackagingFieldId)));
                            catalogueSetName = catalogueSetField.Value.ApplyImageSetExt();
                        }

                        if (String.IsNullOrEmpty(catalogueSetName))
                        {
                            baseTask.Log.LogError("Catalogue set name undefined");
                        }



                        itemMetadata.Add(MetadataType.Path, Path.Combine(assetCatalogueName.ItemSpec, catalogueSetName));
                        itemMetadata.Add(MetadataType.LogicalName, fieldType.GetMetadata(MetadataType.FileName));
                        itemMetadata.Add(MetadataType.MediaName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt().ApplyFieldId(field));

                        itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.ImageAsset);
                        itemMetadata.Add(MetadataType.Size, fieldType.GetMetadata(MetadataType.Size));
                        itemMetadata.Add(MetadataType.Idiom, fieldType.GetMetadata(MetadataType.Idiom));
                        itemMetadata.Add(MetadataType.Idiom2, fieldType.GetMetadata(MetadataType.Idiom2));
                        itemMetadata.Add(MetadataType.Scale, fieldType.GetMetadata(MetadataType.Scale));

                        itemMetadata.Add(MetadataType.Subtype, fieldType.GetMetadata(MetadataType.Subtype));
                        itemMetadata.Add(MetadataType.Extent, fieldType.GetMetadata(MetadataType.Extent));
                        itemMetadata.Add(MetadataType.MinimumSystemVersion, fieldType.GetMetadata(MetadataType.MinimumSystemVersion));
                        itemMetadata.Add(MetadataType.Orientation, fieldType.GetMetadata(MetadataType.Orientation));
                        itemMetadata.Add(MetadataType.ContentsFileName, fieldType.GetMetadata(MetadataType.FileName));

                        //we can use this to build a list to operate on as such
                        itemMetadata.Add(MetadataType.CatalogueSetName, catalogueSetName);
                    }
                }
                itemMetadata.Add(MetadataType.MediaFileId, field.Value);
                itemMetadata.Add(MetadataType.FieldDescription, fieldType.DisplayName);

                var taskItem = new TaskItem(field.FieldId.ToString(), itemMetadata);
                taskItem.SetDisabledMetadata(baseTask, field.Disabled, fieldType.DisplayName);

                baseTask.LogDebug(GetDebugStringFromTaskItem(taskItem, itemMetadata));
                output.Add(taskItem);
            }

            baseTask.LogInformation("Generated {0} Media Field TaskItems", output.Count);
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



    }
}
