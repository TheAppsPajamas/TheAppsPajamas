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
        public string ProjectShouldModifyOriginal { get; set; } = bool.FalseString;


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

                //delete files (only from actual project asset catalogue
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

                //delete all resources in theappspajamas folder

                var allItems = project.Xml.ItemGroups.SelectMany(x => x.Items);

                var buildResourceItems = allItems.Where(x => x.Include.Contains(Consts.TheAppsPajamasResourcesDir));

                int filesDeleted = 0;
                foreach(var item in buildResourceItems){
                    item.Parent.RemoveChild(item);
                    filesDeleted++;
                }


                LogDebug("Delete {0} from {1} folder", filesDeleted, Consts.MediaResourcesDir);

                //add files (again asset catalogue stuff)
                var addItemGroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(addItemGroup, project.Xml.LastChild);
                if (FilesToAddToProject != null)
                {
                    foreach (var fileToAdd in FilesToAddToProject)
                    {

                        LogDebug("Added file {1} to {0}", fileToAdd.ItemSpec, fileToAdd.GetMetadata("IncludePath"));
                        addItemGroup.AddItem(fileToAdd.ItemSpec, fileToAdd.GetMetadata("IncludePath"));
                    }
                }
                else
                {
                    LogDebug("No files to add to project");
                }

                if ((FilesToAddToProject != null && FilesToAddToProject.Length != 0)
                    || (FilesToDeleteFromProject != null && FilesToDeleteFromProject.Length != 0))
                {
                    ProjectShouldModifyOriginal = bool.TrueString;
                }

                //always save (because of the removing allpajama resources)
                var withoutExt = Path.Combine(System.IO.Path.GetDirectoryName(ProjectFileLoad), Path.GetFileNameWithoutExtension(ProjectFileLoad));

                ProjectFileSave = String.Concat(withoutExt, Consts.ModifiedProjectNameExtra, ".csproj");
                LogDebug("Saving project to {0}", ProjectFileSave);

                project.Save(ProjectFileSave);

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
