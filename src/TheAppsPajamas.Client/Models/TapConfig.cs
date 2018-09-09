using System;
namespace TheAppsPajamas.Client.Models
{
    /*
     {
          "TapLogLevel": "Verbose",
          "Endpoint": "http://buildapidebug.me"
     }
     */
    public class TapConfig
    {
        public string TapLogLevel { get; set; }
        public string Endpoint { get; set; }
    }
}
