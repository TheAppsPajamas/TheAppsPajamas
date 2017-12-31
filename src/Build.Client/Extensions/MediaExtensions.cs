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
            var mediaResourceDir = baseTask.GetMediaResourceDir(buildConfiguration);
            try
            {
                var files = Directory.EnumerateFiles(mediaResourceDir, "*.png", SearchOption.AllDirectories);
                if (baseTask.Debug)
                {
                    if (files.Any())
                    {
                        baseTask.Log.LogMessage("{0} png files found in media resources folder", files.Count());
                        foreach (var file in files)
                        {
                            baseTask.LogDebug("Media file found {0}", file);
                        }
                    }
                    else
                    {
                        baseTask.Log.LogMessage("No files found in media resources folder");
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

        public static string GetMediaResourceDir(this BaseTask baseTask, string buildConfiguration)
        {
            var buildResourceDir = baseTask.GetBuildResourceDir();
            baseTask.LogDebug("BuildResourceDir located at '{0}'", buildResourceDir);
            baseTask.LogDebug("BuildConfiguration '{0}'", buildConfiguration);

            try
            {
                var mediaResourceDir = Path.Combine(buildResourceDir, Consts.MediaResourcesDir, buildConfiguration);
                if (!Directory.Exists(mediaResourceDir))
                {
                    baseTask.LogDebug("Created media-resources folder at '{0}'", mediaResourceDir);
                    Directory.CreateDirectory(mediaResourceDir);
                }
                else
                {
                    baseTask.LogDebug("media-resources folder location '{0}'", mediaResourceDir);
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
