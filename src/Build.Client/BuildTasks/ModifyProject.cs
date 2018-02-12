using System;
using System.Collections.Generic;
using System.IO;
using Build.Client.Constants;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace Build.Client.BuildTasks
{
    public class ModifyProject : BaseTask
    {
        public string ProjectFileLoad { get; set; }

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

                var slItemGroup = project.Xml.CreateItemGroupElement();
                project.Xml.InsertAfterChild(slItemGroup, project.Xml.LastChild);

                slItemGroup.AddItem("ImageAsset", "TheAppsPajamas.xcassets\\Contents.json");
                slItemGroup.AddItem("ImageAsset", "TheAppsPajamas.xcassets\\Image.imageset\\Contents.json");


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
