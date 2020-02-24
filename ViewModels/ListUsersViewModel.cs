using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class ListUsersViewModel
    {
        public List<IdentityUser> Users { get; set; }
        public int ProjectId { get; set; }
        public string OwnerId { get; set; }
    }
}
