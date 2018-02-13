using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Build.Client.Constants;
using Build.Client.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class DownloadMediaFiles : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }
        public string BuildConfiguration { get; set; }
        public ITaskItem Token { get; set; }
        public ITaskItem BuildAppId { get; set; }

        [Output]
        public ITaskItem[] FilesToDeleteFromProject { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Downloading media files");

            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration).Select(x => new FileHolder(x));
            Log.LogMessage("{0} media files required, {1} media files already available", allMediaFields.Count(), existingFiles.Count());

            if (Token == null || String.IsNullOrEmpty(Token.ItemSpec)){
                var securityConfig = this.GetSecurityConfig();

                if (securityConfig == null)
                    return false;
                
                Token = this.Login(securityConfig);
                if (Token == null)
                {
                    Log.LogError("Authentication failure");
                    return false;
                }
            }


            var filesToDeleteFromProject = new List<ITaskItem>();

            if (FilesToDeleteFromProject != null)
            {
                filesToDeleteFromProject.AddRange(FilesToDeleteFromProject);
            }

            var mediaResourceDir = this.GetMediaResourceDir(BuildConfiguration);
            try
            {
                foreach(var field in allMediaFields){
                    var exists = existingFiles.Any(x => x.FileNoExt == field.GetMetadata(MetadataType.MediaName));
                    if (!exists)
                    {
                        if (field.GetMetadata(MetadataType.Disabled) == bool.TrueString)
                        {
                            Log.LogMessage("Media field {0} is disabled, not downloading", field.GetMetadata(MetadataType.FieldDescription));
                            continue;
                        }

                        using (WebClient client = new WebClient())
                        {
                            client.SetWebClientHeaders(Token);
                            var url = String.Concat(Consts.UrlBase, Consts.MediaEndpoint, "/", BuildAppId.ItemSpec, "/", field.GetMetadata(MetadataType.MediaFileId));
                            var directory = Path.Combine(mediaResourceDir, field.GetMetadata(MetadataType.Path));
                            if (!Directory.Exists(directory))
                            {
                                LogDebug("Created folder {0}", directory);
                                Directory.CreateDirectory(directory);
                            }
                            var fileName = Path.Combine(mediaResourceDir, field.GetMetadata(MetadataType.Path), field.GetMetadata(MetadataType.MediaName).ApplyPngExt());
                            Log.LogMessage("Downloading media file {0}, from url {1}", fileName, url);
                            client.DownloadFile(url, fileName);
                        }
                    }
                    else if (field.GetMetadata(MetadataType.Disabled) == bool.TrueString)
                    {
                        //Think this takes care of deleting. not sure about old files tho?
                        Log.LogMessage("Media file {0} exists, but is now disabled, deleting", field.GetMetadata(MetadataType.FieldDescription));
                        var fileInfo = existingFiles.FirstOrDefault(x => x.FileNoExt == field.GetMetadata(MetadataType.MediaName));
                        fileInfo.FileInfo.Delete();

                        var fileToDelete = new TaskItem(field.GetMetadata(MetadataType.MSBuildItemType), new Dictionary<string, string>{
                            { MetadataType.DeletePath, fileInfo.FileInfo.FullName }
                        });
                        filesToDeleteFromProject.Add(fileToDelete);    

                    } else {
                        Log.LogMessage("Media file {0} exists, skipping", field.GetMetadata(MetadataType.LogicalName));
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
