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

        [Output]
        public ITaskItem[] FilesToDeleteFromProject { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Deleting unused media files");
            var allMediaFields = this.CombineMediaFields(AppIconFields, SplashFields);

            //get directory or create
            var existingFiles = this.GetExistingMediaFiles(BuildConfiguration);

            var filesToDeleteFromProject = new List<ITaskItem>();

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
                        var fileToDelete = new TaskItem(field.GetMetadata(MetadataType.MSBuildItemType), new Dictionary<string, string>{
                            { "DeletePath", file }
                        });
                        filesToDeleteFromProject.Add(fileToDelete);    

                        fileInfo.Delete();
                    } else {

                        Log.LogMessage("File {0} still required, skipping", fileInfo.Name);
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
    }
}
