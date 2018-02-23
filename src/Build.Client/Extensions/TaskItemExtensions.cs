using System;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Microsoft.Build.Framework;

namespace Build.Client.Extensions
{
    public static class TaskItemExtensions
    {
        public static void SetDisabledMetadata(this ITaskItem taskItem, BaseTask baseTask, bool disabled, string description){
            if (disabled)
            {
                baseTask.LogWarning($"{description} is disabled");
                taskItem.SetMetadata(MetadataType.Disabled, bool.TrueString);
            }
            else
            {
                baseTask.LogDebug($"{description} is enabled");
                taskItem.SetMetadata(MetadataType.Disabled, bool.FalseString);
            }
        }

        public static bool IsDisabled(this ITaskItem taskItem){
            if (taskItem.GetMetadata(MetadataType.Disabled) == bool.TrueString){
                return true;
            }
            return false;
        }

        public static bool IsEnabled(this ITaskItem taskItem)
        {
            if (taskItem.GetMetadata(MetadataType.Disabled) == bool.TrueString)
            {
                return false;
            }
            return true;
        }

        public static bool IsTrue(this ITaskItem taskItem, BaseTask baseTask){
            if (taskItem.GetMetadata("Value") == "1")
            {
                baseTask.LogDebug($"Value is true for boolean task item {taskItem.ItemSpec}");
                return true;
            }

            return false;
        }
    }
}
