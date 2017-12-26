using System;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public abstract class BaseTask : Task
    {
        public bool Debug { get; set; }

        public void LogDebug(string message, params object[] messageArgs)
        {

            if (Debug) Log.LogMessage(message, messageArgs);
        }
    }
}
