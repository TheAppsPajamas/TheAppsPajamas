﻿using System;
using System.IO;
using System.Net;
using Build.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using Build.Client.Extensions;
using System.Linq;
using Build.Client.Constants;

namespace Build.Client.BuildTasks
{
    public class LoadRemoteBuildConfig : BaseLoadTask
    {

        public override bool Execute()
        {


            //going to need to load up the _something_file and get the appId
            LogDebug("Running LoadRemoteBuildConfig in debug");

            LogDebug("Project name '{0}'", ProjectName);
            LogDebug("Build configuration '{0}'", BuildConfiguration);

            BuildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(BuildResourceDir))
                return false;

            var buildResourcesConfig = this.GetResourceConfig();

            if (buildResourcesConfig == null)
                return false;

            var url = String.Concat(Consts.UrlBase, Consts.ClientEndpoint, "?", "appId=", buildResourcesConfig.AppId, "&projectName=", ProjectName, "&buildConfiguration=", BuildConfiguration );

            Log.LogMessage("Loading remote build config from '{0}'", url);

            string jsonClientConfig = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    //client.Credentials = GetConfiguredCredentials();
                    jsonClientConfig = client.DownloadString(url);
                    Log.LogMessage("Successfully loaded remote build config from '{0}', recieved '{1}'", url, jsonClientConfig.Length);
                    LogDebug("Json data recieved\n{0}", jsonClientConfig);
                    //write to file
                    //deserialise now (extension method)
                    //return object
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }


            ClientConfigDto clientConfigDto = this.GetClientConfig(jsonClientConfig);
            if (clientConfigDto == null)
                return false;

            var projectConfig = this.GetProjectConfig();

            //var thisProject = this.GetProjectConfig(projectsConfig);


            projectConfig.ClientConfig = clientConfigDto;
            if (!this.SaveProject(projectConfig))
                return false;
            if (TargetFrameworkIdentifier == "Xamarin.iOS")
            {
                AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig);
                AppIconCatalogueName = this.GetAppIconCatalogueName(projectConfig.ClientConfig);

            }
            PackagingOutput = this.GetPackagingOutput(clientConfigDto);
            AppIconOutput = this.GetAppIconOutput(projectConfig.ClientConfig, AssetCatalogueName, AppIconCatalogueName);
            SplashOutput = this.GetSplashOutput(clientConfigDto);

            return true;
        }
    }
}
