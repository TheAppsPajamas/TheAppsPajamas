using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.Constants;
using Build.Client.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class DeleteUnusedMediaFiles : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }
        public string BuildConfiguration { get; set; }

        public ITaskItem[] DroidResources { get; set; }
        public ITaskItem[] IosImageAssets { get; set; }
        public ITaskItem[] IosItunesArtwork { get; set; }

        [Output]
        public ITaskItem[] FilesToDeleteFromProject { get; set; }


        [Output]
        public ITaskItem[] DroidResourcesToRemoveFromProject { get; set; }


        [Output]
        public ITaskItem[] IosImageAssetsToRemoveFromProject { get; set; }


        [Output]
        public ITaskItem[] IosItunesArtworkToRemoveFromProject { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Deleting unused media files");
            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            //get directory or create
            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration);

            var filesToDeleteFromProject = new List<ITaskItem>();
            var droidResourcesToRemoveFromProject = new List<ITaskItem>();
            var iosImageAssetsToRemoveFromProject = new List<ITaskItem>();
            var iosItunesArtworkToRemoveFromProject = new List<ITaskItem>();

            Log.LogMessage("Found {0} existing media files", existingFiles.Count());

            var droidResources = new List<ITaskItem>();
            var iosImageAssets = new List<ITaskItem>();
            var iosItunesArtwork = new List<ITaskItem>();

      
            if (DroidResources != null && DroidResources.Any())
            {
                droidResources.AddRange(DroidResources);
            }

            if (IosImageAssets != null && IosImageAssets.Any())
            {
                iosImageAssets.AddRange(IosImageAssets);
            }

            if (IosItunesArtwork != null && IosItunesArtwork.Any())
            {
                iosItunesArtwork.AddRange(IosItunesArtwork);
            }

            foreach (var file in existingFiles)
            {
                var fileInfo = new FileInfo(file);
                var fileNoExt = Path.GetFileNameWithoutExtension(fileInfo.Name);

                var field = allMediaFields.FirstOrDefault(x => x.GetMetadata(MetadataType.MediaName) == fileNoExt);
                if (field == null)
                {
                    ITaskItem existingAsset = null;
                    string existingAssetItemGroup = String.Empty;
                    existingAsset = droidResources.FirstOrDefault(x => x.ItemSpec == file.GetPathRelativeToProject(ProjectDir));
                    if (existingAsset != null){
                        droidResourcesToRemoveFromProject.Add(new TaskItem(file.GetPathRelativeToProject(ProjectDir)));
                    }
                    if (existingAsset == null)
                    {
                        existingAsset = iosImageAssets.FirstOrDefault(x => x.ItemSpec == file.GetPathRelativeToProject(ProjectDir));
                        LogDebug($"Searching for existing asset {file.GetPathRelativeToProject(ProjectDir)} in iosimage assets");
                        if (existingAsset != null)
                        {
                            LogDebug("Found  in iosimage assets");
                            iosImageAssetsToRemoveFromProject.Add(new TaskItem(file.GetPathRelativeToProject(ProjectDir)));
                        }
                    }
                    if (existingAsset == null)
                    {
                        existingAsset = iosItunesArtwork.FirstOrDefault(x => x.ItemSpec == file.GetPathRelativeToProject(ProjectDir));
                        if (existingAsset != null)
                        {
                            iosItunesArtworkToRemoveFromProject.Add(new TaskItem(file.GetPathRelativeToProject(ProjectDir)));
                        }
                    }


                    if (existingAsset == null)
                    {
                        Log.LogMessage($"Existing project asset to delete {file.GetPathRelativeToProject(ProjectDir)} doesn't exist in project, deletion will still occur, but csproj file might be confused");
                    }
                    else
                    {
                        LogDebug($"Existing asset to delete found in project {existingAsset.ItemSpec}");
                    }
                    Log.LogMessage($"File {fileInfo.Name} no longer required, adding to deletion list");


                    var fileToDelete = new TaskItem(file);
                    filesToDeleteFromProject.Add(fileToDelete);
                    //can't delete, because we can't remove it from project programatically if it doesn't exist. deletes when modifyproject is run
                    //fileInfo.Delete();
                }
                else
                {

                    Log.LogMessage("File {0} still required, skipping", fileInfo.Name);
                }
            }

            FilesToDeleteFromProject = filesToDeleteFromProject.ToArray();
            DroidResourcesToRemoveFromProject = droidResourcesToRemoveFromProject.ToArray();
            IosImageAssetsToRemoveFromProject = iosImageAssetsToRemoveFromProject.ToArray();
            IosImageAssetsToRemoveFromProject = iosItunesArtworkToRemoveFromProject.ToArray();

            return true;
        }
    }
}
