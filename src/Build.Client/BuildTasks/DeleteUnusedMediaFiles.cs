using System;
using System.IO;
using System.Linq;
using Build.Client.Extensions;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class DeleteUnusedMediaFiles : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }
        public string BuildConfiguration { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Deleting unused media files");
            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            //get directory or create
            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration);

            Log.LogMessage("Found {0} existing media files", existingFiles.Count());
            try
            {
                foreach (var file in existingFiles)
                {
                    var fileInfo = new FileInfo(file);
                    var fileNoExt = Path.GetFileNameWithoutExtension(fileInfo.Name);

                    var field = allMediaFields.FirstOrDefault(x => x.GetMetadata("MediaName") == fileNoExt);
                    if (field == null)
                    {
                        Log.LogMessage("File {0} no longer required, deleting", fileInfo.Name);
                        fileInfo.Delete();
                    } else {

                        Log.LogMessage("File {0} still required, skipping", fileInfo.Name);
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
    }
}
