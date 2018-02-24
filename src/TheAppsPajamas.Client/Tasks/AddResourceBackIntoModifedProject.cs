using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Extensions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace TheAppsPajamas.Client.Tasks
{
    public class AddResourceBackIntoModifedProject : BaseTask
    {
        public string ProjectFileModifiedName { get; set; }

        [Output]
        public string ProjectNeedsSave { get; set; }

        public override bool Execute()
        {
            try
            {
                //List<ILogger> loggers = new List<ILogger>();
                //loggers.Add(new ConsoleLogger());

                var collection = new ProjectCollection();
                //collection.RegisterLoggers(loggers);


                LogDebug("Loading project {0}", ProjectFileModifiedName);
                var project = collection.LoadProject(ProjectFileModifiedName);

                string[] allMediaFiles = Directory.GetFiles(Path.Combine(ProjectDir, Consts.TapResourcesDir, Consts.MediaResourcesDir)
                                                       , "*.*", SearchOption.AllDirectories);

                if (allMediaFiles == null){
                    return true;
                }

                //add files (again asset catalogue stuff)
                var addItemGroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(addItemGroup, project.Xml.LastChild);

                foreach (var fileToAdd in allMediaFiles)
                {
                    string itemSpec = String.Empty;
                    if (fileToAdd.Contains("iTunesArtwork"))
                    {
                        itemSpec = "iTunesArtwork";
                    }
                    else
                    {
                        itemSpec = "ImageAsset";
                    }

                    LogDebug("Added file {1} to {0}", itemSpec, fileToAdd);
                    addItemGroup.AddItem(itemSpec, fileToAdd.GetPathRelativeToProject(ProjectDir));
                }

                LogDebug("Saving modified project");

                project.Save();

                ProjectNeedsSave = bool.TrueString;

                //project.Build();

                //collection.UnregisterAllLoggers();

                //collection.un

            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);

            }
            return true;
        }
    }
}
