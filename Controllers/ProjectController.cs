using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using BugTrackerProject.Models;
using BugTrackerProject.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using BugTrackerProject.Models.SubModels;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BugTrackerProject.Security;

namespace BugTrackerProject.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IBugRepository _bugRepository;
        private readonly UserManager<IdentityUser> userManager;
        //private readonly int _currentUserId;
        private readonly IProjectRepository _projectRepository;

        public ProjectController(ILogger<ProjectController> logger,
           IProjectRepository projectRepository, IBugRepository bugRepository,
           UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _bugRepository = bugRepository;
            this.userManager = userManager;
            _projectRepository = projectRepository;
            //_currentUserId = 2;
        }

        [HttpGet]
        public IActionResult AddProject(string userId)
        {
            var viewModel = new AddProjectViewModel
            {
                UserId = userId
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectAttributes project)
        {
            if (ModelState.IsValid)
            {
                var newProject = new ProjectAttributes();
                var userId = userManager.GetUserId(HttpContext.User);
                var user = await userManager.FindByIdAsync(userId);

                project.OwnerId = userId;
                //project.UsersAssigned = "";
                project.OwnerUserName = user.UserName;
                newProject = _projectRepository.Add(project);
                var viewModel = new ProjectDetailsAndAssociatedBugs()
                {
                    Project = newProject,
                    ProjectBugs = _bugRepository.GetAllProjectBugs(project.ProjectId),
                    ProjectId = newProject.ProjectId
                };

                var claims = await userManager.GetClaimsAsync(user);
                var result = await userManager.RemoveClaimsAsync(user, claims);


                var claimList = new List<Claim>();

                var currentClaims = claims.ToList();
                for (var i = 0; i < ClaimsStore.AllClaims.Count; i++)
                {
                    if (claims.Count == 4 && claims[i].Value != null)
                    {
                        claimList.Add(new Claim(ClaimsStore.AllClaims[i].Type, claims[i].Value + " " + newProject.ProjectId.ToString()));

                    }
                    else
                    {
                        claimList.Add(new Claim(ClaimsStore.AllClaims[i].Type, newProject.ProjectId.ToString()));
                    }

                }


                result = await userManager.AddClaimsAsync(user, claimList);

                //only if needed I guess
                //copied from administrationController
                //if (result.Succeeded == false)
                //{
                //    ModelState.AddModelError("", "Cannot add selected claims to user");
                //    return View(model);
                //}


                return View("ProjectDetails", viewModel);
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult SetGlobalVar(int projectId)
        {
            GlobalVar.ProjectId = projectId;
            return RedirectToAction("ProjectDetails", new { projectId = projectId });

        }




        [HttpGet]
        //[Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> ProjectDetails(int projectId)
        {
            GlobalVar.ProjectId = projectId;


            var project = _projectRepository.GetProject(GlobalVar.ProjectId);

            GlobalVar.Project = project;

            var UserIsUserLevel = UserClaimsLevel.IsUser(HttpContext.User.Claims.ToList(), projectId);

            if(UserIsUserLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            //var projectOwner = await userManager.FindByIdAsync(project.OwnerId);
            //project.OwnerId = projectOwner.UserName;

            var projectBugs = _bugRepository.GetAllProjectBugs(projectId);
            var projectHistory = _projectRepository.GetProjectHistories(projectId);
            var viewModel = new ProjectDetailsAndAssociatedBugs()
            {
                Project = project,
                ProjectBugs = projectBugs,
                projectHistories = projectHistory,
                ProjectId = project.ProjectId
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy  = "ManagerPolicy")]
        public IActionResult ProjectDetails(ProjectDetailsAndAssociatedBugs projectUpdates)
        {


            var originalProject = _projectRepository.GetProject(projectUpdates.Project.ProjectId);



            foreach (var property in originalProject.GetType().GetProperties())
            {
                var oldValue = "";
                var newValue = "";

                if(property.GetValue(projectUpdates.Project) != null)
                {
                    newValue = property.GetValue(projectUpdates.Project).ToString();
                }

                if (property.GetValue(originalProject) != null)
                {
                     oldValue = property.GetValue(originalProject).ToString();
                }
               
                if (oldValue != newValue)
                {
                    var changes = new ProjectHistory
                    {
                        AssociatedProjectId = originalProject.ProjectId,
                        Property = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                        DateChanged = DateTime.Now
                    };
                    _projectRepository.AddHistoryEntry(changes);
                }

            }


            var projectBugs = _bugRepository.GetAllProjectBugs(projectUpdates.Project.ProjectId);
            var project = _projectRepository.Update(projectUpdates.Project);
            var projectHistory = _projectRepository.GetProjectHistories(projectUpdates.Project.ProjectId);

            var viewModel = new ProjectDetailsAndAssociatedBugs()
            {
                Project = project,
                ProjectBugs = projectBugs,
                projectHistories = projectHistory,
                ProjectId = GlobalVar.ProjectId
            };
            return View(viewModel);
        }

        //[Authorize(Policy = "UserPolicy")]
        public IActionResult ProjectBugs(int projectId)
        {

            GlobalVar.ProjectId = projectId;

            var project = _projectRepository.GetProject(projectId);

            GlobalVar.Project = project;

            var UserIsUserLevel = UserClaimsLevel.IsUser(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsUserLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var projectBugs = _bugRepository.GetAllProjectBugs(projectId);
            var viewModel = new ProjectDetailsAndAssociatedBugs()
            {
                Project = project,
                ProjectBugs = projectBugs,
                ProjectId = projectId
            };
            return View(viewModel);
        }



        [Authorize(Policy = "AdminPolicy")]

        public IActionResult DeleteProject(int projectId)
        {
            var project = _projectRepository.Delete(projectId);
            return RedirectToAction("Index");
        }




    }
}