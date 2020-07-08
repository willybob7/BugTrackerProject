using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BugTrackerProject.Models;
using BugTrackerProject.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using BugTrackerProject.Models.SubModels;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTrackerProject.Security;
using BugTrackerProject.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.InteropServices.WindowsRuntime;

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
        private readonly IFirebaseFileStorage firebaseFileStorage;

        public BugController(ILogger<BugController> logger,
           IBugRepository bugRepository, IProjectRepository projectRepository,
           IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager,
           IFirebaseFileStorage firebaseFileStorage)
        {
            _logger = logger;
            _bugRepository = bugRepository;
            _projectRepository = projectRepository;
            //_currentUserId = 2;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this.firebaseFileStorage = firebaseFileStorage;
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

            var currentUserId = userManager.GetUserId(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            var UserIsUserLevel = UserClaimsLevel.IsUser(claims.ToList(), projectId);

            if (UserIsUserLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = new List<IdentityUser>();
            var projectUsers = new List<string>();
            projectUsers.Add(project.OwnerId);
            if( project.UsersAssigned != null)
            {
                projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());
            }

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
                var currentUserId = userManager.GetUserId(HttpContext.User);
                var currentUser = await userManager.FindByIdAsync(currentUserId);
                var claims = await userManager.GetClaimsAsync(currentUser);

                GlobalVar.globalCurrentUserClaims = claims.ToList();

                var UserIsMangerLevel = UserClaimsLevel.IsManager(claims.ToList(), newbug.NewBugAttributes.AssociatedProject);

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
                if (GlobalVar.InitialScreenShots == true)
                {
                    uniqueFileNames = await UploadScreenShotsToStorage(bug.BugId);
                }

                GlobalVar.InitialScreenShots = false;
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

            var currentUserId = userManager.GetUserId(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = claims.ToList();


            var UserIsUserLevel = UserClaimsLevel.IsUser(claims.ToList(), bug.AssociatedProject);

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
            if (project.UsersAssigned != null)
            {
                projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());
            }

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
                Src = screenShots,
                bugHistories = bugHistory,
                ProjectUsers = users,
                CurrentUserName = HttpContext.User.Identity.Name
            };

            //if (screenShots != null)
            //{
            //    foreach (var path in screenShots)
            //    {
            //      var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", path.FilePath);

            //        using (FileStream stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
            //        {
            //            var image = Image.FromStream(stream);

            //            using (var mStream = new MemoryStream())
            //            {
            //                image.Save(mStream, ImageFormat.Jpeg);
            //                var byteData = mStream.ToArray();
            //                string imreBase64Data = Convert.ToBase64String(byteData);
            //                string imgDataURL = $"data:image/{path.FilePath.Split(".")[1]};base64,{imreBase64Data}";
            //                viewModel.Src.Add(new ScreenShots
            //                {
            //                    id = path.id,
            //                    AssociatedBug = path.AssociatedBug,
            //                    FilePath = imgDataURL
            //                });
            //            }
            //        }
            //    }
            //}
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> BugDetails(BugDetailsAndProjectNameAndId updatedBug)
        {

            var currentUserId = userManager.GetUserId(HttpContext.User);
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(currentUser);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            var UserIsMangerLevel = UserClaimsLevel.IsManager(claims.ToList(), updatedBug.Bug.AssociatedProject);

            if (UserIsMangerLevel && updatedBug.Bug.AssigneeUserId != null)
            {
                var assignedUser = await userManager.FindByIdAsync(updatedBug.Bug.AssigneeUserId);
                updatedBug.Bug.AssingeeUserName = assignedUser.UserName;
            }

            List<ScreenShots> uniqueFileNames = new List<ScreenShots>();

            if (GlobalVar.InitialScreenShots == true)
            {
                uniqueFileNames = await UploadScreenShotsToStorage(updatedBug.Bug.BugId);
            }

            GlobalVar.InitialScreenShots = false;

            var originalBug = _bugRepository.GetBug(updatedBug.Bug.BugId);

            if (updatedBug.Bug.Title == null)
            {
                updatedBug.Bug.Title = originalBug.Title;
            }

            var UserIsDeveloperLevel = UserClaimsLevel.IsDeveloper(claims.ToList(), updatedBug.Bug.AssociatedProject);

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
            if (project.UsersAssigned != null)
            {
                projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());
            }

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
                //Src = new List<ScreenShots>(),
                Src = bug.ScreenShots,
                bugHistories = bugHistory,
                ProjectUsers = users
            };
           
            return View(viewModel);
        }

        
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> DeleteBug(int bugId)
        {
            var currentUserId = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(user);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            var bug = _bugRepository.Delete(bugId);
            return RedirectToAction("ProjectBugs", "Project", new { projectId = bug.AssociatedProject });
        }


        [HttpPost]
        [Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> DeleteScreenshot(int screenShotId)
        {

            var currentUserId = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(user);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            try
            {
                _bugRepository.DeleteScreenShots(screenShotId);
                return Json(new { status = "success", message = "screenshot deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });

            }
        }

        //HttpPostedFileBase

        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> StoreInitialScreenShots(List<IFormFile> Attachments)
        {


            var currentUserId = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(user);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            var extensions = new List<string>() { ".tiff", ".pjp", ".pjpeg", ".jfif", ".tif",
            ".svg", ".bmp", ".png", ".jpeg", ".svgz", ".jpg", ".webp", ".ico", ".xbm", ".dib"};

            var maxFileSize = 3 * 1024 * 1024;

            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "temporaryFileStorage");


            try
            {

                foreach (var file in Attachments)
                {
                    var extension = Path.GetExtension(file.FileName);

                    if (extensions.Contains(extension.ToLower()) == false)
                    {
                        var filePaths = Directory.GetFiles(uploadsFolder).ToList();

                        if(filePaths.Count > 0)
                        {
                            foreach (var path in filePaths)
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        return Json(new { status = "fileNotImage", message = "Please upload an image file" });


                    }
                    else if (file.Length > maxFileSize)
                    {

                        var filePaths = Directory.GetFiles(uploadsFolder).ToList();

                        if (filePaths.Count > 0)
                        {
                            foreach (var path in filePaths)
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        return Json(new { status = "fileTooLarge", message = "Please upload a smaller file" });
                    }
                    else {
                        var uniqueFileName = "";
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(stream);
                        }
                    } 

                }

                GlobalVar.InitialScreenShots = true;

                return Json(new { status = "success", message = "files temporarily stored" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }


        public async Task<List<ScreenShots>> UploadScreenShotsToStorage(int bugId)
        {

            var currentUserId = userManager.GetUserId(HttpContext.User);
            var user = await userManager.FindByIdAsync(currentUserId);
            var claims = await userManager.GetClaimsAsync(user);

            GlobalVar.globalCurrentUserClaims = claims.ToList();

            List<ScreenShots> uniqueFileNames = new List<ScreenShots>();
            if (GlobalVar.InitialScreenShots == true)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "temporaryFileStorage");

                var filePaths = Directory.GetFiles(uploadsFolder).ToList();

                foreach (var file in filePaths)
                {

                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {

                        var fileNameSplit = stream.Name.Split("\\");
                        var fileNameSplitLength = fileNameSplit.Length;

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileNameSplit[fileNameSplitLength - 1];

                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            var downloadUrl = await firebaseFileStorage.Upload(fileBytes, uniqueFileName);
                            uniqueFileNames.Add(new ScreenShots
                            {
                                Url = downloadUrl,
                                AssociatedBug = bugId,
                                FileName = uniqueFileName
                            });

                        }

                    }
                    System.IO.File.Delete(file);
                }

            }

            return uniqueFileNames;
        }

    }
}