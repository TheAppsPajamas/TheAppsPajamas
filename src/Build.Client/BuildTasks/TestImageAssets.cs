using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class TestImageAssets : Task
    {
        public ITaskItem[] ImageAssets { get; set; }

        public override bool Execute()
        {
            foreach(var a in ImageAssets){
                //Log.LogMessage("Image Asset {0}, md count {1}, fullpath {2}, link {3}, definingprojectpath {4}", a.ItemSpec, a.MetadataCount, a.GetMetadata("FullPath"), a.GetMetadata("Link"), a.GetMetadata("DefiningProjectFullPath"));
            }
            return true;
        }
    }
}
