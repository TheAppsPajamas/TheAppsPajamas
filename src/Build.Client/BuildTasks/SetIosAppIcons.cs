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


        public ITaskItem[] ExistingiTunesArtworks { get; set; }

        public ITaskItem[] ExistingImageAssets { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Set Ios App Icons started");

            var filesToAddToModifiedProject = new List<ITaskItem>();

            var existingAssets = new List<ITaskItem>();

            if (ExistingImageAssets != null && ExistingImageAssets.Length != 0)
            {
                existingAssets.AddRange(ExistingImageAssets);
            } else if (ExistingImageAssets == null || ExistingImageAssets.Length == 0){
                LogDebug("No existing image assets found in project");
            }

            if (ExistingiTunesArtworks != null && ExistingiTunesArtworks.Length == 0)
            {
                existingAssets.AddRange(ExistingiTunesArtworks);
            
            }
            else if (ExistingiTunesArtworks == null || ExistingiTunesArtworks.Length == 0)
            {
                LogDebug("No existing iTunesArtwork found in project");
            }

            foreach (var taskItem in existingAssets)
            {
                LogDebug("Existing asset in project {0}", taskItem.ItemSpec);
            }

            try
            {
                var mediaResourcesBuildConfigDir = this.GetMediaResourceDir(BuildConfiguration);

                //could handle disbled here
                var firstField = AppIconFields.FirstOrDefault();
                if (firstField == null)
                {
                    Log.LogError("App Icon Field set malformed");
                }

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

                var mediaResourceAssetCatalogueContentsPath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);
                if (!File.Exists(mediaResourceAssetCatalogueContentsPath))
                {
                    LogDebug("Creating Asset catalogue Contents.json at {0}", mediaResourceAssetCatalogueContentsPath);
                    File.WriteAllText(mediaResourceAssetCatalogueContentsPath, Consts.AssetCatalogueContents);
                }

                var outputAssetCatalogueContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);

                if (!File.Exists(outputAssetCatalogueContentsPath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", outputAssetCatalogueContentsPath);
                    File.WriteAllText(outputAssetCatalogueContentsPath, Consts.AssetCatalogueContents);
                    Log.LogMessage("Saving {1} Contents.json to path {0}", outputAssetCatalogueContentsPath, AssetCatalogueName.ItemSpec);
                }

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == outputAssetCatalogueContentsPath) == null)
                {
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, outputAssetCatalogueContentsPath } }));
                    LogDebug("Adding {0} to add to project list as it is not in current project", outputAssetCatalogueContentsPath);
                }

                //create appiconset folder, and contents.json
                var appIconSetDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec);

                if (!Directory.Exists(appIconSetDir))
                {
                    Directory.CreateDirectory(appIconSetDir);
                    LogDebug("Created app-icon-set folder at {0}", appIconSetDir);
                }                          

                var mediaResourceAppIconSetContentsPath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
                var mediaResourceAppIconSetItem = new TaskItem(mediaResourceAppIconSetContentsPath);

                var mediaResourceAppIconSetContents = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconSetDefaultContents);

                LogDebug("Added media-resource {0} Contents.json at path {0}", mediaResourceAppIconSetItem.ItemSpec, AppIconCatalogueName.ItemSpec);

                var outputAppIconSetContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
                var outputAppIconSetItem = new TaskItem(outputAppIconSetContentsPath);
               
                var outputAppIconSetContents = JsonConvert.DeserializeObject<AppIconAssetCatalogue>(Consts.AppIconSetDefaultContents);

                LogDebug("Added project output {0} Contents.json at path {0}", outputAppIconSetItem.ItemSpec, AppIconCatalogueName.ItemSpec);

                //TODO if this doesn't exist, need to add it etc
                foreach (var field in AppIconFields)
                {
                    //do itunes artwork first
                    if (String.IsNullOrEmpty(field.GetMetadata(MetadataType.Idiom))){

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, Consts.iTunesArtworkDir, field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                        var outputFilePath = Path.Combine(base.ProjectDir, field.GetMetadata(MetadataType.LogicalName));

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == outputFilePath) == null){
                            LogDebug("Adding {0} to add to project list as it is not in current project", outputFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.iTunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, outputFilePath } }));
                        }

                        File.Copy(existingFilePath, outputFilePath, true);
                        var artworkTaskItem = new TaskItem(field.GetMetadata(MetadataType.LogicalName));

                        LogDebug("Added {2} from {0} to {1}", existingFilePath, outputFilePath, MSBuildItemName.iTunesArtwork);

                        continue;
                    }

                    var mediaResourceImageCatalogue = mediaResourceAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                            && x.idiom == field.GetMetadata(MetadataType.Idiom)
                                                                                                            && x.scale == field.GetMetadata(MetadataType.Scale));
                    
                    var outputImageCatalogue = outputAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                              && x.idiom == field.GetMetadata(MetadataType.Idiom)
                                                                                              && x.scale == field.GetMetadata(MetadataType.Scale));
                    if (outputImageCatalogue == null)
                    {
                        Log.LogWarning("Image catalogue not found for field {0}, size {1}, idiom {2}, scale {3}"
                                       , field.GetMetadata(MetadataType.LogicalName)
                                       , field.GetMetadata(MetadataType.Size)
                                       , field.GetMetadata(MetadataType.Idiom)
                                       , field.GetMetadata(MetadataType.Scale));
                        return false;
                    }
                    else
                    {
                        outputImageCatalogue.filename = field.GetMetadata(MetadataType.CatalogueName);
                        mediaResourceImageCatalogue.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                        LogDebug("Set asset catalogue filename to {0}", outputImageCatalogue.filename);

                        //sometimes we use the same image twice in the contents.json
                        var outputImageCatalogue2 = outputAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                        if (outputImageCatalogue2 != null){
                            outputImageCatalogue2.filename = field.GetMetadata(MetadataType.CatalogueName);
                            var mediaResourceImageCatalogue2 = mediaResourceAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                            mediaResourceImageCatalogue2.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                            LogDebug("Set second asset catalogue filename to {0}", outputImageCatalogue2.filename);
                        }

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir
                                                            , field.GetMetadata(MetadataType.Path)
                                                            , field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                        var outputFilePath = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.Path)
                                                          , field.GetMetadata(MetadataType.LogicalName));

                        //we want a list of existing imageassets, and itunesartwork to work of, rather than a test of file existence

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == outputFilePath) == null)
                        {
                            LogDebug("Adding {0} to add to project list as it is not in current project", outputFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, outputFilePath } }));
                          
                        }

                        LogDebug("Copying file {0} to {1}", existingFilePath, outputFilePath);
                        File.Copy(existingFilePath, outputFilePath, true);

                    }
                }

                //mediaresource (output)
                mediaResourceAppIconSetContents.images = mediaResourceAppIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var mediaResourceAppCatalogueJson = JsonConvert.SerializeObject(mediaResourceAppIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                var mediaResourceAppCataloguePath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);

                Log.LogMessage("Saving media-resources {1} Contents.json to {0}", mediaResourceAppCataloguePath, AppIconCatalogueName.ItemSpec);
                File.WriteAllText(mediaResourceAppCataloguePath, mediaResourceAppCatalogueJson);


                //output
                outputAppIconSetContents.images = outputAppIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var outputAppCatalogueJson = JsonConvert.SerializeObject(outputAppIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });

 
                var outputAppCataloguePath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == outputAppCataloguePath) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", outputAppCataloguePath);                    
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, outputAppCataloguePath } }));
                          
                }
                Log.LogMessage("Saving project {1} Contents.json to {0}", outputAppCataloguePath, AppIconCatalogueName.ItemSpec);
                File.WriteAllText(outputAppCataloguePath, outputAppCatalogueJson);

                Log.LogMessage("AppIcons wants to add {0} files to the build project", filesToAddToModifiedProject.Count());
                Log.LogMessage("AppIcons wants to show {0} media-resources files in the final project", filesToAddToModifiedProject.Count());

                FilesToAddToProject = filesToAddToModifiedProject.ToArray();

                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
