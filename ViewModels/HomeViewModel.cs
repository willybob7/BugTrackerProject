using BugTrackerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class HomeViewModel
    {
        public List<BugAttributes> MyBugs { get; set; }
        public List<ProjectAttributes> MyProjects { get; set; }
        public string UserId { get; set; }
    }
}
