using BugTrackerProject.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class ManagerLevel : AuthorizationHandler<ManagerClaimsRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManagerClaimsRequirement requirement)
        {

            var ManagerProjects = new List<int>();

            //var things = context.User.Identity;


            if (GlobalVar.globalCurrentUserClaims != null)
            {

                var ManagerClaims = GlobalVar.globalCurrentUserClaims.Find(c => c.Type == "Manager Role");
                var AdminClaims = GlobalVar.globalCurrentUserClaims.Find(c => c.Type == "Admin Role");


                var claimProjects = ManagerClaims.Value.Split(" ").ToList();
                claimProjects.AddRange(AdminClaims.Value.Split(" ").ToList());


                foreach (var projectId in claimProjects)
                {
                    if (projectId.Length > 0)
                    {
                        ManagerProjects.Add(Convert.ToInt32(projectId));
                    }
                }


                if (ManagerProjects.Contains(GlobalVar.ProjectId))
                {
                    context.Succeed(requirement);
                }
            } else
            {
                var ManagerClaims = context.User.FindFirst(c => c.Type == "Manager Role");
                var AdminClaims = context.User.FindFirst(c => c.Type == "Admin Role");


                var claimProjects = ManagerClaims.Value.Split(" ").ToList();
                claimProjects.AddRange(AdminClaims.Value.Split(" ").ToList());


                foreach (var projectId in claimProjects)
                {
                    if (projectId.Length > 0)
                    {
                        ManagerProjects.Add(Convert.ToInt32(projectId));
                    }
                }


                if (ManagerProjects.Contains(GlobalVar.ProjectId))
                {
                    context.Succeed(requirement);
                }
            }


            return Task.CompletedTask;
        }
    }
}
