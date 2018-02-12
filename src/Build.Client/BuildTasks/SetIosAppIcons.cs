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
        //public string BaseOutputDir { get; set; }

        //[Output]
        //public ITaskItem[] OutputImageAssets { get; set; }

        //[Output]
        //public ITaskItem[] OutputITunesArtwork { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Set Ios App Icons started");

            var filesToAddToProject = new List<ITaskItem>();


            var filesToExcludeFromProject = new List<ITaskItem>();

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
                var outputAssetCatalogueDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec);


                if (!Directory.Exists(outputAssetCatalogueDir)){
                    Directory.CreateDirectory(outputAssetCatalogueDir);
                    //Don't need to add this either, it's just contents.json that gets added
                    //filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string>{ {"IncludePath", outputAssetCatalogueDir}}));
                    LogDebug("Created {0} folder at {1}", AssetCatalogueName, outputAssetCatalogueDir);
                } 
                //stop deleting folder that's just confusing
                //else {
                //    Directory.Delete(outputAssetCatalogueDir, true);
                //    Directory.CreateDirectory(outputAssetCatalogueDir);
                //    LogDebug("Cleaned and Created {0} folder at {1}", AssetCatalogueName, outputAssetCatalogueDir);
                //}
                                             
                var assetCatalogueContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, "Contents.json");

                if (!File.Exists(assetCatalogueContentsPath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", assetCatalogueContentsPath);
                    File.WriteAllText(assetCatalogueContentsPath, Consts.AssetCatalogueContents);
                    filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string>{ {"IncludePath", assetCatalogueContentsPath}}));

                }

                var assetCatalogueItem = new TaskItem(assetCatalogueContentsPath);
                outputImageAssets.Add(assetCatalogueItem);

                Log.LogMessage("Saving {1} Contents.json to path {0}", assetCatalogueItem.ItemSpec, AssetCatalogueName.ItemSpec);

                //create appiconset folder, and contents.json
                var appIconSetDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec);

                if (!Directory.Exists(appIconSetDir))
                {
                    Directory.CreateDirectory(appIconSetDir);
                    //don't need to add this folder
                    //filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string> { { "IncludePath", appIconSetDir } }));

                    LogDebug("Created app-icon-set folder at {0}", appIconSetDir);
                }                          

                var appIconSetContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, "Contents.json");
                var appIconSetItem = new TaskItem(appIconSetContentsPath);
                outputImageAssets.Add(appIconSetItem);

                var appIconSetContents = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconSetDefaultContents);

                LogDebug("Added {0} Contents.json at path {0}", appIconSetItem.ItemSpec, AppIconCatalogueName.ItemSpec);

                //TODO if this doesn't exist, need to add it etc
                foreach (var field in AppIconFields)
                {
                    //do itunes artwork first
                    if (String.IsNullOrEmpty(field.GetMetadata("idiom"))){

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, String.Concat(field.GetMetadata("MediaName"), ".png"));

                        var filePath = Path.Combine(base.ProjectDir, field.GetMetadata("LogicalName"));

                        if (!File.Exists(filePath))
                        {
                            filesToAddToProject.Add(new TaskItem("ITunesArtwork", new Dictionary<string, string> { { "IncludePath", filePath } }));

                        }

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


                        var filePath = Path.Combine(ProjectDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        if (!File.Exists(filePath)){
                            filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string> { { "IncludePath", filePath } }));
                          
                        }

                        LogDebug("Copying file {0} to {1}", existingFilePath, filePath);
                        File.Copy(existingFilePath, filePath, true);

                        var logicalFilePath = Path.Combine(ProjectDir, field.GetMetadata("Path"), field.GetMetadata("CatalogueName"));

                        Log.LogMessage("Adding file {0} at path {1}", filePath, logicalFilePath);

                        var taskItem = new TaskItem(logicalFilePath);
                        //outputImageAssets.Add(taskItem);
                    }
                }

                appIconSetContents.images = appIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var appCatalogueOutput = JsonConvert.SerializeObject(appIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });

 
                var appCatalogueOutputPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, "Contents.json");
                if (!File.Exists(appCatalogueOutputPath))
                {                     
                    filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string> { { "IncludePath", appCatalogueOutputPath } }));
                          
                }
                Log.LogMessage("Saving {1} Contents.json to {0}", appCatalogueOutputPath, AppIconCatalogueName.ItemSpec);
                File.WriteAllText(appCatalogueOutputPath, appCatalogueOutput);

                //OutputImageAssets = outputImageAssets.ToArray();
                //OutputITunesArtwork = outputITunesArtwork.ToArray();

                Log.LogMessage("AppIcons wants to add {0} files to the project", filesToAddToProject.Count());

                FilesToAddToProject = filesToAddToProject.ToArray();
                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
