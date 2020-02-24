using BugTrackerProject.Models;
using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class AddNewBug
    {
        public BugAttributes NewBugAttributes { get; set; }
        [Display(Name = "Add A Screenshot")]
        public List<IFormFile> ScreenShots { get; set; }
        public int ProjectId { get; set; }
        public List<IdentityUser> ProjectUsers { get; set; }


    }
}
