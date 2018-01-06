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

                var firstField = AppIconFields.FirstOrDefault();
                if (firstField == null)
                {
                    Log.LogError("App Icon Field set malformed");
                }

                var outputFiles = new List<ITaskItem>();

                //we clone into packages/project dir because of weirdness with xamarin actool task
                var baseOutputDir = firstField.GetMetadata("DefiningProjectDirectory");

                var assetCatalogueName = firstField.GetMetadata("AssetCatalogueName");
                LogDebug("Asset catalogue name {0}", assetCatalogueName);

                var appIconSetName = firstField.GetMetadata("AppIconSetName");
                LogDebug("AppIconSet name {0}", appIconSetName);

                //create asset catalogue folder, and contents.json
                var assetCatalogueDir = Path.Combine(baseOutputDir, assetCatalogueName);

                if (!Directory.Exists(assetCatalogueDir)){
                    Directory.CreateDirectory(assetCatalogueDir);
                    LogDebug("Created {0} folder at {1}", assetCatalogueName, assetCatalogueDir);
                } else {
                    Directory.Delete(assetCatalogueDir, true);
                    Directory.CreateDirectory(assetCatalogueDir);
                    LogDebug("Cleaned and Created {0} folder at {1}", assetCatalogueName, assetCatalogueDir);
                }
                                             
                var assetCatalogueContentsPath = Path.Combine(baseOutputDir, assetCatalogueName, "Contents.json");

                if (!File.Exists(assetCatalogueContentsPath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", assetCatalogueContentsPath);
                    File.WriteAllText(assetCatalogueContentsPath, Consts.AssetCatalogueContents);
                }

                var assetCatalogueItem = new TaskItem(assetCatalogueContentsPath);
                outputFiles.Add(assetCatalogueItem);

                LogDebug("Added asset catalogue contents.json to taskitems at path {0}", assetCatalogueItem.ItemSpec);

                //create appiconset folder, and contents.json
                var appIconSetDir = Path.Combine(baseOutputDir, assetCatalogueName, appIconSetName);

                if (!Directory.Exists(appIconSetDir))
                {
                    Directory.CreateDirectory(appIconSetDir);
                    LogDebug("Created app-icon-set folder at {0}", appIconSetDir);
                }                          

                var appIconSetContentsPath = Path.Combine(baseOutputDir, assetCatalogueName, appIconSetName, "Contents.json");
                var appIconSetItem = new TaskItem(appIconSetContentsPath);
                outputFiles.Add(appIconSetItem);

                var appIconSetContents = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconSetDefaultContents);

                LogDebug("Added AppIconSet contents.json to taskitems at path {0}", assetCatalogueItem.ItemSpec);

                foreach (var field in AppIconFields)
                {
                    var imageCatalogue = appIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata("size")
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

                        imageCatalogue.filename = field.GetMetadata("CatalogueName");
                        LogDebug("Set asset catalogue filename to {0}", imageCatalogue.filename);

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, field.GetMetadata("Path"), String.Concat(field.GetMetadata("MediaName"), ".png"));


                        var filePath = Path.Combine(baseOutputDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        LogDebug("Copying file {0} to {1}", existingFilePath, filePath);
                        File.Copy(existingFilePath, filePath);

                        var logicalFilePath = Path.Combine(baseOutputDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        LogDebug("Adding file {0} to task items with logical path {1}", filePath, logicalFilePath);

                        var taskItem = new TaskItem(logicalFilePath);
                        outputFiles.Add(taskItem);
                    }
                }

                appIconSetContents.images = appIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var appCatalogueOutput = JsonConvert.SerializeObject(appIconSetContents, Formatting.Indented);
                var appCatalogueOutputPath = Path.Combine(baseOutputDir, assetCatalogueName, appIconSetName, "Contents.json");
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
