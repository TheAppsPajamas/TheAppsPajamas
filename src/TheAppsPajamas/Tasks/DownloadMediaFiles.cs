using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TheAppsPajamas.Constants;
using TheAppsPajamas.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TheAppsPajamas.Tasks
{
    public class DownloadMediaFiles : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }
        public string BuildConfiguration { get; set; }
        public ITaskItem MediaAccessKey { get; set; }
        public ITaskItem TapAppId { get; set; }

        [Output]
        public ITaskItem[] FilesToDeleteFromProject { get; set; }

        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Downloading media files");
            LogDebug($"Media access key ${MediaAccessKey.ItemSpec}");

            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration).Select(x => new FileHolder(x));
            LogInformation("{0} media files required, {1} media files already available", allMediaFields.Count(), existingFiles.Count());

            var filesToDeleteFromProject = new List<ITaskItem>();

            if (FilesToDeleteFromProject != null)
            {
                filesToDeleteFromProject.AddRange(FilesToDeleteFromProject);
            }

            var buildConfigurationAssetDir = this.GetBuildConfigurationAssetDir(BuildConfiguration);
            try
            {
                foreach(var field in allMediaFields){
                    var exists = existingFiles.Any(x => x.FileNoExt == field.GetMetadata(MetadataType.MediaName));
                    if (!exists)
                    {
                        if (field.IsDisabled() == true)
                        {
                            Log.LogMessage("Media field {0} is disabled, not downloading", field.GetMetadata(MetadataType.FieldDescription));
                            continue;
                        }

                        using (WebClient client = new WebClient())
                        {
                            LogDebug("Media file exists, getting setup for download");
                            //do we need headers at all?
                            //client.SetWebClientHeaders(Token);
                            LogDebug("Generating url for mediaId {0}, {1}", field.GetMetadata(MetadataType.MediaFileId), TapAppId.ItemSpec);
                            var url = String.Concat(TapSettings.GetMetadata(MetadataType.MediaEndpoint), "/", TapAppId.ItemSpec, "/", field.GetMetadata(MetadataType.MediaFileId), ".png", MediaAccessKey.ItemSpec);
                            LogDebug($"url generated : {url}");
                            LogDebug("Finding directory");
                            var directory = Path.Combine(buildConfigurationAssetDir, field.GetMetadata(MetadataType.TapAssetPath));
                            LogDebug("Checking directory existance {0}", directory);
                            if (!Directory.Exists(directory))
                            {
                                LogDebug("Created folder {0}", directory);
                                Directory.CreateDirectory(directory);
                            }
                            var fileName = Path.Combine(buildConfigurationAssetDir, field.GetMetadata(MetadataType.TapAssetPath), field.GetMetadata(MetadataType.MediaName).ApplyPngExt());
                            LogInformation("Downloading media file {0}, from url {1}", fileName, url);
                            client.DownloadFile(url, fileName);
                        }
                    
                        //doing this in deleteunusedfiles, so should be alright
                        //}
                    //else if (field.IsDisabled() == true)
                    //{
                        ////Think this takes care of deleting. not sure about old files tho?
                        //Log.LogMessage("Media file {0} exists, but is now disabled, deleting", field.GetMetadata(MetadataType.FieldDescription));
                        //var fileInfo = existingFiles.FirstOrDefault(x => x.FileNoExt == field.GetMetadata(MetadataType.MediaName));
                        //fileInfo.FileInfo.Delete();

                        //var fileToDelete = new TaskItem(field.GetMetadata(MetadataType.MSBuildItemType), new Dictionary<string, string>{
                        //    { MetadataType.DeletePath, fileInfo.FileInfo.FullName }
                        //});
                        //filesToDeleteFromProject.Add(fileToDelete);    

                    } else {
                        LogInformation("Media file {0} exists, skipping", field.GetMetadata(MetadataType.LogicalName));
                    }
                }
                FilesToDeleteFromProject = filesToDeleteFromProject.ToArray();
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            return true;
        }

        private class FileHolder{
            public FileInfo FileInfo { get; set; }
            public string FileNoExt { get; set; }

            public FileHolder(string path){
                FileInfo = new FileInfo(path);
                FileNoExt = Path.GetFileNameWithoutExtension(FileInfo.Name);
            }
        }
            
    }
}
