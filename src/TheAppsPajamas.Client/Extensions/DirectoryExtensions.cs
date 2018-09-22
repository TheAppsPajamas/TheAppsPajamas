using System;
using System.IO;
using TheAppsPajamas.Client.Tasks;

namespace TheAppsPajamas.Client.Extensions
{
    public static class DirectoryExtensions
    {
        public static string FindTopFileInProjectDir(this BaseTask baseTask, string fileToFind){
            string filePath = String.Empty;
            try
            {

                baseTask.LogVerbose($"Reading DirectoryInfo for folder {baseTask.ProjectDir}");
                var currentDirectoryInfo = new DirectoryInfo(baseTask.ProjectDir);
                if (currentDirectoryInfo == null){
                    baseTask.LogVerbose($"DirectoryInfo for {baseTask.ProjectDir} is null");
                }
                do
                {
                    var fileExistenceToTest = Path.Combine(currentDirectoryInfo.FullName, fileToFind);
                    baseTask.LogVerbose($"Testing file existance at path {fileExistenceToTest}");
                    if (File.Exists(fileExistenceToTest))
                    {
                        baseTask.LogVerbose($"File exists at path {fileExistenceToTest}");
                        filePath = fileExistenceToTest;
                    }
                    if (currentDirectoryInfo.Parent == null){
                        break;
                    }
                    currentDirectoryInfo = currentDirectoryInfo.Parent;
                } while (currentDirectoryInfo != null);

                if (!String.IsNullOrEmpty(filePath))
                {
                    baseTask.LogInformation($"Found {fileToFind} at path {filePath}");
                }
                else
                {
                    baseTask.LogDebug($"Did not find file {fileToFind} in path {baseTask.ProjectDir}");
                }
            } catch (Exception e){
                baseTask.Log.LogError($"Exception reading files {e.Message}");
            }
            return filePath;
        }
    }
}
