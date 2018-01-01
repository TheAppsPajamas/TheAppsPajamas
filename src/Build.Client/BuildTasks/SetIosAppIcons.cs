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
    public class SetIosAppIcons : BaseTask
    {
        public ITaskItem[] AppIconFields { get; set; }

        [Output]
        public ITaskItem[] OutputFiles { get; set; }


        public string BuildConfiguration { get; set; }
        public override bool Execute()
        {
            Log.LogMessage("Set Ios App Icons started");

            try
            {
                var mediaResourcesDir = this.GetMediaResourceDir(BuildConfiguration);

                var outputFiles = new List<ITaskItem>();

                var firstField = AppIconFields.FirstOrDefault();
                if (firstField == null)
                {
                    Log.LogError("App Icon Field set malformed");
                }
                var assetCatalogue = Path.Combine(mediaResourcesDir, firstField.GetMetadata("AssetCatalogueName"), "Contents.json");


                if (!File.Exists(assetCatalogue)){
                    LogDebug("Creating Asset catalogue Contents.json");
                    File.WriteAllText(assetCatalogue, Consts.AssetCatalogueContents);
                }

                var catMetadata = new Dictionary<string, string>();
                catMetadata.Add("LogicalName", Path.Combine(firstField.GetMetadata("AssetCatalogueName"), "Contents.json"));
                var catItem = new TaskItem(assetCatalogue, catMetadata);
                outputFiles.Add(catItem);

                var appIconSet = firstField.GetMetadata("AppIconSetName");
                var appIconSetMetadata = new Dictionary<string, string>();
                appIconSetMetadata.Add("LogicalName", Path.Combine(firstField.GetMetadata("AppIconSetName"), "Contents.json"));
                var appIconSetItem = new TaskItem(assetCatalogue, catMetadata);
                outputFiles.Add(appIconSetItem);

                foreach (var field in AppIconFields)
                {
                    var filePath = Path.Combine(mediaResourcesDir, field.GetMetadata("Path"), String.Concat(field.GetMetadata("MediaName"), ".png"));

                    var itemMetadata = new Dictionary<string, string>();
                    itemMetadata.Add("LogicalName", field.GetMetadata("LogicalName"));

                    LogDebug("Added Logical file {0} to processing list from media-resource source {1}", field.GetMetadata("LogicalName"), filePath);
                    var taskItem = new TaskItem(filePath, itemMetadata);
                    outputFiles.Add(taskItem);
                }

                OutputFiles = outputFiles.ToArray();
                return true;
            } catch (Exception ex){
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }


}
