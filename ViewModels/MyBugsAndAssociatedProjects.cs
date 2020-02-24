using BugTrackerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class MyBugsAndAssociatedProjects
    {
        public IEnumerable<BugAttributes> MyBugs { get; set; }
        public IEnumerable<ProjectAttributes> AssociatedProjects { get; set; }
    }
}
