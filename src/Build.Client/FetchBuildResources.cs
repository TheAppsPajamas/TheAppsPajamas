using System;
using Microsoft.Build.Utilities;

namespace Build.Client
{
    public class FetchBuildResources : Task
    {
        public FetchBuildResources()
        {
        }

        public override bool Execute()
        {
            //so this needs to do a lot of things

            //first job is send to api the current project name
            //and get the dump down

            //then we roll through each of those
            //and excecute
            //like
            //packaging
            //if type is core
            //androidpackagingservice.execute(packagingdataset)
            //if type is android
            //we could set these all up with a resolver first
            //goes interface IPackagingService = resovlewith droid version

            //gets the ugly if's out of the way of the main processing block


            //variables required MSBuildProjectName
            //buildconfiguration

            var t = BuildEngine;
            var p = TaskResources;

            throw new NotImplementedException();
        }
    }
}
