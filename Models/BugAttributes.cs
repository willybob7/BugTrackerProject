using BugTrackerProject.Models.SubModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models

{
    public class BugAttributes
    {
        [Key]
        public int BugId { get; set; }
        public int AssociatedProject { get; set; }
        public string ReporterID { get; set; }
        public string ReporterUserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EnteredDate { get; set; }
        public DateTime DueDate { get; set; }
        public BugStatus Status { get; set; }
        public string AssingeeUserName { get; set; }
        public string AssigneeUserId { get; set; }
        //public String UsersAssigned { get; set; }// do this once we have authentication
        public BugSeverity Severity { get; set; }
        public IsItReproducible Reproducible { get; set; }
        public List<ScreenShots> ScreenShots { get; set; }
        public List<Comment> Comments { get; set; }

    }
}
