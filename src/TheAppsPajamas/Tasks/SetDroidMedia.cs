using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Constants;
using TheAppsPajamas.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TheAppsPajamas.Tasks
{
    public class SetDroidMedia : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        public ITaskItem[] SplashFields { get; set; }

        public ITaskItem[] ExistingAndroidResources { get; set; }
        public ITaskItem[] ExistingTapAssets { get; set; }

        public ITaskItem AppIconHolder { get; set; }
        public ITaskItem SplashHolder { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputAndroidResources { get; set; }

        public string BuildConfiguration { get; set; }
        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Set Droid Media started");

            var filesToAddToModifiedProject = new List<ITaskItem>();
            var outputAndroidResources = new List<ITaskItem>();

            var existingAssets = new List<ITaskItem>();

            if (ExistingAndroidResources != null && ExistingAndroidResources.Length != 0)
            {
                existingAssets.AddRange(ExistingAndroidResources);
            }
            else if (ExistingAndroidResources == null || ExistingAndroidResources.Length == 0)
            {
                LogDebug("No existing android resource assets found in project");
            }

            if (ExistingTapAssets != null && ExistingTapAssets.Length != 0)
            {
                existingAssets.AddRange(ExistingTapAssets);
            }
            else if (ExistingTapAssets == null || ExistingTapAssets.Length == 0)
            {
                LogDebug("No existing tap assets found in project");
            }

            if (this.IsVerbose())
            {
                foreach (var taskItem in existingAssets)
                {
                    LogVerbose("Existing asset in project {0}", taskItem.ItemSpec);
                }
            }

            var allMediaFields = new List<ITaskItem>();

            if (AppIconHolder.IsDisabled())
            {
                LogInformation($"App icons are disabled in this configuration");
            } else {
                LogDebug($"App icons are enabled in this configuration");
                allMediaFields.AddRange(AppIconFields);
            }
            if (SplashHolder.IsDisabled())
            {
                LogInformation($"Splash screens are disabled in this configuration");
            }
            else
            {
                LogDebug($"Splash screens are enabled in this configuration");
                allMediaFields.AddRange(SplashFields);
            }

            //left out because it might give us an empty array and null out
            if (AppIconHolder.IsDisabled() && SplashHolder.IsDisabled()){

                Log.LogMessage($"Both media types are disabled in this configuration, SetDroidMedia does not need to continue");
                return true;
            }

            var buildConfigAssetDir = this.GetBuildConfigurationAssetDir(BuildConfiguration);

            foreach(var field in allMediaFields){
                if (field.IsDisabled()){
                    if (field.HolderIsEnabled())
                    {
                        Log.LogMessage($"{field.GetMetadata(MetadataType.FieldDescription)} is disabled in this configuration");
                    }
                    //always continue 
                    continue;
                }

                var existingFilePath = Path.Combine(buildConfigAssetDir, field.GetMetadata(MetadataType.TapAssetPath), field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                var outputDir = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.ProjectAssetPath));

                if (!Directory.Exists(outputDir)){
                    Directory.CreateDirectory(outputDir);
                    LogDebug("Create asset folder at {0}", outputDir);
                }
                                             
                if (existingAssets.FirstOrDefault(x => x.ItemSpec.StripSlashes() == existingFilePath.GetPathRelativeToProject(ProjectDir).StripSlashes()) == null)
                {
                    LogDebug($"Adding {existingFilePath} as a {MSBuildItemName.TapAsset} to add to project list as it is not in current project");
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.TapAsset, new Dictionary<string, string> { { MetadataType.IncludePath, existingFilePath } }));
                }

                var outputFilePath = Path.Combine(ProjectDir, field.GetMetadata(MetadataType.ProjectAssetPath), field.GetMetadata(MetadataType.LogicalName));

                if (existingAssets.FirstOrDefault(x => x.ItemSpec.StripSlashes() == outputFilePath.GetPathRelativeToProject(ProjectDir).StripSlashes()) == null)
                {
                    LogDebug($"Adding {outputFilePath} as a {MSBuildItemName.AndroidResource} to add to project list as it is not in current project");
                    filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.AndroidResource, new Dictionary<string, string> { { MetadataType.IncludePath, outputFilePath } }));
                }

                File.Copy(existingFilePath, outputFilePath, true);

                LogDebug($"Including {MSBuildItemName.TapAsset} from {existingFilePath} to {outputFilePath} as {MSBuildItemName.AndroidResource}");
                outputAndroidResources.Add(new TaskItem(outputFilePath));
            }

            FilesToAddToProject = filesToAddToModifiedProject.ToArray();
            OutputAndroidResources = outputAndroidResources.ToArray();
            return true;
        }
    }


}
