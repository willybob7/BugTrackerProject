using BugTrackerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class UserLevel : AuthorizationHandler<UserClaimsReqirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserClaimsReqirement requirement)
        {

            //var authFilterContext = context.Resource as AuthorizationFilterContext;

            //if (authFilterContext == null)
            //{
            //    return Task.CompletedTask;
            //}


            var UserProjects = new List<int>();

            var UserClaims = context.User.FindFirst(c => c.Type == "User Role");
            var DeveloperClaims = context.User.FindFirst(c => c.Type == "Developer Role");
            var ManagerClaims = context.User.FindFirst(c => c.Type == "Manager Role");
            var AdminClaims = context.User.FindFirst(c => c.Type == "Admin Role");

            var claimProjects = UserClaims.Value.Split(" ").ToList();
            claimProjects.AddRange(DeveloperClaims.Value.Split(" ").ToList());
            claimProjects.AddRange(ManagerClaims.Value.Split(" ").ToList());
            claimProjects.AddRange(AdminClaims.Value.Split(" ").ToList());

            foreach (var projectId in claimProjects)
            {
                if (projectId.Length > 0)
                {
                    UserProjects.Add(Convert.ToInt32(projectId));
                }
            }


            if (UserProjects.Contains(GlobalVar.ProjectId))
            {
                context.Succeed(requirement);
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
