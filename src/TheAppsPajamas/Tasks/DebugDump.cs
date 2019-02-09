using System;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class DebugDump : BaseTask
    {
        public ITaskItem[] ImageAssets { get; set; }


        public ITaskItem[] FilesToDeleteFromProject { get; set; }


        public ITaskItem[] FilesToAddToProject { get; set; }


        public override bool Execute()
        {
            LogDebug("Debug dump running");

            if (ImageAssets == null)
            {
                LogDebug("No image assets in project");
            }

            foreach(var imageAsset in ImageAssets){
                String rem = String.Empty;
                try{
                    rem = imageAsset.GetMetadata("Remove");
                }catch (Exception e)
                {
                    rem = "No remove found";
                }
                LogDebug($"Image asset in {imageAsset.ItemSpec} project {imageAsset.GetMetadata("Include")}, remove {rem}");
            }
            return true;
        }
    }
}
