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

        public string PackagesOutputDir { get; set; }

        public ITaskItem[] ExistingiTunesArtworks { get; set; }

        public ITaskItem[] ExistingImageAssets { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputImageAssets { get; set; }

        [Output]
        public ITaskItem[] OutputITunesArtwork { get; set; }



        public override bool Execute()
        {
            Log.LogMessage("Set Ios App Icons started");


            var filesToAddToModifiedProject = new List<ITaskItem>();

            var outputImageAssets = new List<ITaskItem>();
            var outputITunesArtwork = new List<ITaskItem>();

            var existingAssets = new List<ITaskItem>();

            if (ExistingImageAssets != null && ExistingImageAssets.Length != 0)
            {
                existingAssets.AddRange(ExistingImageAssets);
            } else if (ExistingImageAssets == null || ExistingImageAssets.Length == 0){
                LogDebug("No existing image assets found in project");
            }

            if (ExistingiTunesArtworks != null && ExistingiTunesArtworks.Length != 0)
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


                LogDebug("Packages Output Folder {0}", PackagesOutputDir);

                #region CreateAssetCatalogueBaseFolderAndContentsJson
                //create packages asset catalogue folder etc

                var packagesAssetCatalogueDir = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec);

                if (!Directory.Exists(packagesAssetCatalogueDir))
                {
                    Directory.CreateDirectory(packagesAssetCatalogueDir);
                    LogDebug("Created {0} folder at {1}", AssetCatalogueName, packagesAssetCatalogueDir);
                }
                else
                {
                    Directory.Delete(packagesAssetCatalogueDir, true);
                    Directory.CreateDirectory(packagesAssetCatalogueDir);
                    LogDebug("Cleaned and Created {0} folder at {1}", AssetCatalogueName, packagesAssetCatalogueDir);
                }

                var packagesAssetCatalogueContentsPath = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);
                if (!File.Exists(packagesAssetCatalogueContentsPath))
                {
                    LogDebug("Creating Asset catalogue Contents.json at {0}", packagesAssetCatalogueContentsPath);
                    File.WriteAllText(packagesAssetCatalogueContentsPath, Consts.AssetCatalogueContents);
                }
                outputImageAssets.Add(new TaskItem(packagesAssetCatalogueContentsPath));


                //create project asset catalogue folder, and contents.json
                var projectAssetCatalogueDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec);

                if (!Directory.Exists(projectAssetCatalogueDir)){
                    Directory.CreateDirectory(projectAssetCatalogueDir);
                    //Don't need to add this either, it's just contents.json that gets added
                    //filesToAddToProject.Add(new TaskItem("ImageAsset", new Dictionary<string, string>{ {"IncludePath", outputAssetCatalogueDir}}));
                    LogDebug("Created {0} folder at {1}", AssetCatalogueName, projectAssetCatalogueDir);
                } 

                var mediaResourceAssetCatalogueContentsPath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);
                if (!File.Exists(mediaResourceAssetCatalogueContentsPath))
                {
                    LogDebug("Creating Asset catalogue Contents.json at {0}", mediaResourceAssetCatalogueContentsPath);
                    File.WriteAllText(mediaResourceAssetCatalogueContentsPath, Consts.AssetCatalogueContents);
                }

                var projectAssetCatalogueContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);

                if (!File.Exists(projectAssetCatalogueContentsPath)){
                    LogDebug("Creating Asset catalogue Contents.json at {0}", projectAssetCatalogueContentsPath);
                    File.WriteAllText(projectAssetCatalogueContentsPath, Consts.AssetCatalogueContents);
                    Log.LogMessage("Saving {1} Contents.json to path {0}", projectAssetCatalogueContentsPath, AssetCatalogueName.ItemSpec);
                }

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == mediaResourceAssetCatalogueContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, mediaResourceAssetCatalogueContentsPath } }));
                    LogDebug("Adding {0} to add to project list as it is not in current project", mediaResourceAssetCatalogueContentsPath);
                }

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectAssetCatalogueContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, projectAssetCatalogueContentsPath } }));
                    LogDebug("Adding {0} to add to project list as it is not in current project", projectAssetCatalogueContentsPath);
                }

