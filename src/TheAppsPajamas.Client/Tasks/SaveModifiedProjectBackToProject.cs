using System;
using System.Collections.Generic;
using System.IO;

namespace TheAppsPajamas.Client.Tasks
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
                File.Delete(ProjectFileModifiedName);
                LogInformation("Modified project saved back to original, This should cause a project reload");

            } catch (Exception ex){
                Log.LogErrorFromException(ex);

            }
            return true;
        }
    }
}
