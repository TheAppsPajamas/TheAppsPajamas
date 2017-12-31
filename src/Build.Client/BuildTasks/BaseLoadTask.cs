using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public abstract class BaseLoadTask : BaseTask
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }




        //this should connect to remote server
        //get it's data
        //save it to the build-resources folder in the solution folder
        //and return the same string of variables that the loadlocal returns?
        //or should somehow trigger that task - which would then have to load the file of disc
        //but that shouldn't matter too much really?

        //it's going to need to know
        //solution folder
        //project name
        //possbile frameworkid


        public string _appId;

        [Output]
        public string BuildResourceDir { get; set; }

        [Output]
        public ITaskItem[] PackagingOutput { get; set; }

        [Output]
        public ITaskItem[] AppIconOutput { get; set; }

        [Output]
        public ITaskItem[] SplashOutput { get; set; }
    }
}