#endregion

                //create appiconset folder, and contents.json
                var packagesAppIconSetDir = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec);

                if (!Directory.Exists(packagesAppIconSetDir))
                {
                    Directory.CreateDirectory(packagesAppIconSetDir);
                    LogDebug("Created packages app-icon-set folder at {0}", packagesAppIconSetDir);
                }     

                var projectAppIconSetDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec);

                if (!Directory.Exists(projectAppIconSetDir))
                {
                    Directory.CreateDirectory(projectAppIconSetDir);
                    LogDebug("Created project app-icon-set folder at {0}", projectAppIconSetDir);
                }                          

                var packagesAppIconSetContentsPath = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
                outputImageAssets.Add(new TaskItem(packagesAppIconSetContentsPath));

                var mediaResourceAppIconSetContentsPath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
              
                var mediaResourceAppIconSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(Consts.AppIconCatalogueSetDefaultContents);

                LogDebug("Added media-resource {0} Contents.json at path {0}", mediaResourceAppIconSetContentsPath, AppIconCatalogueName.ItemSpec);

                var projectOutputAppIconSetContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
               
                //use this contents for packages folder and for project output folder (it only has relative paths
                var outputAppIconSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(Consts.AppIconCatalogueSetDefaultContents);

                LogDebug("Added project output {0} Contents.json at path {0}", projectOutputAppIconSetContentsPath, AppIconCatalogueName.ItemSpec);

                //TODO if this doesn't exist, need to add it etc
                foreach (var field in AppIconFields)
                {
                    //do itunes artwork first
                    if (String.IsNullOrEmpty(field.GetMetadata(MetadataType.Idiom))){

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, Consts.iTunesArtworkDir, field.GetMetadata(MetadataType.MediaName).ApplyPngExt());
                        var packagesOutputFilePath = Path.Combine(PackagesOutputDir, field.GetMetadata(MetadataType.LogicalName));

                        var projectOutputFilePath = Path.Combine(base.ProjectDir, field.GetMetadata(MetadataType.LogicalName));

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == existingFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                        {
                            LogDebug("Adding {0} to add to project list as it is not in current project", existingFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.iTunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, existingFilePath } }));
                        }

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectOutputFilePath.GetPathRelativeToProject(ProjectDir)) == null){
                            LogDebug("Adding {0} to add to project list as it is not in current project", projectOutputFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.iTunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputFilePath } }));
                        }

                        File.Copy(existingFilePath, projectOutputFilePath, true);
                        File.Copy(existingFilePath, packagesOutputFilePath, true);
                        outputITunesArtwork.Add(new TaskItem(packagesOutputFilePath));
                        var artworkTaskItem = new TaskItem(field.GetMetadata(MetadataType.LogicalName));

                        LogDebug("Added {2} from {0} to {1}", existingFilePath, projectOutputFilePath, MSBuildItemName.iTunesArtwork);

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
                        outputImageCatalogue.filename = field.GetMetadata(MetadataType.ContentsFileName);
                        mediaResourceImageCatalogue.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                        LogDebug("Set asset catalogue filename to {0}", outputImageCatalogue.filename);

                        //sometimes we use the same image twice in the contents.json
                        var outputImageCatalogue2 = outputAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                        if (outputImageCatalogue2 != null){
                            outputImageCatalogue2.filename = field.GetMetadata(MetadataType.ContentsFileName);
                            var mediaResourceImageCatalogue2 = mediaResourceAppIconSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                            mediaResourceImageCatalogue2.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                            LogDebug("Set second asset catalogue filename to {0}", outputImageCatalogue2.filename);
                        }

                        var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir
                                                            , field.GetMetadata(MetadataType.Path)
                                                            , field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                        var packagingOutputFilePath = Path.Combine(PackagesOutputDir, field.GetMetadata(MetadataType.Path)
                                                          , field.GetMetadata(MetadataType.LogicalName));
                        
                        var projectOutputFilePath = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.Path)
                                                          , field.GetMetadata(MetadataType.LogicalName));

                        //we want a list of existing imageassets, and itunesartwork to work of, rather than a test of file existence

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == existingFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                        {
                            LogDebug("Adding {0} to add to project list as it is not in current project", existingFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, existingFilePath } }));

                        }

                        if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectOutputFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                        {
                            LogDebug("Adding {0} to add to project list as it is not in current project", projectOutputFilePath);
                            filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputFilePath } }));
                          
                        }

                        LogDebug("Copying file {0} to {1}", existingFilePath, projectOutputFilePath);
                        File.Copy(existingFilePath, projectOutputFilePath, true);
                        File.Copy(existingFilePath, packagingOutputFilePath, true);
                        outputImageAssets.Add(new TaskItem(packagingOutputFilePath));

                    }
                }

                //mediaresource (output)
                mediaResourceAppIconSetContents.images = mediaResourceAppIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var mediaResourceAppCatalogueJson = JsonConvert.SerializeObject(mediaResourceAppIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                //var mediaResourceAppIconSetContentsPath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);
                //var mediaResourceAppCataloguePath = Path.Combine(mediaResourcesBuildConfigDir, AssetCatalogueName.ItemSpec, AppIconCatalogueName.ItemSpec, Consts.iOSContents);

                Log.LogMessage("Saving media-resources {1} Contents.json to {0}", mediaResourceAppIconSetContentsPath, AppIconCatalogueName.ItemSpec);
                File.WriteAllText(mediaResourceAppIconSetContentsPath, mediaResourceAppCatalogueJson);

                //packagesAppIconSetContentsPath
                //output
                outputAppIconSetContents.images = outputAppIconSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var outputAppCatalogueJson = JsonConvert.SerializeObject(outputAppIconSetContents, Formatting.Indented
                    , new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == mediaResourceAppIconSetContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", mediaResourceAppIconSetContentsPath);
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, mediaResourceAppIconSetContentsPath } }));

                }
                if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectOutputAppIconSetContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", projectOutputAppIconSetContentsPath);                    
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset, new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputAppIconSetContentsPath } }));
                          
                }
                Log.LogMessage("Saving project {1} Contents.json to {0}", projectOutputAppIconSetContentsPath, AppIconCatalogueName.ItemSpec);
                File.WriteAllText(projectOutputAppIconSetContentsPath, outputAppCatalogueJson);
                File.WriteAllText(packagesAppIconSetContentsPath, outputAppCatalogueJson);

                Log.LogMessage("AppIcons wants to add {0} files to the build project", filesToAddToModifiedProject.Count());
                Log.LogMessage("AppIcons wants to show {0} media-resources files in the final project", filesToAddToModifiedProject.Count());

                FilesToAddToProject = filesToAddToModifiedProject.ToArray();

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
