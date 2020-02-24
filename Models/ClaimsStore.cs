using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Admin Role", "Admin Role"),
            new Claim("Manager Role","Manager Role"),
            new Claim("Developer Role","Developer Role"),
            new Claim("User Role","User Role")
        };
    }
}
