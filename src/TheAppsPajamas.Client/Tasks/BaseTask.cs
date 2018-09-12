using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TheAppsPajamas.Client.Extensions;

namespace TheAppsPajamas.Client.Tasks
{
    public abstract class BaseTask : Task
    {
        public ITaskItem TapSettings { get; set; }

        public string PackagesDir { get; set; }
        public string ProjectDir { get; set; }

        [Output]
        public ITaskItem TapSettingsOutput { get; set; }

        public override bool Execute()
        {
            if (TapSettings == null){
                TapSettings = this.GetTapSettings();
                TapSettingsOutput = TapSettings;
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
