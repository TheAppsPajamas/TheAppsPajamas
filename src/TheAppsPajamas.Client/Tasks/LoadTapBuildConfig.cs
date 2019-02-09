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
using TheAppsPajamas.Client.JsonDtos;

namespace TheAppsPajamas.Client.Tasks
{
    public class LoadTapBuildConfig : BaseLoadTask
    {
        [Output]
        public ITaskItem TapAppId { get; set; }

        [Output]
        public ITaskItem MediaAccessKey { get; set; }

        public LoadTapBuildConfig()
        {
            _taskName = "LoadTapBuildConfig";
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


            var securitySettings = this.GetSecurity();

            if (securitySettings == null)
            {
                Log.LogError($"{Consts.TapSecurityFile} file not set, please see solution root and complete");
                return false;
            }

            TapAppId = new TaskItem(_tapSetting.TapAppId);

            var token = this.Login(securitySettings);
            if (token == null){
                Log.LogError("Authentication failure");
                return false;
            }

            var unmodifedProjectName = ProjectName.Replace(Consts.ModifiedProjectNameExtra, String.Empty);
            var url = String.Concat(TapSettings.GetMetadata(MetadataType.TapEndpoint), Consts.TapClientEndpoint, "?", "tapAppId=", _tapSetting.TapAppId, "&projectName=", unmodifedProjectName, "&buildConfiguration=", BuildConfiguration );

            LogInformation("Loading tap build config from '{0}'", url);

            string jsonClientConfig = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.SetWebClientHeaders(token);

                    jsonClientConfig = client.DownloadString(url);
                    LogInformation("Successfully loaded tap build config from '{0}', recieved '{1}'", url, jsonClientConfig.Length);
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
                    Log.LogError($"Unknown server api error {response.StatusCode.ToString()}, exiting");
                    //Log.LogErrorFromException(ex);
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.NotFound){
                    Log.LogError($"Build configuration or Tap App Id not found on server, exiting");
                    TapShouldContinue = bool.FalseString;
                    return false;
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

            if (clientConfigDto.AppIcon.Succeeded == false 
                || clientConfigDto.Splash.Succeeded == false)
            {
                foreach(var message in clientConfigDto.AppIcon.Messages.Where(x => x.MessageImportanceTypeValue ==
                    MessageImportanceType.Error.Value))
                {
                    Log.LogError(message.MessageBody);
                }
                foreach (var message in clientConfigDto.Splash.Messages.Where(x => x.MessageImportanceTypeValue ==
                    MessageImportanceType.Error.Value))
                {
                    Log.LogError(message.MessageBody);
                }
                return false;
            }

            MediaAccessKey = new TaskItem(clientConfigDto.MediaAccessKey);
            //this is not quite identical in base

            var projectsConfig = this.GetProjectsConfig();

            var projectConfig = projectsConfig.Projects.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration);

            if (projectConfig == null){
                projectConfig = new ProjectJson();
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
