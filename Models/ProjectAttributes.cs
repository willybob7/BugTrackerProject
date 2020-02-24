using BugTrackerProject.Models.SubModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class ProjectAttributes
    {
        [Key]
        public int ProjectId { get; set; }
        [Required]
        [Display(Name ="Project Name")]
        public string ProjectName { get; set; }
        [Required]
        [Display(Name ="Project Description")]
        public string ProjectDescription { get; set; }
        public string UsersAssigned { get; set; }
        public string OwnerId { get; set; }
        public string OwnerUserName { get; set; }
        [Required]
        public ProjectStatus ProjectStatus{ get; set; }
        public List<ProjectBugs> ProjectBugs { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
       

    }
}
