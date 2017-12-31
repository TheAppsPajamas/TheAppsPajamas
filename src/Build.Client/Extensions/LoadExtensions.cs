using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Build.Client.BuildTasks;
using Build.Client.Constants;
using Build.Client.Models;
using Newtonsoft.Json;

namespace Build.Client.Extensions
{
    public static class LoadExtensions
    {
        public static string GetBuildResourceDir(this BaseTask baseTask)
        {
            baseTask.LogDebug("ProjectDir located at '{0}'", baseTask.ProjectDir);

            try
            {
                var buildResourceDir = Path.Combine(baseTask.ProjectDir, Consts.BuildResourcesDir);
                if (!Directory.Exists(buildResourceDir))
                {
                    baseTask.LogDebug("Created Build-Resources folder at '{0}'", buildResourceDir);
                    Directory.CreateDirectory(buildResourceDir);
                }
                else
                {
                    baseTask.LogDebug("Build-Resources folder location '{0}'", buildResourceDir);
                }
                var directoryInfo = new DirectoryInfo(buildResourceDir);
                return directoryInfo.FullName;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static string GetBuildResourceDirOld(this BaseTask baseTask)
        {
            baseTask.LogDebug("PackagesDir located at '{0}'", baseTask.PackagesDir);

            try
            {
                var buildResourceDir = Path.Combine(baseTask.PackagesDir, Consts.BuildResourcesDir);
                if (!Directory.Exists(buildResourceDir))
                {
                    baseTask.LogDebug("Created Build-Resources folder at '{0}'", buildResourceDir);
                    Directory.CreateDirectory(buildResourceDir);
                }
                else
                {
                    baseTask.LogDebug("Build-Resources folder location '{0}'", buildResourceDir);
                }
                var directoryInfo = new DirectoryInfo(buildResourceDir);
                return directoryInfo.FullName;
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        //TODO to go - being replaced with individual project config
        public static ProjectsConfig GetProjectsConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading projects.config file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);
                ProjectsConfig projectsConfig = null;
                if (!File.Exists(projectsConfigPath))
                {
                    baseTask.LogDebug("Creating blank projects config at {0}", projectsConfigPath);
                    projectsConfig = new ProjectsConfig();
                    var json = JsonConvert.SerializeObject(projectsConfig);
                    File.WriteAllText(projectsConfigPath, json);
                    baseTask.Log.LogMessage("Projects config file not found, created at {0}", projectsConfigPath);
                    return projectsConfig;
                }
                else
                {
                    var json = File.ReadAllText(projectsConfigPath);
                    projectsConfig = JsonConvert.DeserializeObject<ProjectsConfig>(json);
                    return projectsConfig;
                    
                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ProjectConfig GetProjectConfigV2(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading project.config file");

            try
            {
                var projectConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);
                ProjectConfig projectConfig = null;
                if (!File.Exists(projectConfigPath))
                {
                    baseTask.LogDebug("Creating blank project.config at {0}", projectConfigPath);
                    projectConfig = new ProjectConfig();
                    var json = JsonConvert.SerializeObject(projectConfig);
                    File.WriteAllText(projectConfigPath, json);
                    baseTask.Log.LogMessage("Project config file not found, created at {0}", projectConfigPath);
                    return projectConfig;
                }
                else
                {
                    var json = File.ReadAllText(projectConfigPath);
                    projectConfig = JsonConvert.DeserializeObject<ProjectConfig>(json);
                    return projectConfig;

                }
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return null;
        }

        public static ProjectConfig GetProjectConfig(this BaseLoadTask baseTask, ProjectsConfig projectsConfig){
            ProjectConfig thisProject = null;
            if (projectsConfig.Projects != null)
            {
                thisProject = projectsConfig.Projects
                                    .FirstOrDefault(x => x.ProjectName == baseTask.ProjectName
                                                    && x.BuildConfiguration == baseTask.BuildConfiguration);
            } 

            if (thisProject == null)
            {
                baseTask.LogDebug("Project {0}, Build Configuration {1}, record not found, creating"
                                  , baseTask.ProjectName, baseTask.BuildConfiguration);
                thisProject = new ProjectConfig
                {
                    ProjectName = baseTask.ProjectName,
                    BuildConfiguration = baseTask.BuildConfiguration
                };
                projectsConfig.Projects.Add(thisProject);
            } else {
                baseTask.LogDebug("Project {0}, Build Configuration {1}, record exists"
                                  , baseTask.ProjectName, baseTask.BuildConfiguration);
                
            }

            return thisProject;
        }

        public static bool SaveProjects(this BaseLoadTask baseTask, ProjectsConfig projectsConfig)
        {
            baseTask.LogDebug("Saving projects.config file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);
 
                    baseTask.LogDebug("Saving projects config at {0}", projectsConfigPath);
                    var json = JsonConvert.SerializeObject(projectsConfig);
                File.WriteAllText(projectsConfigPath, json);
                baseTask.LogDebug("Saved projects config at {0}", projectsConfigPath);
       
            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return true;
        }

        public static bool SaveProject(this BaseLoadTask baseTask, ProjectConfig projectConfig)
        {
            baseTask.LogDebug("Saving project.config file");

            try
            {
                var projectConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectConfig);

                baseTask.LogDebug("Saving project config at {0}", projectConfigPath);
                var json = JsonConvert.SerializeObject(projectConfig);
                File.WriteAllText(projectConfigPath, json);
                baseTask.LogDebug("Saved project config at {0}", projectConfigPath);

            }
            catch (Exception ex)
            {
                baseTask.Log.LogErrorFromException(ex);
            }
            return true;
        }
    }
}
