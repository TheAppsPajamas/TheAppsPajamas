using System;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;

namespace TheAppsPajamas.Client.Extensions
{
    public static class LogExtensions
    {
        public static bool IsVerbose(this BaseTask baseTask)
        {
            if (baseTask.TapSettings == null){
                return true;
            }
            if (baseTask.TapSettings.GetMetadata(MetadataType.TapLogLevel).ToLower() == TapLogLevel.Verbose.ToLower())
            {
                return true;
            }
            return false;

        }

        public static bool IsDebug(this BaseTask baseTask){
            if (baseTask.TapSettings == null)
            {
                return true;
            }
            if (baseTask.TapSettings.GetMetadata(MetadataType.TapLogLevel).ToLower() == TapLogLevel.Debug.ToLower())
            {
                return true;
            }
            else if (baseTask.IsVerbose())
            {
                return true;
            }

            return false;
            
        }

        public static bool IsInformation(this BaseTask baseTask)
        {
            if (baseTask.TapSettings == null)
            {
                return true;
            }

            if (baseTask.TapSettings.GetMetadata(MetadataType.TapLogLevel).ToLower() == TapLogLevel.Information.ToLower())
            {
                return true;
            }
            else if (baseTask.IsDebug())
            {
                return true;
            }
            return false;

        }

        public static bool IsWarning(this BaseTask baseTask)
        {
            if (baseTask.TapSettings == null)
            {
                return true;
            }

            if (baseTask.TapSettings.GetMetadata(MetadataType.TapLogLevel).ToLower() == TapLogLevel.Warn.ToLower())
            {
                return true;
            }
            else if (baseTask.IsInformation())
            {
                return true;
            }

            return false;

        }
    }
}
