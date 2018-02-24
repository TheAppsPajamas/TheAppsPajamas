using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Extensions;
using TheAppsPajamas.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace TheAppsPajamas.Client.Tasks
{
    public class SetIosAssetCatalogueSets : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }

        public string BuildConfiguration { get; set; }

        public ITaskItem AssetCatalogueName { get; set; }


        public string PackagesOutputDir { get; set; }


        public ITaskItem[] ExistingImageAssets { get; set; }

        public ITaskItem[] ExistingFilesToAddToProject { get; set; }
        public ITaskItem[] ExistingOutputImageAssets { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputImageAssets { get; set; }


        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Set Ios Asset Cataloge Sets started");

            //turns out msbuild adds to the output array itself, so if we send the output back down, and into the same 
            //item property it will get added
            var filesToAddToModifiedProject = new List<ITaskItem>();


            //this above discovery will change this
            var outputImageAssets = new List<ITaskItem>();
            if (ExistingOutputImageAssets != null && ExistingOutputImageAssets.Any())
            {
                outputImageAssets.AddRange(ExistingOutputImageAssets);
            }

            var existingAssets = new List<ITaskItem>();

            if (ExistingImageAssets != null && ExistingImageAssets.Length != 0)
            {
                existingAssets.AddRange(ExistingImageAssets);
            }
            else if (ExistingImageAssets == null || ExistingImageAssets.Length == 0)
            {
                LogDebug("No existing image assets found in project");
            }

            foreach (var taskItem in existingAssets)
            {
                LogDebug("Existing asset in project {0}", taskItem.ItemSpec);
            }

            //try
            //{
            var buildConfigurationResourceDir = this.GetBuildConfigurationResourceDir(BuildConfiguration);

            //could handle disbled here
            var firstField = AppIconFields.FirstOrDefault();
            if (firstField == null)
            {
                Log.LogError("App Icon Field set malformed");
            }

            LogDebug("Asset catalogue name {0}", AssetCatalogueName.ItemSpec);

            //LogDebug("AppIconSet name {0}", AppIconCatalogueName.ItemSpec);


            LogDebug("Packages Output Folder {0}", PackagesOutputDir);

            //get us a list of required catalogues to create/operate on here

            var allFields = new List<ITaskItem>();
            allFields.AddRange(AppIconFields);
            allFields.AddRange(SplashFields);

            var cataloguesToSet = allFields
                .GroupBy(x => x.GetMetadata(MetadataType.CatalogueSetName))
                .Select(x => x.Key)
                .ToList();

            cataloguesToSet = cataloguesToSet.Where(x => !String.IsNullOrEmpty(x)).ToList();

            foreach (var catalogue in cataloguesToSet)
            {

                LogDebug("Asset Catalogue set found in metadata {0}", catalogue);
            }

            //for temp testing jsut make it do app icons - till we've got the other contents.json stuff in
            foreach (var catalogue in cataloguesToSet)
            {
                LogDebug("Asset Catalogue set required {0}", catalogue);
                //create catalogue set folder, and contents.json
                var packagesCatalogueSetDir = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec, catalogue);

                if (!Directory.Exists(packagesCatalogueSetDir))
                {
                    Directory.CreateDirectory(packagesCatalogueSetDir);
                    LogDebug($"Created packages {catalogue} folder at {packagesCatalogueSetDir}");
                }

                var projectCatalogueSetDir = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, catalogue);

                if (!Directory.Exists(projectCatalogueSetDir))
                {
                    Directory.CreateDirectory(projectCatalogueSetDir);
                    LogDebug($"Created project {catalogue} folder at {projectCatalogueSetDir}");
                }

                var packagesCatalogueSetContentsPath = Path.Combine(PackagesOutputDir, AssetCatalogueName.ItemSpec, catalogue, Consts.iOSContents);
                outputImageAssets.Add(new TaskItem(packagesCatalogueSetContentsPath));


                //TODO we need to have a different contents set for each type - just use string matching from extension
                //this type can probably change to CatalogueSet - they're similar, and if we load them with 
                //the right json setting, all null fields will disapear

                string defaultContents = String.Empty;
                MediaAssetCatalogue mediaResourceCatalogueSetContents = null;
                MediaAssetCatalogue outputCatalogueSetContents = null;
                if (catalogue.Contains(".appiconset"))
                {
                    defaultContents = Consts.AppIconCatalogueSetDefaultContents;
                    mediaResourceCatalogueSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(defaultContents);

                    outputCatalogueSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(defaultContents);
                }
                else if (catalogue.Contains(".launchimage"))
                {
                    defaultContents = Consts.LaunchImageCatalogueSetDefaultContents;
                    mediaResourceCatalogueSetContents = new MediaAssetCatalogue();
                    outputCatalogueSetContents = new MediaAssetCatalogue();

                }
                else if (catalogue.Contains(".imageset"))
                {
                    defaultContents = Consts.ImageCatalogueSetDefaultContents;
                    mediaResourceCatalogueSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(defaultContents);

                    outputCatalogueSetContents = JsonConvert.DeserializeObject<MediaAssetCatalogue>(defaultContents);
                }
                else
                {
                    throw new Exception($"Cannot figure catalogue {0} default contents out");
                }



                foreach (var field in allFields.Where(x => x.GetMetadata(MetadataType.CatalogueSetName) == catalogue))
                {
                    //skip itunes artwork first, something else will do that
                    if (String.IsNullOrEmpty(field.GetMetadata(MetadataType.Idiom)))
                    {

                        //var existingFilePath = Path.Combine(mediaResourcesBuildConfigDir, Consts.iTunesArtworkDir, field.GetMetadata(MetadataType.MediaName).ApplyPngExt());
                        //var packagesOutputFilePath = Path.Combine(PackagesOutputDir, field.GetMetadata(MetadataType.LogicalName));

                        //var projectOutputFilePath = Path.Combine(base.ProjectDir, field.GetMetadata(MetadataType.LogicalName));

                        //if (existingAssets.FirstOrDefault(x => x.ItemSpec == existingFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                        //{
                        //    LogDebug("Adding {0} to add to project list as it is not in current project", existingFilePath);
                        //    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.iTunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, existingFilePath } }));
                        //}

                        //if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectOutputFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                        //{
                        //    LogDebug("Adding {0} to add to project list as it is not in current project", projectOutputFilePath);
                        //    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.iTunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputFilePath } }));
                        //}

                        //File.Copy(existingFilePath, projectOutputFilePath, true);
                        //File.Copy(existingFilePath, packagesOutputFilePath, true);
                        //outputITunesArtwork.Add(new TaskItem(packagesOutputFilePath));
                        //var artworkTaskItem = new TaskItem(field.GetMetadata(MetadataType.LogicalName));

                        //LogDebug("Added {2} from {0} to {1}", existingFilePath, projectOutputFilePath, MSBuildItemName.iTunesArtwork);

                        continue;
                    }

                    Image mediaResourceImageSet = null;
                    Image outputImageSet = null;
                    if (catalogue.Contains(".appiconset"))
                    {
                        mediaResourceImageSet = GetAppIconCatalogueSetReference(mediaResourceCatalogueSetContents, field);
                        outputImageSet = GetAppIconCatalogueSetReference(outputCatalogueSetContents, field);


                    }
                    else if (catalogue.Contains(".launchimage"))
                    {
                        mediaResourceImageSet = GetLaunchImageCatalogueSetReference(mediaResourceCatalogueSetContents, field);
                        outputImageSet = GetLaunchImageCatalogueSetReference(outputCatalogueSetContents, field);

                    }
                    else if (catalogue.Contains(".imageset"))
                    {
                        mediaResourceImageSet = GetImageCatalogueSetReference(mediaResourceCatalogueSetContents, field);
                        outputImageSet = GetImageCatalogueSetReference(outputCatalogueSetContents, field);

                    }
                    else
                    {
                        throw new Exception("Error finding catalogue");
                        //throw new Exception($"Cannot calculate image catalogue or field {field.GetMetadata(MetadataType.LogicalName)}, size {1}, idiom {field.GetMetadata(MetadataType.Idiom)}, scale {field.GetMetadata(MetadataType.Scale)}");
                    }


                    if (outputImageSet == null)
                    {
                        LogWarning("Image catalogue not found for field {0}, size {1}, idiom {2}, scale {3}"
                                       , field.GetMetadata(MetadataType.LogicalName)
                                       , field.GetMetadata(MetadataType.Size)
                                       , field.GetMetadata(MetadataType.Idiom)
                                       , field.GetMetadata(MetadataType.Scale));
                        //return false;
                    }
                    else
                    {
                        outputImageSet.filename = field.GetMetadata(MetadataType.ContentsFileName);
                        mediaResourceImageSet.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                        LogDebug("Set asset catalogue set filename to {0}", outputImageSet.filename);

                        //sometimes we use the same image twice in the contents.json
                        var outputImageCatalogue2 = outputCatalogueSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                        if (outputImageCatalogue2 != null)
                        {
                            outputImageCatalogue2.filename = field.GetMetadata(MetadataType.ContentsFileName);
                            var mediaResourceImageCatalogue2 = mediaResourceCatalogueSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                   && x.idiom == field.GetMetadata(MetadataType.Idiom2)
                                                                                                   && x.scale == field.GetMetadata(MetadataType.Scale));
                            mediaResourceImageCatalogue2.filename = field.GetMetadata(MetadataType.MediaName).ApplyPngExt();
                            LogDebug("Set second asset catalogue set filename to {0}", outputImageCatalogue2.filename);
                        }

                        var existingFilePath = Path.Combine(buildConfigurationResourceDir
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
                mediaResourceCatalogueSetContents.images = mediaResourceCatalogueSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var mediaResourceCatalogueSetJson = JsonConvert.SerializeObject(mediaResourceCatalogueSetContents, Formatting.Indented
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                var mediaResourceCatalogueSetContentsPath = Path.Combine(buildConfigurationResourceDir, AssetCatalogueName.ItemSpec, catalogue, Consts.iOSContents);
                LogDebug($"Added media-resource {catalogue} Contents.json at path {mediaResourceCatalogueSetContentsPath}");

                LogInformation($"Saving media-resources {catalogue} Contents.json to {mediaResourceCatalogueSetContentsPath}");
                File.WriteAllText(mediaResourceCatalogueSetContentsPath, mediaResourceCatalogueSetJson);

                //packagesAppIconSetContentsPath
                //output
                outputCatalogueSetContents.images = outputCatalogueSetContents.images.Where(x => !String.IsNullOrEmpty(x.filename)).ToList();

                var outputCatalogueSetJson = JsonConvert.SerializeObject(outputCatalogueSetContents, Formatting.Indented
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == mediaResourceCatalogueSetContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", mediaResourceCatalogueSetContentsPath);
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset
                                                                 , new Dictionary<string, string> { { MetadataType.IncludePath, mediaResourceCatalogueSetContentsPath } }));

                }

                var projectOutputCatalogueSetContentsPath = Path.Combine(ProjectDir, AssetCatalogueName.ItemSpec, catalogue, Consts.iOSContents);
                LogDebug($"Added project output {catalogue} Contents.json at path {projectOutputCatalogueSetContentsPath}");

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == projectOutputCatalogueSetContentsPath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", projectOutputCatalogueSetContentsPath);
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ImageAsset
                                                                 , new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputCatalogueSetContentsPath } }));

                }

                LogInformation($"Saving project {catalogue} Contents.json to {projectOutputCatalogueSetContentsPath}");
                File.WriteAllText(projectOutputCatalogueSetContentsPath, outputCatalogueSetJson);
                File.WriteAllText(packagesCatalogueSetContentsPath, outputCatalogueSetJson);

            }


            LogInformation("Asset Catalogue Sets wants to add {0} files to the build project", filesToAddToModifiedProject.Count());
            LogInformation("Asset Catalogue Sets wants to show {0} media-resources files in the final project", filesToAddToModifiedProject.Count());

            FilesToAddToProject = filesToAddToModifiedProject.ToArray();

            OutputImageAssets = outputImageAssets.ToArray();

            return true;
            //} catch (Exception ex){
            //    Log.LogErrorFromException(ex);
            //    return false;
            //}
        }

        private static Image GetLaunchImageCatalogueSetReference(MediaAssetCatalogue catalogueSetContents, ITaskItem field)
        {
            //var set = catalogueSetContents.images.Where(x => x.size == field.GetMetadata(MetadataType.Size)
            //                                                                        && x.idiom == field.GetMetadata(MetadataType.Idiom)
            //                                                                        && x.scale == field.GetMetadata(MetadataType.Scale));
            //if (set != null && !String.IsNullOrEmpty(field.GetMetadata(MetadataType.Extent))){
            //    set = set.Where(x => x.extent == field.GetMetadata(MetadataType.Extent));
            //}

            //if (set != null && !String.IsNullOrEmpty(field.GetMetadata(MetadataType.MinimumSystemVersion)))
            //{
            //    set = set.Where(x => x.minimumsystemversion == field.GetMetadata(MetadataType.MinimumSystemVersion));
            //}

            //if (set != null && !String.IsNullOrEmpty(field.GetMetadata(MetadataType.Orientation)))
            //{
            //    set = set.Where(x => x.orientation == field.GetMetadata(MetadataType.Orientation));
            //}
            //return set.FirstOrDefault();

            var image = new Image();
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Idiom)))
            {
                image.idiom = field.GetMetadata(MetadataType.Idiom);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Subtype)))
            {
                image.subtype = field.GetMetadata(MetadataType.Subtype);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Scale)))
            {
                image.scale = field.GetMetadata(MetadataType.Scale);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Size)))
            {
                image.size = field.GetMetadata(MetadataType.Size);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Extent)))
            {
                image.extent = field.GetMetadata(MetadataType.Extent);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.MinimumSystemVersion)))
            {
                image.minimumsystemversion = field.GetMetadata(MetadataType.MinimumSystemVersion);
            }
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Orientation)))
            {
                image.orientation = field.GetMetadata(MetadataType.Orientation);
            }

            //image.subtype = field.GetMetadata(MetadataType.Subtype);
            //image.scale = field.GetMetadata(MetadataType.Scale);
            catalogueSetContents.images.Add(image);
            return image;

        }

        private static Image GetImageCatalogueSetReference(MediaAssetCatalogue catalogueSetContents, ITaskItem field)
        {
            if (!String.IsNullOrEmpty(field.GetMetadata(MetadataType.Subtype)))
            {
                var image = new Image();
                image.idiom = field.GetMetadata(MetadataType.Idiom);
                image.subtype = field.GetMetadata(MetadataType.Subtype);
                image.scale = field.GetMetadata(MetadataType.Scale);
                catalogueSetContents.images.Add(image);
                return image;
            }

            return catalogueSetContents.images.FirstOrDefault(x => x.idiom == field.GetMetadata(MetadataType.Idiom)
                                                                                                && x.scale == field.GetMetadata(MetadataType.Scale));
        }

        private static Image GetAppIconCatalogueSetReference(MediaAssetCatalogue catalogueSetContents, ITaskItem field)
        {
            return catalogueSetContents.images.FirstOrDefault(x => x.size == field.GetMetadata(MetadataType.Size)
                                                                                                && x.idiom == field.GetMetadata(MetadataType.Idiom)
                                                                                                && x.scale == field.GetMetadata(MetadataType.Scale));
        }
    }


}
