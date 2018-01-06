using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.Constants;
using Build.Client.Extensions;
using Build.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.BuildTasks
{
    public class SetIosAppIcons : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        [Output]
        public ITaskItem[] OutputFiles { get; set; }


        public string BuildConfiguration { get; set; }
        public override bool Execute()
        {
            Log.LogMessage("Set Ios App Icons started");

            try
            {
                var mediaResourcesBuildConfigDir = this.GetMediaResourceDir(BuildConfiguration);

                var assetCatalogoutOutputDir = this.GetAssetCatalogueOutputDir();

                var outputFiles = new List<ITaskItem>();

                var firstField = AppIconFields.FirstOrDefault();
                if (firstField == null)
                {
                    Log.LogError("App Icon Field set malformed");
                }

                var assetCatalogueDir = Path.Combine(assetCatalogoutOutputDir, firstField.GetMetadata("AssetCatalogueName"));
                if (!Directory.Exists(assetCatalogueDir)){
                    Directory.CreateDirectory(assetCatalogueDir);
                    LogDebug("Created asset-catalogue folder at {0}", assetCatalogueDir);
                } else {
                    Directory.Delete(assetCatalogueDir, true);
                    Directory.CreateDirectory(assetCatalogueDir);
                }
                                                     

                var assetCataloguePath = Path.Combine(assetCatalogoutOutputDir, firstField.GetMetadata("AssetCatalogueName"), "Contents.json");

                if (!File.Exists(assetCataloguePath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", assetCataloguePath);
                    File.WriteAllText(assetCataloguePath, Consts.AssetCatalogueContents);
                }

                //var catMetadata = new Dictionary<string, string>();
                //catMetadata.Add("LogicalName", Path.Combine(firstField.GetMetadata("AssetCatalogueName"), "Contents.json"));
                var assetCatalogueItemPath = Path.Combine(Consts.BuildResourcesDir, Consts.AssetCatalogueOutputDir, firstField.GetMetadata("AssetCatalogueName"), "Contents.json");
                var catItem = new TaskItem(assetCatalogueItemPath);//, catMetadata);
                outputFiles.Add(catItem);

                LogDebug("Added asset catalogue contents.json to taskitems at path {0}", catItem.ItemSpec);

                var appIconSetDir = Path.Combine(assetCatalogoutOutputDir, firstField.GetMetadata("AssetCatalogueName"), firstField.GetMetadata("AppIconSetName"));

                if (!Directory.Exists(appIconSetDir))
                {
                    Directory.CreateDirectory(appIconSetDir);
                    LogDebug("Created app-icon-set folder at {0}", appIconSetDir);
                }                          

                var appIconSet = Path.Combine(Consts.BuildResourcesDir, Consts.AssetCatalogueOutputDir, firstField.GetMetadata("AssetCatalogueName"), firstField.GetMetadata("AppIconSetName"), "Contents.json");
                //var appIconSetMetadata = new Dictionary<string, string>();
                //appIconSetMetadata.Add("LogicalName", Path.Combine(firstField.GetMetadata("AssetCatalogueName"), firstField.GetMetadata("AppIconSetName"), "Contents.json"));
                var appIconSetItem = new TaskItem(appIconSet);//, appIconSetMetadata);
                outputFiles.Add(appIconSetItem);

                LogDebug("Added AppIconSet contents.json to taskitems at path {0}", catItem.ItemSpec);

                var appIconCatalogue = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconCatalogueDefaultContents);

                foreach (var field in AppIconFields)
                {


                    var imageCatalogue = appIconCatalogue.images.FirstOrDefault(x => x.size == field.GetMetadata("size")
                                                                   && x.idiom == field.GetMetadata("idiom")
                                                                   && x.scale == field.GetMetadata("scale"));
                    if (imageCatalogue == null)
                    {
                        Log.LogWarning("Image catalogue not found for field {0}, size {1}, idiom {2}, scale {3}"
                                     , field.GetMetadata("LogicalName")
                                     , field.GetMetadata("size")
                                     , field.GetMetadata("idiom")
                                     , field.GetMetadata("scale"));
                        //return false;
                    }
                    else
                    {

                        imageCatalogue.filename = field.GetMetadata("NewCatalogueName");
                        LogDebug("Set asset catalogue filename to {0}", imageCatalogue.filename);

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, field.GetMetadata("TestPath"), String.Concat(field.GetMetadata("MediaName"), ".png"));


                        var filePath = Path.Combine(assetCatalogoutOutputDir, field.GetMetadata("TestPath"), field.GetMetadata("NewCatalogueName"));

                        LogDebug("Copying file {0} to {1}", existingFilePath, filePath);
                        File.Copy(existingFilePath, filePath);

                        var itemMetadata = new Dictionary<string, string>();
                        itemMetadata.Add("DefiningProjectFullPath", base.ProjectDir);

                        //LogDebug("Added Logical file {0} to processing list from media-resource source {1}", field.GetMetadata("LogicalName"), filePath);

                        var logicalFilePath = Path.Combine(Consts.BuildResourcesDir, Consts.AssetCatalogueOutputDir, field.GetMetadata("TestPath"), field.GetMetadata("NewCatalogueName"));

                        LogDebug("Adding file {0} to task items with logical path {1}", filePath, logicalFilePath);

                        var taskItem = new TaskItem(logicalFilePath);//, itemMetadata);
                        outputFiles.Add(taskItem);
                    }
                }

                appIconCatalogue.images = appIconCatalogue.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var appCatalogueOutput = JsonConvert.SerializeObject(appIconCatalogue, Formatting.Indented);
                var appCatalogueOutputPath = Path.Combine(assetCatalogoutOutputDir, firstField.GetMetadata("AssetCatalogueName"), firstField.GetMetadata("AppIconSetName"), "Contents.json");
                LogDebug("Saving AppIcon catalogue to {0}", appCatalogueOutputPath);
                File.WriteAllText(appCatalogueOutputPath, appCatalogueOutput);

                OutputFiles = outputFiles.ToArray();
                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
