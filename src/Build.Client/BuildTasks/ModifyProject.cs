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


        [Output]
        public string ProjectFileSave { get; set; }



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

                var existingItems = project.Xml.ItemGroups.SelectMany(x => x.Items);

                //think this should work, won't be able to test until we have are further along
                foreach(var deleteItem in FilesToDeleteFromProject){
                    var existingItem = existingItems.FirstOrDefault(x => x.ItemType == deleteItem.ItemSpec
                                                                    && x.Include == deleteItem.GetMetadata("DeletePath"));
                    
                    existingItem.Parent.RemoveChild(existingItem);

                    LogDebug("Removed {0} from project", deleteItem.GetMetadata("DeletePath"));

                }


                var slItemGroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(slItemGroup, project.Xml.LastChild);

                slItemGroup.AddItem("ImageAsset", "theappspajamas.xcassets\\Contents.json");
                slItemGroup.AddItem("ImageAsset", "theappspajamas.xcassets\\Image.imageset\\Contents.json");


                var withoutExt = Path.Combine(System.IO.Path.GetDirectoryName(ProjectFileLoad), Path.GetFileNameWithoutExtension(ProjectFileLoad));

                ProjectFileSave = String.Concat(withoutExt, Consts.ModifiedProjectNameExtra, ".csproj");

                LogDebug("Saving project to {0}", ProjectFileSave);

                project.Save(ProjectFileSave);
                //project.Build();

                //collection.UnregisterAllLoggers();

                //collection.un

            } catch (Exception ex){
                Log.LogErrorFromException(ex);

            }
            return true;
        }
    }
}
