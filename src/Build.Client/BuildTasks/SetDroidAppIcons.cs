using System;
using System.Collections.Generic;
using System.IO;
using Build.Client.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class SetDroidAppIcons : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        [Output]
        public ITaskItem[] OutputFiles { get; set; }


        public override bool Execute()
        {
            Log.LogMessage("Set Droid App Icons started");

            var mediaResourcesDir = this.GetMediaResourceDir();

            var outputFiles = new List<ITaskItem>();
            foreach(var field in AppIconFields){
                var filePath = Path.Combine(mediaResourcesDir, String.Concat(field.GetMetadata("MediaFileId"), ".png"));

                var itemMetadata = new Dictionary<string, string>();
                itemMetadata.Add("LogicalName", field.GetMetadata("LogicalName"));

                LogDebug("Added Logical file {0} to processing list from media-resource source {1}", field.GetMetadata("LogicalName"), filePath);
                var taskItem = new TaskItem(filePath, itemMetadata);
                outputFiles.Add(taskItem);
            }

            OutputFiles = outputFiles.ToArray();
            return true;
        }
    }


}
