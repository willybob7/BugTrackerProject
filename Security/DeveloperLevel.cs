using BugTrackerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class DeveloperLevel : AuthorizationHandler<DeveloperClaimsRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            DeveloperClaimsRequirement requirement)
        {

            //var thing = context.Resource;

            //var authFilterContext = context.Resource as AuthorizationFilterContext;

            //if (authFilterContext == null)
            //{
            //    return Task.CompletedTask;
            //}

            var DeveloperProjects = new List<int>();

            //var things = context.User.Identity;

            if (GlobalVar.globalCurrentUserClaims != null)
            {
                var DeveloperClaims = GlobalVar.globalCurrentUserClaims.Find(c => c.Type == "Developer Role");
                var ManagerClaims = GlobalVar.globalCurrentUserClaims.Find(c => c.Type == "Manager Role");
                var AdminClaims = GlobalVar.globalCurrentUserClaims.Find(c => c.Type == "Admin Role");

                var claimProjects = DeveloperClaims.Value.Split(" ").ToList();
                claimProjects.AddRange(ManagerClaims.Value.Split(" ").ToList());
                claimProjects.AddRange(AdminClaims.Value.Split(" ").ToList());


                foreach (var projectId in claimProjects)
                {
                    if (projectId.Length > 0)
                    {
                        DeveloperProjects.Add(Convert.ToInt32(projectId));
                    }
                }


                if (DeveloperProjects.Contains(GlobalVar.ProjectId))
                {
                    context.Succeed(requirement);
                }

            } else
            {

                var DeveloperClaims = context.User.FindFirst(c => c.Type == "Developer Role");
                var ManagerClaims = context.User.FindFirst(c => c.Type == "Manager Role");
                var AdminClaims = context.User.FindFirst(c => c.Type == "Admin Role");

                var claimProjects = DeveloperClaims.Value.Split(" ").ToList();
                claimProjects.AddRange(ManagerClaims.Value.Split(" ").ToList());
                claimProjects.AddRange(AdminClaims.Value.Split(" ").ToList());


                foreach (var projectId in claimProjects)
                {
                    if (projectId.Length > 0)
                    {
                        DeveloperProjects.Add(Convert.ToInt32(projectId));
                    }
                }


                if (DeveloperProjects.Contains(GlobalVar.ProjectId))
                {
                    context.Succeed(requirement);
                }

            }




            //string loggedInAdminId =
            //    context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            //string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];

            //if (context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
            //    adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            //{
            //    context.Succeed(requirement);
            //}

            return Task.CompletedTask;
        }
    }
}
