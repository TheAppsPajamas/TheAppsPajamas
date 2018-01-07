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

        public string BuildConfiguration { get; set; }

        public ITaskItem AssetCatalogueName { get; set; }

        public ITaskItem AppIconCatalogueName { get; set; }

        //we clone into packages/project dir because of weirdness with xamarin actool task
        public string BaseOutputDir { get; set; }

        [Output]
        public ITaskItem[] OutputImageAssets { get; set; }

        [Output]
        public ITaskItem[] OutputITunesArtwork { get; set; }


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

                var outputImageAssets = new List<ITaskItem>();
                var outputITunesArtwork = new List<ITaskItem>();


                LogDebug("Asset catalogue name {0}", AssetCatalogueName.ItemSpec);

                LogDebug("AppIconSet name {0}", AppIconCatalogueName.ItemSpec);

                //create asset catalogue folder, and contents.json
                var assetCatalogueDir = Path.Combine(BaseOutputDir, AssetCatalogueName.ItemSpec);

                if (!Directory.Exists(assetCatalogueDir)){
                    Directory.CreateDirectory(assetCatalogueDir);
                    LogDebug("Created {0} folder at {1}", AssetCatalogueName, assetCatalogueDir);
                } else {
                    Directory.Delete(assetCatalogueDir, true);
                    Directory.CreateDirectory(assetCatalogueDir);
                    LogDebug("Cleaned and Created {0} folder at {1}", AssetCatalogueName, assetCatalogueDir);
                }
                                             
                var assetCatalogueContentsPath = Path.Combine(BaseOutputDir, AssetCatalogueName.ItemSpec, "Contents.json");

                if (!File.Exists(assetCatalogueContentsPath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", assetCatalogueContentsPath);
                    File.WriteAllText(assetCatalogueContentsPath, Consts.AssetCatalogueContents);
                }

                var assetCatalogueItem = new TaskItem(assetCatalogueContentsPath);
                outputImageAssets.Add(assetCatalogueItem);

                LogDebug("Added asset catalogue contents.json to taskitems at path {0}", assetCatalogueItem.ItemSpec);

                //create appiconset folder, and contents.json
                var appIconSetDir = Path.Combine(BaseOutputDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec);

                if (!Directory.Exists(appIconSetDir))
                {
                    Directory.CreateDirectory(appIconSetDir);
                    LogDebug("Created app-icon-set folder at {0}", appIconSetDir);
                }                          

                var appIconSetContentsPath = Path.Combine(BaseOutputDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, "Contents.json");
                var appIconSetItem = new TaskItem(appIconSetContentsPath);
                outputImageAssets.Add(appIconSetItem);

                var appIconSetContents = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconSetDefaultContents);

                LogDebug("Added AppIconSet contents.json to taskitems at path {0}", appIconSetItem.ItemSpec);

                foreach (var field in AppIconFields)
                {
                    //do itunes artwork first
                    if (String.IsNullOrEmpty(field.GetMetadata("idiom"))){

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, String.Concat(field.GetMetadata("MediaName"), ".png"));

                        var filePath = Path.Combine(base.ProjectDir, field.GetMetadata("LogicalName"));
                        File.Copy(existingFilePath, filePath, true);
                        var artworkTaskItem = new TaskItem(field.GetMetadata("LogicalName"));
                        outputITunesArtwork.Add(artworkTaskItem);
                        continue;
                    }

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
                        return false;
                    }
                    else
                    {
                        imageCatalogue.filename = field.GetMetadata("CatalogueName");
                        LogDebug("Set asset catalogue filename to {0}", imageCatalogue.filename);

                        var imageCatalogue2 = appIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata("size")
                                                                   && x.idiom == field.GetMetadata("idiom2")
                                                                   && x.scale == field.GetMetadata("scale"));
                        if (imageCatalogue2 != null){
                            imageCatalogue2.filename = field.GetMetadata("CatalogueName");
                            LogDebug("Set second asset catalogue filename to {0}", imageCatalogue2.filename);
                        }

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, field.GetMetadata("Path"), String.Concat(field.GetMetadata("MediaName"), ".png"));


                        var filePath = Path.Combine(BaseOutputDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        LogDebug("Copying file {0} to {1}", existingFilePath, filePath);
                        File.Copy(existingFilePath, filePath);

                        var logicalFilePath = Path.Combine(BaseOutputDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        LogDebug("Adding file {0} to task items with logical path {1}", filePath, logicalFilePath);

                        var taskItem = new TaskItem(logicalFilePath);
                        outputImageAssets.Add(taskItem);
                    }
                }

                appIconSetContents.images = appIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var appCatalogueOutput = JsonConvert.SerializeObject(appIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                var appCatalogueOutputPath = Path.Combine(BaseOutputDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, "Contents.json");
                LogDebug("Saving AppIcon catalogue to {0}", appCatalogueOutputPath);
                File.WriteAllText(appCatalogueOutputPath, appCatalogueOutput);

                OutputImageAssets = outputImageAssets.ToArray();
                OutputITunesArtwork = outputITunesArtwork.ToArray();
                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
