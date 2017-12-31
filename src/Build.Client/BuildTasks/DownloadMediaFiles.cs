using System;
using System.IO;
using System.Linq;
using System.Net;
using Build.Client.Constants;
using Build.Client.Extensions;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class DownloadMediaFiles : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }
        public string BuildConfiguration { get; set; }

        public override bool Execute()
        {
            Log.LogWarning("Downloading media files");

            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            LogDebug("{0} media files required", allMediaFields.Count());
            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration).Select(x => new FileHolder(x));
            LogDebug("{0} existing media files already available", existingFiles.Count());

            var mediaResourceDir = this.GetMediaResourceDir(BuildConfiguration);
            try
            {
                foreach(var field in allMediaFields){
                    var exists = existingFiles.Any(x => x.FileNoExt == field.GetMetadata("MediaName"));
                    if (!exists){
                        using (WebClient client = new WebClient())
                        {
                            var url = String.Concat(Consts.UrlBase, Consts.MediaEndpoint, "/", field.GetMetadata("MediaFileId"));
                            var directory = Path.Combine(mediaResourceDir, field.GetMetadata("Path"));
                            if (!Directory.Exists(directory)){
                                LogDebug("Created folder {0}", directory);
                                Directory.CreateDirectory(directory);
                            }
                            var fileName = Path.Combine(mediaResourceDir, field.GetMetadata("Path"), string.Concat(field.GetMetadata("MediaName"), ".png"));
                            LogDebug("Downloading mediaFileId {0}, from url {1}", field.GetMetadata("MediaFileId"), url);
                            LogDebug("Saving logical file {1} as media-resource file {0}", fileName, field.GetMetadata("LogicalName"));
                            client.DownloadFile(url, fileName);
                        }
                    } else {
                        LogDebug("Logical media-resource {1} exists as {0}.png, not downloading", field.GetMetadata("MediaFileId"), field.GetMetadata("LogicalName"));
                    }
                }
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
