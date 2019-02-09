using System;
using System.IO;
using TheAppsPajamas.Tasks;

namespace TheAppsPajamas.Extensions
{
    public static class DirectoryExtensions
    {
        public static string FindTopFileInProjectDir(this BaseTask baseTask, string fileToFind){
            string filePath = String.Empty;
            try
            {

                baseTask.LogVerbose($"Searching for config file {fileToFind}");
                var currentDirectoryInfo = new DirectoryInfo(baseTask.ProjectDir);
                if (currentDirectoryInfo == null){
                    baseTask.Log.LogError($"Directory {baseTask.ProjectDir} is null");
                }
                do
                {
                    var fileExistenceToTest = Path.Combine(currentDirectoryInfo.FullName, fileToFind);
                    //baseTask.LogVerbose($"Testing for file {fileExistenceToTest}");
                    if (File.Exists(fileExistenceToTest))
                    {
                        //baseTask.LogVerbose($"File exists at {fileExistenceToTest}");
                        filePath = fileExistenceToTest;
                    }
                    if (currentDirectoryInfo.Parent == null){
                        break;
                    }
                    currentDirectoryInfo = currentDirectoryInfo.Parent;
                } while (currentDirectoryInfo != null);

                if (!String.IsNullOrEmpty(filePath))
                {
                    baseTask.LogInformation($"Selecting {fileToFind} from {filePath}");
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
