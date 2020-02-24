using BugTrackerProject.Models;
using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.ViewModels
{
    public class BugDetailsAndProjectNameAndId
    {
        public BugAttributes Bug { get; set; }
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
        public int Updated { get; set; }
        public List<ScreenShots> Src { get; set; }
        public List<IFormFile> ScreenShots { get; set; }
        public List<BugHistory> bugHistories { get; set; }
        public List<IdentityUser> ProjectUsers { get; set; }


    }
}
