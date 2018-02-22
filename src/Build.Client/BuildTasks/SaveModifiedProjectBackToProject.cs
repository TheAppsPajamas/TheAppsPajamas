using System;
using System.Collections.Generic;
using System.IO;
using Build.Client.Constants;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class SaveModifiedProjectBackToProject : BaseTask
    {
        public string ProjectFileOriginalName { get; set; }

        public string ProjectFileModifiedName { get; set; }


        public override bool Execute()
        {
            var baseResult = base.Execute();
            try
            {
                LogInformation("Saving modified project {0} to {1}", ProjectFileModifiedName, ProjectFileOriginalName);

                File.Copy(ProjectFileModifiedName, ProjectFileOriginalName, true);

                LogInformation("Modified project saved back to original, Reload project now required");
                //collection.un

            } catch (Exception ex){
                Log.LogErrorFromException(ex);

            }
            return true;
        }
    }
}
