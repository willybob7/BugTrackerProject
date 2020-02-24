using BugTrackerProject.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class AdministratorLevel : AuthorizationHandler<AdminClaimsRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminClaimsRequirement requirement)
        {

            //var thing = context.Resource;

            //var authFilterContext = context.Resource as AuthorizationFilterContext;

            //if (authFilterContext == null)
            //{
            //    return Task.CompletedTask;
            //}

            var AdministratorProjects = new List<int>();

            var things = context.User.Identity;



            var UserClaims = context.User.FindFirst(c => c.Type == "Admin Role");

            var claimProjects = UserClaims.Value.Split(" ").ToList();

            foreach (var projectId in claimProjects)
            {
                if (projectId.Length > 0)
                {
                    AdministratorProjects.Add(Convert.ToInt32(projectId));
                }
            }


            if (AdministratorProjects.Contains(GlobalVar.ProjectId))
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
