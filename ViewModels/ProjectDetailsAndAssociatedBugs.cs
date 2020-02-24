using BugTrackerProject.Models;
using BugTrackerProject.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class ProjectDetailsAndAssociatedBugs
    {
        public ProjectDetailsAndAssociatedBugs()
        {
            projectHistories = new List<ProjectHistory>();
        }

        public IEnumerable<BugAttributes> ProjectBugs { get; set; }
        public ProjectAttributes Project { get; set; }
        public List<ProjectHistory> projectHistories { get; set; }
        public int ProjectId { get; set; }
    }
}
