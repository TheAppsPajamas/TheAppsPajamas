using System;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class HelloWorld : Microsoft.Build.Utilities.Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Hello world this worked");
            return true;
        }
    }
}
