using BugTrackerProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class AddProjectViewModel
    {
        public string UserId { get; set; }
        public ProjectAttributes Project { get; set; }
    }
}
