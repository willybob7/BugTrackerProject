using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Authorization;

namespace BugTrackerProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBugRepository _bugRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<IdentityUser> userManager;


        public HomeController(ILogger<HomeController> logger, 
            IBugRepository bugRepository, IProjectRepository projectRepository,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _bugRepository = bugRepository;
            _projectRepository = projectRepository;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(userId);
            var currentUserClaims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = currentUserClaims.ToList();


            var viewModel = new HomeViewModel()
            {
                MyBugs = _bugRepository.GetAllAssigneeBugs(userId),
                MyProjects = _projectRepository.GetAllOwnedOrAssignedProjects(userId).Result,
                UserId = userId 

            };
            return View(viewModel);
        }

        public async Task<IActionResult> MyBugs()
        {


            var userId = userManager.GetUserId(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(userId);
            var currentUserClaims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = currentUserClaims.ToList();

            var myBugs = _bugRepository.GetAllAssigneeBugs(userId);
            var associatedProjects = new List<ProjectAttributes>();
            foreach(var bug in myBugs)
            {
                associatedProjects.Add(_projectRepository.GetProject(bug.AssociatedProject));
            }
            var viewModel = new MyBugsAndAssociatedProjects()
            {
                MyBugs = myBugs,
                AssociatedProjects = associatedProjects
            };
            return View(viewModel);
        }

        
        public async Task<IActionResult> MyProjects()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var userName = userManager.GetUserName(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(userId);
            var currentUserClaims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = currentUserClaims.ToList();

            var projectList = _projectRepository.GetAllOwnedOrAssignedProjects(userId).Result;
            foreach(var project in projectList)
            {
                var projectBugList = new List<ProjectBugs>();
                var projectBugs = _bugRepository.GetAllProjectBugs(project.ProjectId);
                foreach(var bug in projectBugs)
                {
                    var bugEntry = new ProjectBugs {
                        BugId = bug.BugId,
                        ProjectId = bug.AssociatedProject
                    };
                    projectBugList.Add(bugEntry);
                }
                project.ProjectBugs = projectBugList;
            }
            return View(projectList);
        }
       



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
