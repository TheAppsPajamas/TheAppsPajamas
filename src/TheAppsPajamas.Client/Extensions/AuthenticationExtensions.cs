using System;
using System.Net;
using System.Text;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using TheAppsPajamas.Client.JsonDtos;

namespace TheAppsPajamas.Client.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Login client, and return bearer token
        /// </summary>
        /// <returns>The login.</returns>
        /// <param name="baseTask">Base task.</param>
        public static ITaskItem Login(this BaseTask baseTask, TapSecurityJson tapSecurity)
        {
            LoginResponseDto token;
            //authenticate
            try
            {
                using (WebClient client = new WebClient())
                {
                    var tokenUrl = String.Concat(baseTask.TapSettings.GetMetadata(MetadataType.TapEndpoint), Consts.TokenEndpoint);

                    System.Collections.Specialized.NameValueCollection postData =
                        new System.Collections.Specialized.NameValueCollection()
                       {
                        { "username", tapSecurity.Username },
                        { "password", tapSecurity.Password },
                        { "grant_type", "password" },
                        { "scope", "openid email plantype profile offline_access roles"},
                        { "resource", "loadremotebuildconfig"}

                       };

                    var tokenResult = Encoding.UTF8.GetString(client.UploadValues(tokenUrl, postData));

                    token = JsonConvert.DeserializeObject<LoginResponseDto>(tokenResult);
                    //client.Credentials = new NetworkCredential(securityConfig.UserName, securityConfig.Password);
                    //var tokenResult = client.DownloadString(tokenUrl);
                    baseTask.LogDebug("Token result recieved <-- value removed from log -->", token.access_token);
                    return new TaskItem(token.access_token);
                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
                return null;
            }
        }

        public static void SetWebClientHeaders(this WebClient webClient, ITaskItem token){
            webClient.Headers.Add("Authorization", $"Bearer {token.ItemSpec}");
        }
    }
}
