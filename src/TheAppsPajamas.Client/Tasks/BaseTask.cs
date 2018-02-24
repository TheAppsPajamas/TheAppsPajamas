using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TheAppsPajamas.Client.Extensions;

namespace TheAppsPajamas.Client.Tasks
{
    public abstract class BaseTask : Task
    {
        public ITaskItem TapConfig { get; set; }

        public string PackagesDir { get; set; }
        public string ProjectDir { get; set; }

        [Output]
        public ITaskItem TapConfigOutput { get; set; }

        public override bool Execute()
        {
            if (TapConfig == null){
                TapConfig = this.GetTapConfig();
                TapConfigOutput = TapConfig;
            }
            return true;
        }

        public void LogDebug(string message, params object[] messageArgs)
        {

            if (this.IsDebug()) Log.LogMessage(message, messageArgs);
        }

        public void LogVerbose(string message, params object[] messageArgs)
        {

            if (this.IsVerbose()) Log.LogMessage(message, messageArgs);
        }

        public void LogInformation(string message, params object[] messageArgs)
        {

            if (this.IsInformation()) Log.LogMessage(message, messageArgs);
        }

        public void LogWarning(string message, params object[] messageArgs)
        {

            if (this.IsWarning()) Log.LogMessage(message, messageArgs);
        }
    }
}
