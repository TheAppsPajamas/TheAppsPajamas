using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheAppsPajamas.Client.Tasks;
using TheAppsPajamas.Client.Constants;
using Microsoft.Build.Framework;

namespace TheAppsPajamas.Client.Extensions
{
    public static class MediaExtensions
    {
        public static IEnumerable<string> GetExistingMediaFiles(this BaseTask baseTask, string buildConfiguration){
            var buildConfigAssetDir = baseTask.GetBuildConfigurationAssetDir(buildConfiguration);
            try
            {
                var files = Directory.EnumerateFiles(buildConfigAssetDir, "*.png", SearchOption.AllDirectories);
                if (baseTask.IsVerbose())
                {
                    if (files.Any())
                    {
                        baseTask.LogVerbose("{0} png files found in resources folder {1}", files.Count(), buildConfigAssetDir);
                        foreach (var file in files)
                        {
                            baseTask.LogDebug("Media file found {0}", file);
                        }
                    }
                    else
                    {
                        baseTask.LogVerbose("No files found in resources folder {0}", buildConfigAssetDir);
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

        public static string GetBuildConfigurationAssetDir(this BaseTask baseTask, string buildConfiguration)
        {
            var tapAssetDir = baseTask.GetAssetDir();
            baseTask.LogDebug($"{Consts.TapAssetsDir} located at {tapAssetDir}", tapAssetDir);
            baseTask.LogDebug("BuildConfiguration {0}", buildConfiguration);

            try
            {
                var mediaAssetDir = Path.Combine(tapAssetDir, buildConfiguration);
                if (!Directory.Exists(mediaAssetDir))
                {
                    baseTask.LogDebug("Created asset folder at '{0}'", mediaAssetDir);
                    Directory.CreateDirectory(mediaAssetDir);
                }
                else
                {
                    baseTask.LogDebug("Asset folder location '{0}'", mediaAssetDir);
                }
                var directoryInfo = new DirectoryInfo(mediaAssetDir);
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
