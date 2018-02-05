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

namespace Build.Client.BuildTasks
{
    public class LoadRemoteBuildConfig : BaseLoadTask
    {

        public override bool Execute()
        {

            Log.LogMessage("Running LoadRemoteBuildConfig");

            LogDebug("Project name '{0}'", ProjectName);
            LogDebug("Build configuration '{0}'", BuildConfiguration);

            BuildResourceDir = this.GetBuildResourceDir();
            if (String.IsNullOrEmpty(BuildResourceDir))
                return false;

            var buildResourcesConfig = this.GetResourceConfig();

            if (buildResourcesConfig == null)
                return false;

            var securityConfig = this.GetSecurityConfig();

            if (buildResourcesConfig == null || securityConfig == null)
                return false;

            LoginResponseDto token;
            //authenticate
            try{
                using (WebClient client = new WebClient())
                {
                    //so this needs to either use a recieved token
                    //or create one,
                    //and the other place we use it, either needds to
                    //use existing token recieved, or authenticate again (this method might not have run)
                    var tokenUrl = String.Concat(Consts.UrlBase, Consts.TokenEndpoint);

                //    let header = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
                //let params = new HttpParams()
                  //.append('username', userName)
                  //.append('password', password)
                  //.append('grant_type', 'password')
                  //.append('scope', 'openid email plantype profile offline_access roles')
                  //.append('resource', window.location.origin);


                    System.Collections.Specialized.NameValueCollection postData =
                        new System.Collections.Specialized.NameValueCollection()
                       {
                        { "username", securityConfig.UserName },
                        { "password", securityConfig.Password },
                        { "grant_type", "password" },
                        { "scope", "openid email plantype profile offline_access roles"},
                        { "resource", "loadremotebuildconfig"}

                       };

     
                    var tokenResult = Encoding.UTF8.GetString(client.UploadValues(tokenUrl, postData));

                    token = JsonConvert.DeserializeObject<LoginResponseDto>(tokenResult);
                    //client.Credentials = new NetworkCredential(securityConfig.UserName, securityConfig.Password);
                    //var tokenResult = client.DownloadString(tokenUrl);
                    LogDebug("Token result recieved\n{0}", token.access_token);
                }
            } catch (Exception ex){
                    Log.LogErrorFromException(ex);
                return false;
            }



            var url = String.Concat(Consts.UrlBase, Consts.ClientEndpoint, "?", "appId=", buildResourcesConfig.AppId, "&projectName=", ProjectName, "&buildConfiguration=", BuildConfiguration );

            Log.LogMessage("Loading remote build config from '{0}'", url);

            string jsonClientConfig = null;
            try
            {
                using (WebClient client = new WebClient())
                {

                    client.Headers.Add("Authorization", $"Bearer {token.access_token}");

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
