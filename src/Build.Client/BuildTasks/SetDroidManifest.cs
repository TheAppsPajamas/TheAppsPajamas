using System;
using System.Linq;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class SetDroidManifest : BaseTask
    {
        public string AndroidManifest { get; set; }

        public ITaskItem[] PackagingFields { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Setting Droid manifest file");

            LogDebug("Manifest file name '{0}'", AndroidManifest);
            LogDebug("Packaging fields: '{0}'", PackagingFields.Count());

            if (String.IsNullOrEmpty(AndroidManifest)){
                Log.LogMessage("Android manifest file name empty, aborting SetDroidManifest as ran successful");
                return true;
            }
            return true;
        }
    }
}
