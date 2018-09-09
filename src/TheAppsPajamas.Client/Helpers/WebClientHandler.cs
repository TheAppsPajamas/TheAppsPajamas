using System;
using System.Net;

namespace TheAppsPajamas.Client.Helpers
{
    public static class WebClientHandler
    {
        private static WebClient _instance;
        public static WebClient WebClient
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WebClient();
                }
                return _instance;
            }
        }
    }
}
