using System;
using System.IO;
using Build.Client.Constants;
using Build.Client.Extensions;

namespace Build.Client.BuildTasks
{
    public class CleanProjectsConfig : BaseTask
    {
        public override bool Execute()
        {
            Log.LogMessage("Cleaning build resource project.config");

            var buildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(buildResourceDir))
            {
                Log.LogMessage("Build Resource folder not found, further clean not required");
                return true;
            }

            var projectsConfigPath = Path.Combine(buildResourceDir, Consts.ProjectConfig);
            if (File.Exists(projectsConfigPath)){
                File.Delete(projectsConfigPath);
                Log.LogMessage("Build resource project config file deleted");
                LogDebug("Project config file deleted at {0}", projectsConfigPath);
            } else {
                Log.LogMessage("Build resource project config file not found, further clean not required");
            }
            return true;
        }
    }
}
