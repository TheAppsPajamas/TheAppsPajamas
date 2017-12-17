using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class GetDroidAppIcons : Task
    {
        public GetDroidAppIcons()
        {
        }

        [Output]
        public ITaskItem[] OutputFiles { get; set; }


        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Get Droid App Icons started");

            var itemMetadata = new Dictionary<string, string>();

            itemMetadata.Add("LogicalName", "logical-name.png");
            OutputFiles = new ITaskItem[] {
                new TaskItem("Resources\\mipmap-hdpi\\Icon.png"
                             , new Dictionary<string, string>{ {"LogicalName", "mipmap-hdpi\\Icon.png"}}),
                new TaskItem("..\\mipmap-xxhdpi\\Icon.png"
                             , new Dictionary<string, string>{ {"LogicalName", "mipmap-xxhdpi\\Icon.png"}})
                };

            return true;
        }
    }


}
