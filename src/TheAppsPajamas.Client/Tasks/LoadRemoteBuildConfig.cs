using System;
using System.IO;
using System.Net;
using TheAppsPajamas.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using TheAppsPajamas.Client.Extensions;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using System.Text;
using TheAppsPajamas.Shared.Types;
using System.Collections.Generic;

namespace TheAppsPajamas.Client.Tasks
{
    public class LoadRemoteBuildConfig : BaseLoadTask
    {
        [Output]
        public ITaskItem Token { get; set; }

        [Output]
        public ITaskItem BuildAppId { get; set; }

        public LoadRemoteBuildConfig()
        {
            _taskName = "LoadRemoteBuildConfig";
        }

        public override bool Execute()
        {
            var baseResult = base.Execute();
            if (baseResult == false){
                return false;
            }

            TapAssetDir = this.GetAssetDir();
            if (String.IsNullOrEmpty(TapAssetDir))
            {
                Log.LogError($"{Consts.TapAssetsDir} folder not found, exiting");
                return false;
            }


            var securityConfig = this.GetSecurityConfig();

            if (securityConfig == null)
            {
                Log.LogError($"{Consts.TapSecurityConfig} file not set, please see solution root and complete");
                return false;
            }

            BuildAppId = new TaskItem(_tapSetting.TapAppId.ToString());

            Token = this.Login(securityConfig);
            if (Token == null){
                Log.LogError("Authentication failure");
                return false;
            }

            var unmodifedProjectName = ProjectName.Replace(Consts.ModifiedProjectNameExtra, String.Empty);
            var url = String.Concat(Consts.UrlBase, Consts.ClientEndpoint, "?", "appId=", _tapSetting.TapAppId, "&projectName=", unmodifedProjectName, "&buildConfiguration=", BuildConfiguration );

            LogInformation("Loading remote build config from '{0}'", url);

            string jsonClientConfig = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.SetWebClientHeaders(Token);

                    jsonClientConfig = client.DownloadString(url);
                    LogInformation("Successfully loaded remote build config from '{0}', recieved '{1}'", url, jsonClientConfig.Length);
                    LogDebug("Json data recieved\n{0}", jsonClientConfig);
                    //write to file
                    //deserialise now (extension method)
                    //return object
                }
            }
            catch (WebException ex){
                var response = ex.Response as HttpWebResponse;
                if (response == null)
                {
                    LogDebug("Webexception not ususal status code encountered, fatal, exiting");
                    Log.LogErrorFromException(ex);
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.NotFound){
                    LogWarning($"Tap server responded with message '{response.StatusDescription}', TheAppsPajamams cannot continue, exiting gracefully, build will continue");
                    TapShouldContinue = bool.FalseString;
                    return true;
                }

                //TODO load client config from projects if no web available and run anyway
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }


            ClientConfigDto clientConfigDto = this.GetClientConfig(jsonClientConfig);
            if (clientConfigDto == null)
                return false;

            //this is not quite identical in base

            var projectsConfig = this.GetProjectsConfig();

            var projectConfig = projectsConfig.Projects.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration);

            if (projectConfig == null){
                projectConfig = new ProjectConfig();
                projectsConfig.Projects.Add(projectConfig);
            }

            projectConfig.BuildConfiguration = BuildConfiguration;
            projectConfig.ClientConfig = clientConfigDto;
            if (!this.SaveProjects(projectsConfig))
                return false;

            //this is identical in base

            AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig, TargetFrameworkIdentifier);

            BuildConfigHolderOutput = this.GetHolderOutput(projectConfig.ClientConfig.BuildConfig, "Build config");
            PackagingHolderOutput = this.GetHolderOutput(projectConfig.ClientConfig.Packaging, "Packaging");
            AppIconHolderOutput = this.GetHolderOutput(projectConfig.ClientConfig.AppIcon, "App icon");
            SplashHolderOutput = this.GetHolderOutput(projectConfig.ClientConfig.Splash, "Splash");

            AppIconFieldOutput = this.GetMediaFieldOutput(projectConfig.ClientConfig.AppIcon.Fields, AssetCatalogueName, projectConfig.ClientConfig, AppIconHolderOutput);

            SplashFieldOutput = this.GetMediaFieldOutput(projectConfig.ClientConfig.Splash.Fields, AssetCatalogueName, projectConfig.ClientConfig, SplashHolderOutput);

            PackagingFieldOutput = this.GetStringFieldOutput(projectConfig.ClientConfig.Packaging.Fields, PackagingHolderOutput);




            return true;
        }
    }
}
