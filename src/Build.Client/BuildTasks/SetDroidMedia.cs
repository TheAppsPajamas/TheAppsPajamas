using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.Constants;
using Build.Client.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public class SetDroidMedia : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }

        public ITaskItem[] ExistingAndroidResources { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputAndroidAssets { get; set; }

        public string BuildConfiguration { get; set; }
        public override bool Execute()
        {
            Log.LogMessage("Set Droid Media started");

            var filesToAddToModifiedProject = new List<ITaskItem>();
            var outputAndroidAssets = new List<ITaskItem>();

            var existingAssets = new List<ITaskItem>();

            if (ExistingAndroidResources != null && ExistingAndroidResources.Length != 0)
            {
                existingAssets.AddRange(ExistingAndroidResources);
            }
            else if (ExistingAndroidResources == null || ExistingAndroidResources.Length == 0)
            {
                LogDebug("No existing android resource assets found in project");
            }

            foreach (var taskItem in existingAssets)
            {
                LogDebug("Existing asset in project {0}", taskItem.ItemSpec);
            }

            var allMediaFields = new List<ITaskItem>();
            allMediaFields.AddRange(AppIconFields);
            allMediaFields.AddRange(SplashFields);

            var buildConfigurationResourceDir = this.GetBuildConfigurationResourceDir(BuildConfiguration);

            foreach(var field in allMediaFields){
                var existingFilePath = Path.Combine(buildConfigurationResourceDir, field.GetMetadata(MetadataType.Path), field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                var outputDir = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.Path));

                if (!Directory.Exists(outputDir)){
                    Directory.CreateDirectory(outputDir);
                    LogDebug("Create resource folder at {0}", outputDir);
                }
                                             
                if (existingAssets.FirstOrDefault(x => x.ItemSpec == existingFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", existingFilePath);
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.AndroidResource, new Dictionary<string, string> { { MetadataType.IncludePath, existingFilePath } }));
                }

                var outputFilePath = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.Path), field.GetMetadata(MetadataType.LogicalName));

                if (existingAssets.FirstOrDefault(x => x.ItemSpec == outputFilePath.GetPathRelativeToProject(ProjectDir)) == null)
                {
                    LogDebug("Adding {0} to add to project list as it is not in current project", outputFilePath);
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.AndroidResource, new Dictionary<string, string> { { MetadataType.IncludePath, outputFilePath } }));
                }

                File.Copy(existingFilePath, outputFilePath, true);

                LogDebug("Added {2} from {0} to {1}", existingFilePath, outputFilePath, MSBuildItemName.AndroidResource);
                outputAndroidAssets.Add(new TaskItem(outputFilePath));
            }

            FilesToAddToProject = filesToAddToModifiedProject.ToArray();
            OutputAndroidAssets = outputAndroidAssets.ToArray();
            return true;
        }
    }


}
