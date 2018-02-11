using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public abstract class BaseTask : Task
    {
        public bool Debug { get; set; }
        public string PackagesDir { get; set; }
        public string ProjectDir { get; set; }

        public void LogDebug(string message, params object[] messageArgs)
        {

            if (Debug) Log.LogMessage(message, messageArgs);
        }
    }
}
