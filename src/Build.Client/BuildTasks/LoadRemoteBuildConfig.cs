using System;
using System.IO;
using System.Net;
using Build.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using Build.Client.Extensions;
using System.Linq;
using Build.Client.Constants;
using System.Text;
using Build.Shared.Types;
using System.Collections.Generic;

namespace Build.Client.BuildTasks
{
    public class LoadRemoteBuildConfig : BaseLoadTask
    {
        [Output] 
        public ITaskItem Token { get; set; }

        [Output]
        public ITaskItem BuildAppId { get; set; }

        public override bool Execute()
        {

            Log.LogMessage("Running LoadRemoteBuildConfig");

            LogDebug("Project name '{0}'", ProjectName);
            LogDebug("Build configuration '{0}'", BuildConfiguration);

            TheAppsPajamasResourceDir = new TaskItem(Consts.TheAppsPajamasResourcesDir);
            TapShouldContinue = bool.TrueString;

            var tapResourcesConfig = this.GetResourceConfig();

            var securityConfig = this.GetSecurityConfig();

            if (tapResourcesConfig == null || securityConfig == null)
            {
                //Change to warning, and return TapShouldContinue = false
                Log.LogError($"{Consts.TapResourcesConfig} file not set, please see solution root and complete");
                return false;
            }


            if (tapResourcesConfig.BuildConfigs == null)
            {
                LogDebug("Added BuildConfigs list");
                tapResourcesConfig.BuildConfigs = new List<BuildConfig>();
            }

            var thisBuildConfig = tapResourcesConfig.BuildConfigs.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration
                                                                                   && x.ProjectName == ProjectName);

            if (thisBuildConfig == null)
            {
                Log.LogMessage($"Project {ProjectName} Build configuration {BuildConfiguration} not found, so adding to {Consts.TapResourcesConfig}");
                tapResourcesConfig.BuildConfigs.Add(new BuildConfig(ProjectName, BuildConfiguration));
                this.SaveResourceConfig(tapResourcesConfig);
            }
            else if (thisBuildConfig.Disabled == true)
            {
                Log.LogMessage($"The Apps Pajamas is disabled in {Consts.TapResourcesConfig} for Project {ProjectName} in configuration {BuildConfiguration}], exiting");
                TapShouldContinue = bool.FalseString;
                return true;
            }



            BuildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(BuildResourceDir))
                return false;


            BuildAppId = new TaskItem(tapResourcesConfig.TapAppId.ToString());

            Token = this.Login(securityConfig);
            if (Token == null){
                Log.LogError("Authentication failure");
                return false;
            }

            var unmodifedProjectName = ProjectName.Replace(Consts.ModifiedProjectNameExtra, String.Empty);
            var url = String.Concat(Consts.UrlBase, Consts.ClientEndpoint, "?", "appId=", tapResourcesConfig.TapAppId, "&projectName=", unmodifedProjectName, "&buildConfiguration=", BuildConfiguration );

            Log.LogMessage("Loading remote build config from '{0}'", url);

            string jsonClientConfig = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.SetWebClientHeaders(Token);

                    jsonClientConfig = client.DownloadString(url);
                    Log.LogMessage("Successfully loaded remote build config from '{0}', recieved '{1}'", url, jsonClientConfig.Length);
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
                    Log.LogWarning($"Tap server responded with message '{response.StatusDescription}', TheAppsPajamams cannot continue, exiting gracefully, build will continue");
                    TapShouldContinue = bool.FalseString;
                    return true;
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
            
            StringFieldClientDto packagingAppIconField = null;
            StringFieldClientDto packagingSplashField = null;

            ITaskItem packagingCatalogueSetName = null;
            if (TargetFrameworkIdentifier == "Xamarin.iOS")
            {
                AssetCatalogueName = this.GetAssetCatalogueName(projectConfig.ClientConfig);
                AppIconCatalogueName = this.GetAppIconCatalogueSetName(projectConfig.ClientConfig);
                packagingCatalogueSetName = this.GetSplashCatalogueSetName(projectConfig.ClientConfig);

            } else if (TargetFrameworkIdentifier == "MonoAndroid"){
                packagingAppIconField = projectConfig.ClientConfig.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidAppIconName.Value);

                packagingSplashField = projectConfig.ClientConfig.PackagingFields.FirstOrDefault(x => x.FieldId == FieldType.PackagingDroidSplashName.Value);
            }


            AppIconOutput = this.GetMediaOutput(projectConfig.ClientConfig.AppIconFields, AssetCatalogueName, projectConfig.ClientConfig);

            SplashOutput = this.GetMediaOutput(projectConfig.ClientConfig.SplashFields, AssetCatalogueName, projectConfig.ClientConfig);

            PackagingOutput = this.GetFieldTypeOutput(projectConfig.ClientConfig.PackagingFields);


            return true;
        }
    }
}
