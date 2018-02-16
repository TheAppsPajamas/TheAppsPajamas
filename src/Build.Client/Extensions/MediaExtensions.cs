using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Microsoft.Build.Framework;

namespace Build.Client.Extensions
{
    public static class MediaExtensions
    {
        public static IEnumerable<string> GetExistingMediaFiles(this BaseTask baseTask, string buildConfiguration){
            var buildConfigResourceDir = baseTask.GetBuildConfigurationResourceDir(buildConfiguration);
            try
            {
                var files = Directory.EnumerateFiles(buildConfigResourceDir, "*.png", SearchOption.AllDirectories);
                if (baseTask.Debug)
                {
                    if (files.Any())
                    {
                        baseTask.Log.LogMessage("{0} png files found in resources folder {1}", files.Count(), buildConfigResourceDir);
                        foreach (var file in files)
                        {
                            baseTask.LogDebug("Media file found {0}", file);
                        }
                    }
                    else
                    {
                        baseTask.Log.LogMessage("No files found in resources folder {0}", buildConfigResourceDir);
                    }
                }
                return files;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;

        }

        public static string GetBuildConfigurationResourceDir(this BaseTask baseTask, string buildConfiguration)
        {
            var buildResourceDir = baseTask.GetTapResourcesDir();
            baseTask.LogDebug($"{Consts.TapResourcesDir} located at {buildResourceDir}", buildResourceDir);
            baseTask.LogDebug("BuildConfiguration {0}", buildConfiguration);

            try
            {
                var mediaResourceDir = Path.Combine(buildResourceDir, buildConfiguration);
                if (!Directory.Exists(mediaResourceDir))
                {
                    baseTask.LogDebug("Created resource folder at '{0}'", mediaResourceDir);
                    Directory.CreateDirectory(mediaResourceDir);
                }
                else
                {
                    baseTask.LogDebug("Resource folder location '{0}'", mediaResourceDir);
                }
                var directoryInfo = new DirectoryInfo(mediaResourceDir);
                return directoryInfo.FullName;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static IEnumerable<ITaskItem> CombineMediaFields(this BaseTask baseTask, ITaskItem[] AppIconFields, ITaskItem[] SplashFields){
            IEnumerable<ITaskItem> allMediaFields = null;

            if (AppIconFields != null){
                if (SplashFields != null)
                {
                    allMediaFields = AppIconFields.Concat(SplashFields);
                }
                else
                {
                    allMediaFields = AppIconFields.AsEnumerable();
                }
            }
            return allMediaFields;
        }
    }
}
