using BugTrackerProject.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public interface IProjectRepository
    {
        ProjectAttributes Add(ProjectAttributes project);
        ProjectAttributes Delete(int id);
        Task<List<ProjectAttributes>> GetAllOwnedOrAssignedProjects(string userId);
        ProjectAttributes GetProject(int id);
        ProjectAttributes Update(ProjectAttributes projectChanges);
        ProjectBugs AddProjectBugs(ProjectBugs bug);
        ProjectHistory AddHistoryEntry(ProjectHistory newEntry);
        List<ProjectHistory> GetProjectHistories(int projectId);
    }
}
