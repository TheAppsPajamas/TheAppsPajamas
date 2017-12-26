using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public abstract class BaseTask : Task
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public string BuildClientResourceBaseDir { get; set; }

        public bool Debug { get; set; }

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

        protected string _urlBase = "http://buildapidebug.me";

        protected string _endpoint = "/api/client";

        public string _appId;

        [Output]
        public string BuildResourceDir { get; set; }

        public void LogDebug(string message, params object[] messageArgs){

            if (Debug) Log.LogMessage(message, messageArgs);
        }
    }
}
