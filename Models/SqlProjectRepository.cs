using BugTrackerProject.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BugTrackerProject.Models
{
    public class SqlProjectRepository : IProjectRepository
    {
        private readonly AppDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public SqlProjectRepository(AppDbContext context, RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public ProjectAttributes Add(ProjectAttributes project)
        {
            context.Projects.Add(project);
            context.SaveChanges();
            return project;
        }

        public ProjectBugs AddProjectBugs(ProjectBugs bug)
        {
            context.ProjectBugs.Add(bug);
            context.SaveChanges();
            return bug;
        }


        public ProjectAttributes Delete(int id)
        {
            var project = context.Projects.Find(id);
            var projectBugs = context.ProjectBugs.Where(b => b.ProjectId == id).ToList();

            foreach (var bug in projectBugs)
            {
                if (bug != null)
                {

                    var associatedScreenshots = context.ScreenShots.Where(s => bug.BugId == s.AssociatedBug);
                    foreach (var file in associatedScreenshots)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", file.FilePath);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        context.ScreenShots.Remove(file);
                    }
                    var bugToDelete = context.Bugs.Find(bug.BugId);
                    context.Bugs.Remove(bugToDelete);
                    context.ProjectBugs.Remove(bug);
                }
            }



            project.ProjectBugs = projectBugs;
            if (project != null)
            {
                var projectHistory = context.ProjectHistory.Where(p => p.AssociatedProjectId == id);
                context.RemoveRange(projectHistory);
                context.Projects.Remove(project);
                context.SaveChanges();
            }
            
            return project;
        }

        public async Task<List<ProjectAttributes>>  GetAllOwnedOrAssignedProjects(string userId)
        {

            var user = await userManager.FindByIdAsync(userId);
            var existingUserClaims = await userManager.GetClaimsAsync(user);


            var usersProjectsList = new List<ProjectAttributes>();
            var usersProjectIdList = new List<string>();

            foreach (var claim in existingUserClaims)
            {
                var projectList = claim.Value.Split(" ").ToList();
                foreach (var id in projectList)
                {
                    if(id == "")
                    {
                        continue;
                    }
                    
                    if (usersProjectIdList.Contains(id) == false)
                    {
                        var idInt = Convert.ToInt32(id);
                        usersProjectIdList.Add(id);
                        var project = context.Projects.AsNoTracking().FirstOrDefault(p => p.ProjectId == idInt);
                        if (project != null)
                        {
                            //something wrong here 
                            //var OwnerId = project.OwnerId;
                            //var Owner = await userManager.FindByIdAsync(OwnerId);
                            //project.OwnerId = Owner.UserName;

                            usersProjectsList.Add(project);
                        }
                       
                    }
                }
            }


            return usersProjectsList;
        }

        public ProjectAttributes GetProject(int id)
        {
            return context.Projects.AsNoTracking().First(p => p.ProjectId == id);
            //return projects.Find(id);
        }

        public ProjectAttributes Update(ProjectAttributes projectChanges)
        {
            var project = context.Projects.Attach(projectChanges);
            project.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return projectChanges;
        }

        public ProjectHistory AddHistoryEntry(ProjectHistory newEntry)
        {
            context.ProjectHistory.Add(newEntry);
            context.SaveChanges();
            return newEntry;
        }

        public List<ProjectHistory> GetProjectHistories(int projectId)
        {
            return context.ProjectHistory.Where(p => p.AssociatedProjectId == projectId).ToList();
        }


    }
}
