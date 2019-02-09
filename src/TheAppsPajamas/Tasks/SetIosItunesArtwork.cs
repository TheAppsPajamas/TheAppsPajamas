using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Constants;
using TheAppsPajamas.Extensions;
using TheAppsPajamas.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

namespace TheAppsPajamas.Tasks
{
    public class SetIosItunesArtwork : BaseTask
    {
        public string BuildConfiguration { get; set; }


        public ITaskItem AppIconHolder { get; set; }
        public ITaskItem[] AppIconFields { get; set; }
        public ITaskItem[] ExistingItunesArtwork { get; set; }
        public ITaskItem[] ExistingFilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] FilesToAddToProject { get; set; }

        [Output]
        public ITaskItem[] OutputItunesArtwork { get; set; }


        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation("Set Ios iTunes Artwork started");

            //turns out msbuild adds to the output array itself, so if we send the output back down, and into the same 
            //item property it will get added
            var filesToAddToModifiedProject = new List<ITaskItem>();


            //this above discovery will change this
            var outputItunesArtwork = new List<ITaskItem>();


            var existingAssets = new List<ITaskItem>();

            if (ExistingItunesArtwork != null && ExistingItunesArtwork.Length != 0)
            {
                existingAssets.AddRange(ExistingItunesArtwork);
            }
            else if (ExistingItunesArtwork == null || ExistingItunesArtwork.Length == 0)
            {
                LogDebug("No existing iTunes Artwork found in project");
            }

            if (this.IsVerbose())
            {
                foreach (var taskItem in existingAssets)
                {
                    LogVerbose("Existing asset in project {0}", taskItem.ItemSpec);
                }
            }

            var buildConfigAssetDir = this.GetBuildConfigurationAssetDir(BuildConfiguration);

            //could handle disbled here
            var firstField = AppIconFields.FirstOrDefault();
            if (firstField == null)
            {
                Log.LogError("App Icon Field set malformed");
            }


            var allFields = new List<ITaskItem>();
            allFields.AddRange(AppIconFields);


            foreach (var field in allFields.Where(x => x.GetMetadata(MetadataType.MSBuildItemType) == MSBuildItemName.ITunesArtwork))
            {
                if (field.IsDisabled())
                {
                    if (field.HolderIsEnabled())
                    {
                        Log.LogMessage($"{field.GetMetadata(MetadataType.FieldDescription)} is disabled in this configuration");
                    }
                    //always continue 
                    continue;
                }
             
                if (String.IsNullOrEmpty(field.GetMetadata(MetadataType.Idiom)))
                {

                    var existingFilePath = Path.Combine(buildConfigAssetDir, Consts.iTunesArtworkDir, field.GetMetadata(MetadataType.MediaName).ApplyPngExt());

                    var projectOutputFilePath = Path.Combine(base.ProjectDir, field.GetMetadata(MetadataType.LogicalName));

                    if (existingAssets.FirstOrDefault(x => x.ItemSpec.StripSlashes() == projectOutputFilePath.GetPathRelativeToProject(ProjectDir).StripSlashes()) == null)
                    {
                        LogDebug("Adding {0} to add to project list as it is not in current project", existingFilePath);
                        filesToAddToModifiedProject.Add(new TaskItem(MSBuildItemName.ITunesArtwork, new Dictionary<string, string> { { MetadataType.IncludePath, projectOutputFilePath } }));
                    }


                    File.Copy(existingFilePath, projectOutputFilePath, true);
                    //outputITunesArtwork.Add(new TaskItem(packagesOutputFilePath));
                    var artworkTaskItem = new TaskItem(field.GetMetadata(MetadataType.LogicalName));
                    outputItunesArtwork.Add(artworkTaskItem);
                    LogDebug("Added {2} from {0} to {1}", existingFilePath, projectOutputFilePath, MSBuildItemName.ITunesArtwork);

                    continue;
                }

            }

            LogInformation("Set iTunesArtwork wants to add {0} files to the build project", filesToAddToModifiedProject.Count());
            //LogInformation("Asset Catalogue Sets wants to show {0} media-resources files in the final project", filesToAddToModifiedProject.Count());

            FilesToAddToProject = filesToAddToModifiedProject.ToArray();

            OutputItunesArtwork = outputItunesArtwork.ToArray();

            return true;
        }


    }


}
