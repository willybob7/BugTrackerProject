using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTrackerProject.Models;
using BugTrackerProject.ViewModels;
using BugTrackerProject.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BugTrackerProject.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<AdministrationController> logger;
        private readonly IProjectRepository projectRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager, ILogger<AdministrationController> logger, 
            IProjectRepository projectRepository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this.projectRepository = projectRepository;
        }


        [HttpGet]
        //[Authorize(Policy = "DeveloperPolicy")]
        public async Task<IActionResult> ListUsers(int projectId)
        {
            GlobalVar.ProjectId = projectId;

            var users = new List<IdentityUser>();

            var project = projectRepository.GetProject(projectId);

            GlobalVar.Project = project;

            var UserIsDeveloperLevel = UserClaimsLevel.IsDeveloper(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsDeveloperLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var projectUsers = new List<string>();
            projectUsers.Add(project.OwnerId);
            projectUsers.AddRange(project.UsersAssigned.Split(" ").ToList());

            foreach(var userId in projectUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null && !users.Contains(user))
                {
                    users.Add(user);
                }
            }



            var viewModel = new ListUsersViewModel
            {
                Users = users,
                ProjectId = projectId,
                OwnerId = project.OwnerId
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerPolicy")]

        public async Task<IActionResult> RemoveUserFromProject(string userId, int projectId)
        {


            var project = projectRepository.GetProject(projectId);

            var usersAssignedToProject = project.UsersAssigned.Split(" ").ToList();

            foreach (var user in usersAssignedToProject)
            {
                if (user == userManager.GetUserId(HttpContext.User))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }


                if (user == userId)
                {
                    usersAssignedToProject.Remove(user);
                    break;
                }
            }

            project.UsersAssigned = String.Join(" ", usersAssignedToProject);
            project = projectRepository.Update(project);

            var identityUser = await userManager.FindByIdAsync(userId);

            if (identityUser != null)
            {
                var userClaims = await userManager.GetClaimsAsync(identityUser);

                var result = await userManager.RemoveClaimsAsync(identityUser, userClaims);

                var userClaimsList = new List<Claim>();
                var projectIdString = projectId.ToString();


                foreach (var claim in userClaims)
                {
                    var added = false;
                    if(claim.Value != null )
                    {
                        var usersProjects = claim.Value.Split(" ").ToList();
                        
                        foreach(var userProject in usersProjects)
                        {
                            if (userProject == projectIdString)
                            {
                                usersProjects.Remove(userProject);
                                userClaimsList.Add(new Claim(claim.Type, String.Join(" ", usersProjects)));
                                added = true;
                                break;
                            }
                        }
                        if(added == false)
                        {
                            userClaimsList.Add(new Claim(claim.Type, claim.Value));
                        }
                    } else
                    {
                        userClaimsList.Add(new Claim(claim.Type, null));
                    }

                };

                result = await userManager.AddClaimsAsync(identityUser, userClaimsList);

            }

            return Json(new
            {
                status = "successfully removed user from project",
                userDiv = "userDiv_" + userId

            });

           
        }



        [HttpGet]
        //[Authorize(Policy = "DeveloperPolicy")]
        public IActionResult AddUserToProject(int projectId)
        {

            GlobalVar.ProjectId = projectId;


            var project = projectRepository.GetProject(GlobalVar.ProjectId);

            GlobalVar.Project = project;

            var UserIsDeveloperLevel = UserClaimsLevel.IsDeveloper(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsDeveloperLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            var viewModel = new AddUserToProjectViewModel
            {
                ProjectId = projectId
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = "DeveloperPolicy")]
        public async Task<IActionResult> AddUserToProject(int projectId, string users)
        {

            var userList = users.Split(" ").ToList();

            var project = projectRepository.GetProject(projectId);

            foreach (var userId in userList)
            {

                if(project.UsersAssigned == null)
                {
                    project.UsersAssigned = userId;
                    project = projectRepository.Update(project);
                } else
                {
                    project.UsersAssigned += " " + userId;
                    project = projectRepository.Update(project);
                }

                var identityUser = await userManager.FindByIdAsync(userId);

                if(identityUser != null)
                {
                    var userClaims = await userManager.GetClaimsAsync(identityUser);

                    if (userClaims.Count != 4)//this means that the person doesn't have any claims
                    {
                        var userClaimsList = new List<Claim>();
                        foreach (var claim in ClaimsStore.AllClaims)
                        {
                            if (claim.Type == "User Role")
                            {
                                userClaimsList.Add(new Claim(claim.Type, projectId.ToString()));
                            }
                            else
                            {
                                userClaimsList.Add(new Claim(claim.Type, String.Empty));
                            }
                        };

                        var result = await userManager.AddClaimsAsync(identityUser, userClaimsList);
                       
                    }
                    else if (userClaims.Count == 4) //this means that they have claims and have been added to a project
                    { 
                        var result = await userManager.RemoveClaimsAsync(identityUser, userClaims);

                        var claimList = new List<Claim>();
                        var projectIdString = projectId.ToString();

                        foreach (var claim in userClaims)
                        {
                            if(claim.Type == "User Role")
                            {
                                claimList.Add(new Claim(claim.Type, claim.Value + " " + projectIdString));
                            } else
                            {
                                claimList.Add(new Claim(claim.Type, claim.Value));
                            }
                        };

                        result = await userManager.AddClaimsAsync(identityUser, claimList);

                    }

                }
            }

            return Json(new
            {
                status = "successfully added users to project"
            });
           
        }


        [HttpPost]
        [Authorize(Policy = "DeveloperPolicy")]
        public IActionResult FindUsers(string input, int projectId)
        {
            try
            {
                //this is for the search function to add users to projects
                var someUsers = from u in userManager.Users
                                select u;
                var users = new List<IdentityUser>();
                if (!String.IsNullOrEmpty(input))
                {
                    users = userManager.Users.Where(user => user.UserName.Contains(input)).ToList();
                }


                var unassignedUsers = new List<IdentityUser>();
                //filter the users string to not include people that are already on the project
                var project = projectRepository.GetProject(projectId);
                var projectUsers = project.OwnerId + " " + project.UsersAssigned;

                foreach (var user in users)
                {
                    if (projectUsers.Contains(user.Id) == false)
                    {
                        unassignedUsers.Add(user);
                    }
                }


                return Json(new
                {

                    status = "successfully found people",
                    users = unassignedUsers
                });
            }
            catch (Exception ex)
            {
                // to do : log error
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpGet]
        //[Authorize(Policy = "ManagerPolicy")]
        public async Task<IActionResult> EditUser(string id, int projectId)
        {

            GlobalVar.ProjectId = projectId;


            var project = projectRepository.GetProject(GlobalVar.ProjectId);

            GlobalVar.Project = project;

            var UserIsMangerLevel = UserClaimsLevel.IsManager(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsMangerLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                //Claims = projectUserClaims.Select(c => c.Type + " : ").ToList(),
                ProjectId = projectId
            };


            // GetClaimsAsync returns the list of user Claims
            var allUserClaims = await userManager.GetClaimsAsync(user);
            //var projectUserClaims = new List<Claim>();

            var projectIdString = projectId.ToString();

            foreach (var claim in allUserClaims)
            {
                var projectList = claim.Value.Split(" ");
                var claimString = "";

                for (int i = 0; i < projectList.Length; i++)
                {
                    if (projectList[i] == projectIdString)
                    {
                        claimString = claim.Type + " : true";
                        model.Claims.Add(claimString);
                        break;
                    }
                }
                if(claimString == "")
                {
                    model.Claims.Add(claim.Type + " : false");
                }

            }


            // GetRolesAsync returns the list of user Roles
            //I'm not using roles
            //var userRoles = await userManager.GetRolesAsync(user);

          

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeveloperPolicy")]

        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            //else if (model.Id == userManager.GetUserId(HttpContext.User))
            //{
            //    return RedirectToAction("AccessDenied", "Account");
            //}
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers", new { projectId = model.ProjectId});
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "ManagerPolicy")]

        public async Task<IActionResult> ManageUserClaims(string userId, int projectId)
        {

            GlobalVar.ProjectId = projectId;

            var project = projectRepository.GetProject(GlobalVar.ProjectId);

            GlobalVar.Project = project;

            var UserIsMangerLevel = UserClaimsLevel.IsManager(HttpContext.User.Claims.ToList(), projectId);

            if (UserIsMangerLevel == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else if (userId == userManager.GetUserId(HttpContext.User) || userId == project.OwnerId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                ProjectId = projectId,
                UserId = userId
            };

            var projectIdString = projectId.ToString();

            for (var i = 0; i < ClaimsStore.AllClaims.Count; i++)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = ClaimsStore.AllClaims[i].Type
                };

                var projectList = new List<string>();
                
                if (existingUserClaims.Count == 4)
                {
                    projectList = existingUserClaims[i].Value.Split(" ").ToList();
                }

                for (int j = 0; j < projectList.Count; j++)
                {
                    if (projectList[j] == projectIdString)
                    {
                        userClaim.IsSelected = true;
                        break;
                    }
                }
                model.Claims.Add(userClaim);
            }

            return View(model);

        }

        [HttpPost]
        [Authorize(Policy = "ManagerPolicy")]

        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            var project = projectRepository.GetProject(model.ProjectId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            if (model.UserId == userManager.GetUserId(HttpContext.User) || model.UserId == project.OwnerId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var currentClaims = claims.ToList();
            var result = await userManager.RemoveClaimsAsync(user, claims);


            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            var claimList = new List<Claim>();
            var projectIdString = model.ProjectId.ToString();

            for (var i = 0; i < model.Claims.Count; i++)
            {


                var projectList = new List<string>();
                var projectListString = "";
                if (currentClaims.Count == 4)
                {
                    projectList = currentClaims[i].Value.Split(" ").ToList();
                    var currentClaimsContainsId = ClaimStringContainsProjectId.Check(projectIdString, projectList);
                    if (currentClaimsContainsId && model.Claims[i].IsSelected)
                    {
                        projectListString = string.Join(" ", projectList.ToArray());
                    }else if (currentClaimsContainsId == false && model.Claims[i].IsSelected)
                    {
                        projectList.Add(projectIdString);
                        projectListString = string.Join(" ", projectList.ToArray());
                    } else if(currentClaimsContainsId && model.Claims[i].IsSelected == false)
                    {
                        var listWithRemovedId = ClaimStringContainsProjectId.RemoveProjectIdFromList(projectIdString, projectList);
                        projectListString = string.Join(" ", listWithRemovedId.ToArray());
                    } else if(currentClaimsContainsId == false && model.Claims[i].IsSelected == false)
                    {
                        projectListString = string.Join(" ", projectList.ToArray());
                    }

                }

                claimList.Add(new Claim(model.Claims[i].ClaimType, projectListString));
               
            }

            result = await userManager.AddClaimsAsync(user, claimList);

            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId, ProjectId = model.ProjectId });

        }

    }
}