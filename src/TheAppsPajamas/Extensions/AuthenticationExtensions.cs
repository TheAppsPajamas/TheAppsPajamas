using System;
using System.Net;
using System.Text;
using TheAppsPajamas.Tasks;
using TheAppsPajamas.Constants;
using TheAppsPajamas.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using TheAppsPajamas.JsonDtos;

namespace TheAppsPajamas.Extensions
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
                    System.Collections.Specialized.NameValueCollection postData = null;

                    if (String.IsNullOrEmpty(tapSecurity.ServiceUserAccessKey))
                    {
                        postData = new System.Collections.Specialized.NameValueCollection()
                           {
                                { "username", tapSecurity.Username },
                                { "password", tapSecurity.Password },
                                { "grant_type", "password" },
                                { "scope", "openid email plantype profile offline_access roles"},
                                { "resource", "loadremotebuildconfig"}
                           };

                        baseTask.LogDebug("Using grant_type: password");
                    }
                    else
                    {
                        postData = new System.Collections.Specialized.NameValueCollection()
                           {
                                { "password", tapSecurity.ServiceUserAccessKey },
                                { "grant_type", "access_key" },
                                { "scope", "openid email plantype profile offline_access roles"},
                                { "resource", "loadremotebuildconfig"}
                           };
                        baseTask.LogDebug("Using grant_type: access_key");
                    }

                    var tokenResult = Encoding.UTF8.GetString(client.UploadValues(tokenUrl, postData));

                    token = JsonConvert.DeserializeObject<LoginResponseDto>(tokenResult);
                    //client.Credentials = new NetworkCredential(securityConfig.UserName, securityConfig.Password);
                    //var tokenResult = client.DownloadString(tokenUrl);
                    baseTask.LogDebug("Token result recieved <-- value removed from log -->");
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
