using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.Constants;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class ModifyProject : BaseTask
    {
        public string ProjectFileLoad { get; set; }

        public ITaskItem[] FilesToDeleteFromProject { get; set; }


        public ITaskItem[] FilesToAddToProject { get; set; }


        [Output]
        public string ProjectFileSave { get; set; }

        [Output]
        public string ProjectNeedsSave { get; set; } = bool.FalseString;

        public override bool Execute()
        {
            try
            {
                //List<ILogger> loggers = new List<ILogger>();
                //loggers.Add(new ConsoleLogger());

                var collection = new ProjectCollection();
                //collection.RegisterLoggers(loggers);


                LogDebug("Loading project {0}", ProjectFileLoad);
                var project = collection.LoadProject(ProjectFileLoad);

                //project.Xml.I


                if (FilesToDeleteFromProject != null)
                {
                    var existingItems = project.Xml.ItemGroups.SelectMany(x => x.Items);
                    //think this should work, won't be able to test until we have are further along
                    foreach (var deleteItem in FilesToDeleteFromProject)
                    {
                        var existingItem = existingItems.FirstOrDefault(x => x.ItemType == deleteItem.ItemSpec
                                                                        && x.Include == deleteItem.GetMetadata("DeletePath"));

                        existingItem.Parent.RemoveChild(existingItem);

                        LogDebug("Removed {0} from project", deleteItem.GetMetadata("DeletePath"));

                    }
                }


                try
                {
                    var slItemGroup = project.Xml.CreateItemGroupElement();
                    project.Xml.InsertAfterChild(slItemGroup, project.Xml.LastChild);
                    if (FilesToAddToProject != null)
                    {
                        foreach (var fileToAdd in FilesToAddToProject)
                        {
                            try
                            {

                                LogDebug("Added file {1} to {0}", fileToAdd.ItemSpec, fileToAdd.GetMetadata("IncludePath"));
                                slItemGroup.AddItem(fileToAdd.ItemSpec, fileToAdd.GetMetadata("IncludePath"));
                            }
                            catch (Exception e)
                            {
                                Log.LogError("Error adding item to group {0}", fileToAdd.ItemSpec);
                                throw e;
                            }
                        }
                    }
                    else
                    {
                        LogDebug("FilesToAddToProject is null");
                    }




                    if ((FilesToAddToProject != null && FilesToAddToProject.Length != 0)
                        || (FilesToDeleteFromProject != null && FilesToDeleteFromProject.Length != 0))
                    {


                        var withoutExt = Path.Combine(System.IO.Path.GetDirectoryName(ProjectFileLoad), Path.GetFileNameWithoutExtension(ProjectFileLoad));

                        ProjectFileSave = String.Concat(withoutExt, Consts.ModifiedProjectNameExtra, ".csproj");
                        LogDebug("Saving project to {0}", ProjectFileSave);
                        ProjectNeedsSave = bool.TrueString;
                        project.Save(ProjectFileSave);
                    }

                }
                catch (Exception ex)
                {
                    Log.LogError("Error inserting children into item group");
                    Log.LogErrorFromException(ex);
                }
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
