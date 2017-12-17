using System;
using System.IO;
using System.Net;
using Build.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace Build.Client.BuildTasks
{
    public class LoadRemoteBuildConfig : Task
    {
        public LoadRemoteBuildConfig()
        {
        }

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

        private string _urlBase = "http://buildapidebug.me";

        private string _endpoint = "/api/client/get";


        public string _appId;

        [Output]
        public string BuildResourceDir { get; set; }

        public override bool Execute()
        {
            //going to need to load up the _something_file and get the appId
            if (Debug == true) Log.LogMessage("Running LoadRemoteBuildConfig in debug");

            if (Debug == true) Log.LogMessage("Project name '{0}'", ProjectName);
            if (Debug == true) Log.LogMessage("Build configuration '{0}'", BuildConfiguration);

            if (Debug == true) Log.LogMessage("Loading build-config.json file");

            if (Debug == true) Log.LogMessage("BuildClientResourceBaseDir located at '{0}'", BuildClientResourceBaseDir);

            BuildResourceDir = Path.Combine(BuildClientResourceBaseDir, "build-resources");

            if (!Directory.Exists(BuildResourceDir)){
                if (Debug == true) Log.LogMessage("Created BuildClientResourceBase at '{0}'", BuildResourceDir);
                Directory.CreateDirectory(BuildResourceDir);
            } else {
                if (Debug) Log.LogMessage("BuildClientResourceDir location at '{0}'", BuildResourceDir);
            }

            var buildResourcesConfigPath = Path.Combine(Directory.GetParent(BuildResourceDir).ToString(), "build-resources.config");
            BuildResourcesConfig buildResourcesConfig = null;
            if (!File.Exists(buildResourcesConfigPath)){
                Log.LogMessage("Creating blank build resources config at {0}", buildResourcesConfigPath);
                buildResourcesConfig = new BuildResourcesConfig();
                var json = JsonConvert.SerializeObject(buildResourcesConfig);
                File.WriteAllText(buildResourcesConfigPath, json);
                Log.LogError("Build resources config file not found, created at {0}. Please complete appId, username, and password and restart build process", buildResourcesConfigPath);
                return false;
            } else {
                var json = File.ReadAllText(buildResourcesConfigPath);
                buildResourcesConfig = JsonConvert.DeserializeObject<BuildResourcesConfig>(json);

                if (buildResourcesConfig.AppId == null){
                    Log.LogError("Build resources config appId is null, please complete appId, username, and password at {0} and restart build process", buildResourcesConfigPath);
                    return false;
                }
                if (Debug) Log.LogMessage("Build resources config file read\nAppId '{0}'\nUsername '{1}'", buildResourcesConfig.AppId, buildResourcesConfig.UserName);

            }

            var url = String.Concat(_urlBase, _endpoint, "?", "appId=", buildResourcesConfig.AppId, "&projectName=", ProjectName, "&buildConfiguration=", BuildConfiguration );


            Log.LogMessage("Loading remote build config {0}", url);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            return true;
        }
    }
}
