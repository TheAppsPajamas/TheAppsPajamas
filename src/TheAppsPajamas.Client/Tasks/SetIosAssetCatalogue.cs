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
    public class SetIosAssetCatalogue : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }
        public ITaskItem[] SplashFields { get; set; }

        public string BuildConfiguration { get; set; }

        public ITaskItem AssetCatalogueName { get; set; }

        public string PackagesOutputDir { get; set; }
        public ITaskItem AppIconHolder { get; set; }
        public ITaskItem SplashHolder { get; set; }

        public ITaskItem[] ExistingImageAssets { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputImageAssets { get; set; }




        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Set Ios Asset Catalogue started");

            var filesToAddToModifiedProject = new List<ITaskItem>();

            var outputImageAssets = new List<ITaskItem>();
            var outputITunesArtwork = new List<ITaskItem>();

            var existingAssets = new List<ITaskItem>();

            if (ExistingImageAssets != null && ExistingImageAssets.Any())
            {
                existingAssets.AddRange(ExistingImageAssets);
            } else if (ExistingImageAssets == null || ExistingImageAssets.Any() == false){
                LogDebug("No existing image assets found in project");
            }

            //foreach (var taskItem in existingAssets)
            //{
            //    LogDebug("Existing asset in project {0}", taskItem.ItemSpec);
            //}

            try
            {
                var buildConfigurationResourceDir = this.GetBuildConfigurationResourceDir(BuildConfiguration);

                ////could handle disbled here
                //var firstField = AppIconFields.FirstOrDefault();
                //if (firstField == null)
                //{
                //    Log.LogError("App Icon Field set malformed");
                //}

                LogDebug("Asset catalogue name {0}", AssetCatalogueName.ItemSpec);

                LogDebug("Packages Output Folder {0}", PackagesOutputDir);

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
                    LogDebug("Created {0} folder at {1}", AssetCatalogueName, projectAssetCatalogueDir);
                } 

                var mediaResourceAssetCatalogueContentsPath = Path.Combine(buildConfigurationResourceDir, AssetCatalogueName.ItemSpec, Consts.iOSContents);
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

                LogInformation("SetAssetCatalogues wants to add {0} files to the build project", filesToAddToModifiedProject.Count());
               
                FilesToAddToProject = filesToAddToModifiedProject.ToArray();

                OutputImageAssets = outputImageAssets.ToArray();

                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
