using BugTrackerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerProject.Security
{
    public class UserClaimsLevel
    {

        public static bool IsUser(List<Claim> claims, int projectId)
        {
            var UserClaims = claims.Find(c => c.Type == "User Role");
            var DeveloperClaims = claims.Find(c => c.Type == "Developer Role");
            var ManagerClaims = claims.Find(c => c.Type == "Manager Role");
            var AdminClaims = claims.Find(c => c.Type == "Admin Role");


            var projectStrings = new List<string>();

            projectStrings.AddRange(UserClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(DeveloperClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(ManagerClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(AdminClaims.Value.Split(" ").ToList());

            if (projectStrings.Contains(projectId.ToString()))
            {
                return true;
            }

            return false;

        }


        //check if the user is a developer 
        public static bool IsDeveloper(List<Claim> claims, int projectId)
        {

            var DeveloperClaims = claims.Find(c => c.Type == "Developer Role");
            var ManagerClaims = claims.Find(c => c.Type == "Manager Role");
            var AdminClaims = claims.Find(c => c.Type == "Admin Role");


            var projectStrings = new List<string>();

            projectStrings.AddRange(DeveloperClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(ManagerClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(AdminClaims.Value.Split(" ").ToList());

            if (projectStrings.Contains(projectId.ToString()))
            {
                return true;
            }

            return false;
           
        }

        public static bool IsManager(List<Claim> claims, int projectId)
        {
            var ManagerClaims = claims.Find(c => c.Type == "Manager Role");
            var AdminClaims = claims.Find(c => c.Type == "Admin Role");


            var projectStrings = new List<string>();

            projectStrings.AddRange(ManagerClaims.Value.Split(" ").ToList());
            projectStrings.AddRange(AdminClaims.Value.Split(" ").ToList());

            if (projectStrings.Contains(projectId.ToString()))
            {
                return true;
            }

            return false;
        }

        public static bool IsAdmin(List<Claim> claims, int projectId)
        {
            var AdminClaims = claims.Find(c => c.Type == "Admin Role");


            var projectStrings = new List<string>();

            projectStrings.AddRange(AdminClaims.Value.Split(" ").ToList());

            if (projectStrings.Contains(projectId.ToString()))
            {
                return true;
            }

            return false;
        }

    }
}
