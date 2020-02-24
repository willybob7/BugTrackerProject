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
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTrackerProject.Security;

namespace BugTrackerProject.Controllers
{
    public class BugController : Controller
    {


        private readonly ILogger<BugController> _logger;
        private readonly IBugRepository _bugRepository;
        private readonly IProjectRepository _projectRepository;
        //private readonly int _currentUserId;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<IdentityUser> userManager;

        public BugController(ILogger<BugController> logger,
           IBugRepository bugRepository, IProjectRepository projectRepository,
           IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _bugRepository = bugRepository;
            _projectRepository = projectRepository;
            //_currentUserId = 2;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
        }






        [HttpGet]
        //[Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> AddBug(int projectId)
        {
            GlobalVar.ProjectId = projectId;

            var initialCreate = new BugAttributes()
            {
                AssociatedProject = projectId,
                DueDate = DateTime.Today,
            };

            var project = _projectRepository.GetProject(projectId);

            GlobalVar.Project = project;

            var UserIsUserLevel = UserClaimsLevel.IsUser(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsUserLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = new List<IdentityUser>();
            var projectUsers = new List<string>();
            projectUsers.Add(project.OwnerId);
            projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());

            foreach (var userId in projectUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && !users.Contains(user))
                {
                    users.Add(user);
                }
            }


            var viewModel = new AddNewBug()
            {
                NewBugAttributes = initialCreate,
                ProjectId = projectId,
                ProjectUsers = users
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> AddBug(AddNewBug newbug)
        {
            if (ModelState.IsValid)
            {

                var UserIsMangerLevel = UserClaimsLevel.IsManager(HttpContext.User.Claims.ToList(), newbug.NewBugAttributes.AssociatedProject);

                if (UserIsMangerLevel && newbug.NewBugAttributes.AssigneeUserId != null)
                {
                    var assignedUser = await userManager.FindByIdAsync(newbug.NewBugAttributes.AssigneeUserId);
                    newbug.NewBugAttributes.AssingeeUserName = assignedUser.UserName;
                }

                newbug.NewBugAttributes.ReporterID = userManager.GetUserId(HttpContext.User);
                newbug.NewBugAttributes.ReporterUserName = userManager.GetUserName(HttpContext.User);
                newbug.NewBugAttributes.EnteredDate = DateTime.Now;

                var bug = _bugRepository.Add(newbug.NewBugAttributes);

                if(bug.Title == null)
                {
                    bug.Title = $"bug{bug.BugId}";
                    bug = _bugRepository.Update(bug);
                }


                var projectBug = new ProjectBugs
                {
                    BugId = bug.BugId,
                    ProjectId = bug.AssociatedProject
                };
                _projectRepository.AddProjectBugs(projectBug);

                List<ScreenShots> uniqueFileNames = new List<ScreenShots>();
                if (newbug.ScreenShots != null && newbug.ScreenShots.Count > 0)
                {
                    foreach (var file in newbug.ScreenShots)
                    {
                        var uniqueFileName = "";
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "screenshots");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        uniqueFileNames.Add(new ScreenShots { FilePath = uniqueFileName, AssociatedBug = bug.BugId });
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
                        }

                    }
                }
                _bugRepository.AddScreenShots(uniqueFileNames);
                return RedirectToAction("BugDetails", new { bugId = bug.BugId });
            }
            return View();

        }


        [HttpGet]
        //[Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> BugDetails(int bugId)
        {


            var bug = _bugRepository.GetBug(bugId);

            GlobalVar.ProjectId = bug.AssociatedProject;

            var project = _projectRepository.GetProject(bug.AssociatedProject);

            GlobalVar.Project = project;

            var UserIsUserLevel = UserClaimsLevel.IsUser(HttpContext.User.Claims.ToList(), bug.AssociatedProject);

            if (UserIsUserLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var screenShots = _bugRepository.ScreenShots(bugId);
            var projectName = project.ProjectName;
            var comments = _bugRepository.Comments(bugId);
            var bugHistory = _bugRepository.GetBugHistories(bugId);
            bug.Comments = comments;


            var users = new List<IdentityUser>();
            var projectUsers = new List<string>();
            projectUsers.Add(project.OwnerId);
            projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());

            foreach (var userId in projectUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && !users.Contains(user))
                {
                    users.Add(user);
                }
            }

            var viewModel = new BugDetailsAndProjectNameAndId()
            {
                Bug = bug,
                ProjectName = projectName,
                ProjectId = bug.AssociatedProject,
                Updated = 0,
                Src = new List<ScreenShots>(),
                bugHistories = bugHistory,
                ProjectUsers = users
            };

