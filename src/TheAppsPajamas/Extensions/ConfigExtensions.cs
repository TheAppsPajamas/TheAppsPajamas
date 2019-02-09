using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TheAppsPajamas.Tasks;
using TheAppsPajamas.Constants;
using TheAppsPajamas.Models;
using TheAppsPajamas.Shared.Types;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using TheAppsPajamas.JsonDtos;

namespace TheAppsPajamas.Extensions
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Gets the tap config now stored in same json file as BuildConfigurations
        /// </summary>
        /// <returns>The tap config.</returns>
        /// <param name="baseTask">Base task.</param>
        public static ITaskItem GetTapSettings(this BaseTask baseTask)
        {
            try
            {

                var tapSettingsPath = baseTask.FindTopFileInProjectDir(Consts.TapSettingFile);

                if (String.IsNullOrEmpty(tapSettingsPath))
                {
                    var tapSettingTaskItem = new TaskItem(MetadataType.TapConfig);

                    tapSettingTaskItem.SetMetadata(MetadataType.TapEndpoint, Consts.TapEndpoint);
                    tapSettingTaskItem.SetMetadata(MetadataType.MediaEndpoint, Consts.MediaEndpoint);
                    tapSettingTaskItem.SetMetadata(MetadataType.TapLogLevel, LogLevelConsts.Information);

                    //baseTask.Log.LogMessage($"Tap log level set to {tapConfigTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                    return tapSettingTaskItem;
                }
                else
                {
                    var json = File.ReadAllText(tapSettingsPath);
                    var tapSetting = JsonConvert.DeserializeObject<TapSettingJson>(json);

                    var tapSettingTaskItem = new TaskItem(MetadataType.TapConfig);

                    if (!String.IsNullOrEmpty(tapSetting.LogLevel))
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.TapLogLevel, tapSetting.LogLevel);
                    }
                    else
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.TapLogLevel, LogLevelConsts.Information);
                    }

                    if (!String.IsNullOrEmpty(tapSetting.TapEndpoint))
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.TapEndpoint, tapSetting.TapEndpoint);
                    }
                    else
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.TapEndpoint, Consts.TapEndpoint);
                    }
                    if (!String.IsNullOrEmpty(tapSetting.MediaEndpoint))
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.MediaEndpoint, tapSetting.MediaEndpoint);
                    }
                    else
                    {
                        tapSettingTaskItem.SetMetadata(MetadataType.MediaEndpoint, Consts.MediaEndpoint);
                    }
                    baseTask.LogInformation($"Tap log level set to {tapSettingTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                    return tapSettingTaskItem;
                }
            }
            catch
            {
                var tapSettingTaskItem = new TaskItem(MetadataType.TapConfig);

                tapSettingTaskItem.SetMetadata(MetadataType.TapEndpoint, Consts.TapClientEndpoint);
                tapSettingTaskItem.SetMetadata(MetadataType.MediaEndpoint, Consts.MediaEndpoint);
                tapSettingTaskItem.SetMetadata(MetadataType.TapLogLevel, LogLevelConsts.Information);
                baseTask.Log.LogWarning($"Error trying to read {Consts.TapSettingFile}, returning defaults");
                baseTask.LogInformation($"Tap log level set to {tapSettingTaskItem.GetMetadata(MetadataType.TapLogLevel)}");
                return tapSettingTaskItem;
            }
        }

        public static TapSettingJson GetTapSetting(this BaseLoadTask baseTask)
        {
            baseTask.Log.LogMessage($"Loading {Consts.TapSettingFile} file");

            try
            {
                var tapSettingsPath = baseTask.FindTopFileInProjectDir(Consts.TapSettingFile);
                TapSettingJson tapSetting = null;
                if (String.IsNullOrEmpty(tapSettingsPath))
                {
                    baseTask.Log.LogError($"{Consts.TapSettingFile} file not found");
                    return null;
                }
                else
                {
                    //test security file as well
                    baseTask.GetSecurity();
                    var json = File.ReadAllText(tapSettingsPath);
                    tapSetting = JsonConvert.DeserializeObject<TapSettingJson>(json);

                    if (String.IsNullOrEmpty(tapSetting.TapAppId))
                    {
                        baseTask.Log.LogError($"{Consts.TapSettingFile} TapAppId is 0, please complete TapAppId and restart build process");
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug($"{Consts.TapSettingFile} file read, TapAppId '{ tapSetting.TapAppId}");
                        return tapSetting;
                    }
                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static void SaveTapAssetConfig(this BaseLoadTask baseTask, TapSettingJson tapSetting)
        {
            baseTask.LogInformation($"Saving {Consts.TapSettingFile} file");

            try
            {
                var tapSettingsPath = baseTask.FindTopFileInProjectDir(Consts.TapSettingFile);
                var json = JsonConvert.SerializeObject(tapSetting, Formatting.Indented);
                File.WriteAllText(tapSettingsPath, json);
                baseTask.LogDebug($"{Consts.TapSettingFile} file saved");

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
        }

        public static TapSecurityJson GetSecurity(this BaseTask baseTask)
        {
            baseTask.LogDebug($"Loading {Consts.TapSecurityFile} file");

            try
            {
                var tapSecurityPath = baseTask.FindTopFileInProjectDir(Consts.TapSecurityFile);
                TapSecurityJson tapSecurity = null;
                if (String.IsNullOrEmpty(tapSecurityPath))
                {
                    baseTask.Log.LogError($"{Consts.TapSecurityFile} file not found");
                    return null;
                }
                else
                {
                    var json = File.ReadAllText(tapSecurityPath);
                    tapSecurity = JsonConvert.DeserializeObject<TapSecurityJson>(json);

                    if ((String.IsNullOrEmpty(tapSecurity.Username) || String.IsNullOrEmpty(tapSecurity.Password))
                        && String.IsNullOrEmpty(tapSecurity.ServiceUserAccessKey))
                    {
                        baseTask.Log.LogError($"{Consts.TapSecurityFile} username, password, or service user access key is null, please complete username and password, or service user access key at {tapSecurityPath} and restart build process");
                        return null;
                    }
                    else
                    {
                        baseTask.LogDebug($"{Consts.TapSecurityFile} file read from {tapSecurityPath}, Username {tapSecurity.Username}");
                        return tapSecurity;
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
            baseTask.LogDebug("Deserializing ClientConfigDto, length '{0}'", json.Length);
            var clientConfigDto = JsonConvert.DeserializeObject<ClientConfigDto>(json);
            baseTask.LogDebug("Deserialized ClientConfigDto, packagingFields: '{0}', appIconFields: '{1}', splashFields: '{2}'"
                              , clientConfigDto.Packaging.Fields.Count, clientConfigDto.AppIcon.Fields.Count, clientConfigDto.Splash.Fields.Count);
            return clientConfigDto;
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

        public static ITaskItem[] GetStringFieldOutput<TFieldClientDto>(this BaseLoadTask baseTask
                                                                        , IList<TFieldClientDto> fieldsDto
                                                                        , ITaskItem holder)
            where TFieldClientDto : BaseFieldClientDto
        {
            baseTask.LogDebug("Generating String Field Output TaskItems");

            var output = new List<ITaskItem>();
            foreach (var field in fieldsDto)
            {
                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add(MetadataType.Value, field.Value);

                //TODO deal with this in setmanifest
                itemMetadata.Add(MetadataType.FieldHolderDisabled, holder.GetMetadata(MetadataType.Disabled));

                var taskItem = new TaskItem(field.FieldId, itemMetadata);

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
                                                                  , ClientConfigDto clientConfigDto
                                                                      , ITaskItem holder)
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

                itemMetadata.Add(MetadataType.FieldHolderDisabled, holder.GetMetadata(MetadataType.Disabled));

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
                    itemMetadata.Add(MetadataType.TapAssetPath, Path.Combine(Consts.DroidTapAssets, fieldType.GetMetadata(MetadataType.Folder)));
                    itemMetadata.Add(MetadataType.ProjectAssetPath, Path.Combine(Consts.DroidResources, fieldType.GetMetadata(MetadataType.Folder)));
                    itemMetadata.Add(MetadataType.MediaName, droidNameField.Value.ApplyFieldId(field));

                    itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.TapAsset);
                }
                else if (fieldType.ProjectType == ProjectType.Ios)
                {
                    //do iTunesArtWork
                    if (String.IsNullOrEmpty(fieldType.GetMetadata(MetadataType.Idiom)))
                    {
                        itemMetadata.Add(MetadataType.TapAssetPath, Consts.iTunesArtworkDir);
                        itemMetadata.Add(MetadataType.ProjectAssetPath, Consts.iTunesArtworkDir);
                        itemMetadata.Add(MetadataType.MediaName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt().ApplyFieldId(field));
                        itemMetadata.Add(MetadataType.LogicalName, fieldType.GetMetadata(MetadataType.FileName).RemovePngExt());

                        itemMetadata.Add(MetadataType.MSBuildItemType, MSBuildItemName.ITunesArtwork);
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
                            var catalogueSetField = clientConfigDto.Packaging.Fields.FirstOrDefault(x => x.FieldId == fieldType.GetMetadata(MetadataType.CataloguePackagingFieldId));
                            catalogueSetName = catalogueSetField.Value.ApplyImageSetExt();
                        }

                        if (String.IsNullOrEmpty(catalogueSetName))
                        {
                            baseTask.Log.LogError("Catalogue set name undefined");
                        }



                        itemMetadata.Add(MetadataType.TapAssetPath, Path.Combine(assetCatalogueName.ItemSpec, catalogueSetName));
                        itemMetadata.Add(MetadataType.ProjectAssetPath, Path.Combine(assetCatalogueName.ItemSpec, catalogueSetName));
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
