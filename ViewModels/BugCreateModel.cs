using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class BugCreateModel
    {
        [Key]
        public int BugId { get; set; }
        public int AssociatedProjectId { get; set; }
        public int ReporterID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EnteredDate { get; set; }
        public DateTime DueDate { get; set; }
        public BugStatus Status { get; set; }
        public int AssingeeId { get; set; }
        //public List<UsersAssigned> UsersAssigned { get; set; }// do this once we have authentication

        public BugSeverity Severity { get; set; }
        public IsItReproducible Reproducible { get; set; }
        [Display(Name ="Add A Screenshot")]
        public List<IFormFile> ScreenShots { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
