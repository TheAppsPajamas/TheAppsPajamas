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
            var baseResult = base.Execute();
            //try
            //{
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
                        var existingItem = existingItems.FirstOrDefault(x => x.Include == deleteItem.ItemSpec.GetPathRelativeToProject(ProjectDir));

                    //&& x.Include == deleteItem.GetMetadata(MetadataType.DeletePath).GetPathRelativeToProject(ProjectDir));
                    if (existingItem != null)
                    {
                        existingItem.Parent.RemoveChild(existingItem);
                    } else {
                        LogDebug($"File to delete not found in project, not removing {deleteItem.ItemSpec}");
                    }
                        File.Delete(deleteItem.ItemSpec);

                        LogDebug("Removed {0} from project, and deleted", deleteItem.ItemSpec);

                    }
                }

                //don't need to do this anymore because we're programtaically removing them from the build
                //delete all resources in theappspajamas folder



                //var buildResourceItems = allItems.Where(x => x.Include.Contains(Consts.TheAppsPajamasResourcesDir));

                //int filesDeleted = 0;
                //foreach(var item in buildResourceItems){
                //    item.Parent.RemoveChild(item);
                //    filesDeleted++;
                //}


                //LogDebug("Delete {0} from {1} folder", filesDeleted, Consts.MediaResourcesDir);


                //add files (again asset catalogue stuff)
                var addItemGroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(addItemGroup, project.Xml.LastChild);
                if (FilesToAddToProject != null)
                {
                    foreach (var fileToAdd in FilesToAddToProject)
                    {
                        LogDebug("Added file {1} to {0}", fileToAdd.ItemSpec, fileToAdd.GetMetadata(MetadataType.IncludePath));
                        addItemGroup.AddItem(fileToAdd.ItemSpec, fileToAdd.GetMetadata(MetadataType.IncludePath).GetPathRelativeToProject(ProjectDir));
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
                    LogDebug("Project should modify original {0}", ProjectShouldModifyOriginal);
                }

 
                //always save (because of the removing allpajama resources)
                var withoutExt = Path.Combine(System.IO.Path.GetDirectoryName(ProjectFileLoad), Path.GetFileNameWithoutExtension(ProjectFileLoad));

                ProjectFileSave = String.Concat(withoutExt, Consts.ModifiedProjectNameExtra, ".csproj");
                LogDebug("Saving project to {0}", ProjectFileSave);

                project.Save(ProjectFileSave);

                //project.Build();

                //collection.UnregisterAllLoggers();

                //collection.un

            //}
            //catch (Exception ex)
            //{
            //    Log.LogErrorFromException(ex);
            //    return false;

            //}
            return true;
        }
    }
}