            if (screenShots != null)
            {
                foreach (var path in screenShots)
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", path.FilePath);

                    using (FileStream stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                    {
                        var image = Image.FromStream(stream);

                        using (var mStream = new MemoryStream())
                        {
                            image.Save(mStream, ImageFormat.Jpeg);
                            var byteData = mStream.ToArray();
                            string imreBase64Data = Convert.ToBase64String(byteData);
                            string imgDataURL = $"data:image/{path.FilePath.Split(".")[1]};base64,{imreBase64Data}";
                            viewModel.Src.Add(new ScreenShots
                            {
                                id = path.id,
                                AssociatedBug = path.AssociatedBug,
                                FilePath = imgDataURL
                            });
                        }
                    }
                }
            }
            return View(viewModel);
        }



       


        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> BugDetails(BugDetailsAndProjectNameAndId updatedBug)
        {

            var UserIsMangerLevel = UserClaimsLevel.IsManager(HttpContext.User.Claims.ToList(), updatedBug.Bug.AssociatedProject);

            if (UserIsMangerLevel && updatedBug.Bug.AssigneeUserId != null)
            {
                var assignedUser = await userManager.FindByIdAsync(updatedBug.Bug.AssigneeUserId);
                updatedBug.Bug.AssingeeUserName = assignedUser.UserName;
            }


            List<ScreenShots> uniqueFileNames = new List<ScreenShots>();
            if (updatedBug.ScreenShots != null && updatedBug.ScreenShots.Count > 0)
            {
                foreach (var file in updatedBug.ScreenShots)
                {
                    var uniqueFileName = "";
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "screenshots");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                    uniqueFileNames.Add(new ScreenShots { FilePath = uniqueFileName, AssociatedBug = updatedBug.Bug.BugId });
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                }
            }

            var originalBug = _bugRepository.GetBug(updatedBug.Bug.BugId);

            var UserIsDeveloperLevel = UserClaimsLevel.IsDeveloper(HttpContext.User.Claims.ToList(), updatedBug.Bug.AssociatedProject);

            if (UserIsDeveloperLevel)
            {
                foreach (var property in originalBug.GetType().GetProperties())
                {
                    if (property.Name == "AssigneeUserId")
                    {
                        continue;
                    }
                    var oldValue = "";
                    var newValue = "";

                    if (property.GetValue(updatedBug.Bug) != null)
                    {
                        newValue = property.GetValue(updatedBug.Bug).ToString();
                    }

                    if (property.GetValue(originalBug) != null)
                    {
                        oldValue = property.GetValue(originalBug).ToString();
                    }

                    if (oldValue != newValue)
                    {
                        var changes = new BugHistory
                        {
                            AssociatedBugId = originalBug.BugId,
                            Property = property.Name,
                            OldValue = oldValue,
                            NewValue = newValue,
                            DateChanged = DateTime.Now
                        };
                        _bugRepository.AddHistoryEntry(changes);
                    }

                }
            }
            

          
            var bug = new BugAttributes();
            if (UserIsDeveloperLevel)
            {
                updatedBug.Bug.ScreenShots = uniqueFileNames;
                updatedBug.Bug.ScreenShots.AddRange(_bugRepository.ScreenShots(updatedBug.Bug.BugId));
                bug = _bugRepository.Update(updatedBug.Bug);
            }
            else
            {

                //bug = _bugRepository.GetBug(updatedBug.Bug.BugId);
                bug = originalBug;
                bug.ScreenShots = uniqueFileNames;
                bug.ScreenShots.AddRange(_bugRepository.ScreenShots(updatedBug.Bug.BugId));
            }
            var project = _projectRepository.GetProject(bug.AssociatedProject);
            var projectName = project.ProjectName;
            bug.Comments = _bugRepository.Comments(bug.BugId);
            var bugHistory = _bugRepository.GetBugHistories(bug.BugId);

            var users = new List<IdentityUser>();
            var projectUsers = new List<string>();
            projectUsers.Add(project.OwnerId);
            projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());

            foreach (var userId in projectUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && !users.Contains(user))
                {
                    users.Add(user);
                }
            }



            var viewModel = new BugDetailsAndProjectNameAndId()
            {
                Bug = bug,
                ProjectName = projectName,
                ProjectId = bug.AssociatedProject,
                Updated = 1,
                Src = new List<ScreenShots>(),
                bugHistories = bugHistory,
                ProjectUsers = users
            };



            //if (updatedBug.Bug.ScreenShots != null)
            if (bug.ScreenShots != null)
            {
                foreach (var path in bug.ScreenShots)
                {
                    var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", path.FilePath);

                    using (FileStream stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
                    {
                        var image = Image.FromStream(stream);

                        using (var mStream = new MemoryStream())
                        {
                            image.Save(mStream, ImageFormat.Jpeg);
                            var byteData = mStream.ToArray();
                            string imreBase64Data = Convert.ToBase64String(byteData);
                            string imgDataURL = $"data:image/{path.FilePath.Split(".")[1]};base64,{imreBase64Data}";
                            viewModel.Src.Add(new ScreenShots
                            {
                                id = path.id,
                                AssociatedBug = path.AssociatedBug,
                                FilePath = imgDataURL
                            });
                        }
                    }
                }
            }
            return View(viewModel);
        }

        
        [Authorize(Policy = "ManagerPolicy")]
        public IActionResult DeleteBug(int bugId)
        {
            var bug = _bugRepository.Delete(bugId);
            return RedirectToAction("ProjectBugs", "Project", new { projectId = bug.AssociatedProject });
        }






    }
}