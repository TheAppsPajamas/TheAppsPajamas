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

        public override bool Execute()
        {
            Log.LogWarning("Downloading media files");

            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            LogDebug("{0} media files required", allMediaFields.Count());
            var existingFiles = this.GetExistingMediaFiles().Select(x => new FileHolder(x));
            LogDebug("{0} existing media files downloaded", existingFiles.Count());

            var mediaResourceDir = this.GetMediaResourceDir();
            try
            {
                foreach(var field in allMediaFields){
                    var exists = existingFiles.Any(x => x.FileNoExt == field.GetMetadata("LogicalName"));
                    if (!exists){
                        using (WebClient client = new WebClient())
                        {
                            var url = String.Concat(Consts.UrlBase, Consts.MediaEndpoint, "/", field.GetMetadata("LogicalName"));
                            var fileName = Path.Combine(mediaResourceDir, string.Concat(field.GetMetadata("LogicalName"), ".png"));
                            LogDebug("Downloading mediaFileId {0}, from url {1}", field.GetMetadata("LogicalName"), url);
                            LogDebug("Saving as local file {0}", fileName);
                            client.DownloadFile(url, fileName);
                        }
                    } else {
                        LogDebug("{0}.png media file exists, not downloading", field.GetMetadata("LogicalName"));
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
