using System;
using System.IO;
using Build.Client.Constants;
using Build.Client.Extensions;

namespace Build.Client.BuildTasks
{
    //TODo no longer used
    //public class CleanProjectsConfig : BaseTask
    //{
    //    public override bool Execute()
    //    {
    //        var baseResult = base.Execute();
    //        Log.LogMessage($"Cleaning {Consts.ProjectsConfig}");

    //        var tapResourcesDir = this.GetTapResourcesDir();
    //        if (String.IsNullOrEmpty(tapResourcesDir))
    //        {
    //            Log.LogMessage($"{Consts.TapResourcesDir} folder not found, further clean not required");
    //            return true;
    //        }

    //        var projectsConfigPath = Path.Combine(tapResourcesDir, Consts.ProjectsConfig);
    //        if (File.Exists(projectsConfigPath)){
    //            File.Delete(projectsConfigPath);
    //            Log.LogMessage($"{Consts.ProjectsConfig} file deleted");
    //            LogDebug($"{Consts.ProjectsConfig} file deleted at {projectsConfigPath}", projectsConfigPath);
    //        } else {
    //            Log.LogMessage($"{Consts.ProjectsConfig} file not found, further clean not required");
    //        }
    //        return true;
    //    }
    //}
}
