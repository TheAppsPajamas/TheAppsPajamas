using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Client.BuildTasks
{
    public abstract class BaseLoadTask : BaseTask
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public string TargetFrameworkIdentifier { get; set; }

        public string _appId;

        [Output]
        public string BuildResourceDir { get; set; }

        [Output]
        public ITaskItem[] PackagingOutput { get; set; }

        [Output]
        public ITaskItem[] AppIconOutput { get; set; }

        [Output]
        public ITaskItem AssetCatalogueName { get; set; }

        [Output]
        public ITaskItem AppIconCatalogueName { get; set; }

        [Output]
        public ITaskItem[] SplashOutput { get; set; }

        [Output]
        public ITaskItem TheAppsPajamasResourceDir { get; set; }

    }
}
