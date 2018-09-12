using System;
using System.Collections.Generic;
using System.Linq;
using TheAppsPajamas.Client.Constants;
using TheAppsPajamas.Client.Extensions;
using TheAppsPajamas.Client.Models;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TheAppsPajamas.Client.Tasks
{
    public abstract class BaseLoadTask : BaseTask
    {
        public string ProjectName { get; set; }
        public string BuildConfiguration { get; set; }
        public string TargetFrameworkIdentifier { get; set; }

        public string _appId;

        [Output]
        public string TapAssetDir { get; set; }

        [Output]
        public ITaskItem[] PackagingFieldOutput { get; set; }

        [Output]
        public ITaskItem[] AppIconFieldOutput { get; set; }

        [Output]
        public ITaskItem[] SplashFieldOutput { get; set; }

        [Output]
        public ITaskItem BuildConfigHolderOutput { get; set; }

        [Output]
        public ITaskItem PackagingHolderOutput { get; set; }

        [Output]
        public ITaskItem AppIconHolderOutput { get; set; }

        [Output]
        public ITaskItem SplashHolderOutput { get; set; }


        [Output]
        public ITaskItem AssetCatalogueName { get; set; }


        [Output]
        public ITaskItem TapAssetDirRelative { get; set; }

        [Output]
        public string TapShouldContinue { get; set; }


        protected string _taskName;
        protected TapSetting _tapSetting;

        public override bool Execute()
        {
            var baseResult = base.Execute();
            LogInformation($"Running {_taskName}");

            LogDebug($"Project name {ProjectName}");
            LogDebug($"Build configuration {BuildConfiguration}");

            TapAssetDirRelative = new TaskItem(Consts.TapAssetsDir);
            TapShouldContinue = bool.TrueString;

            _tapSetting = this.GetTapSetting();

            //TODO check for security here as well, so both get created
            if (_tapSetting == null)
            {
                //Change to warning, and return TapShouldContinue = false
                Log.LogError($"{Consts.TapSettingFile} file not set, please see solution root and complete");
                return false;
            }


            if (_tapSetting.BuildConfigs == null)
            {
                LogDebug("Added BuildConfigs list");
                _tapSetting.BuildConfigs = new List<BuildConfig>();
            }

            var thisBuildConfig = _tapSetting.BuildConfigs.FirstOrDefault(x => x.BuildConfiguration == BuildConfiguration
                                                                                   && x.ProjectName == ProjectName);

            if (thisBuildConfig == null)
            {
                LogInformation($"Project {ProjectName} Build configuration {BuildConfiguration} not found, so adding to {Consts.TapSettingFile}");
                _tapSetting.BuildConfigs.Add(new BuildConfig(ProjectName, BuildConfiguration));
                this.SaveTapAssetConfig(_tapSetting);
            }
            else if (thisBuildConfig.Disabled == true)
            {
                LogInformation($"The Apps Pajamas is disabled in {Consts.TapSettingFile} for Project {ProjectName} in configuration {BuildConfiguration}], exiting");
                TapShouldContinue = bool.FalseString;
                return true;
            }




            return true;
        }
    }
}
