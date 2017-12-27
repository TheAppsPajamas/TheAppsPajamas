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
        public static ProjectsConfig GetProjectsConfig(this BaseLoadTask baseTask)
        {
            baseTask.LogDebug("Loading projects.config file");

            try
            {
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectsConfig);
                ProjectsConfig projectsConfig = null;
                if (!File.Exists(projectsConfigPath))
                {
                    baseTask.LogDebug("Creating blank projects config at {0}", projectsConfigPath);
                    projectsConfig = new ProjectsConfig();
                    var json = JsonConvert.SerializeObject(projectsConfig);
                    File.WriteAllText(projectsConfigPath, json);
                    baseTask.Log.LogError("Projects config file not found, created at {0}", projectsConfigPath);
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
                var projectsConfigPath = Path.Combine(baseTask.BuildResourceDir, Consts.ProjectsConfig);
 
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
    }
}
