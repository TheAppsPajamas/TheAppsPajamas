using System;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class LoadLocalBuildConfig : Task
    {
        public LoadLocalBuildConfig()
        {
        }

        //this should load the local config if it exists
        //or apply a false if it doesn't exist
        //which should trigger LoadRemoteBuildConfig
        public override bool Execute()
        {
            return true;
        }
    }
}
