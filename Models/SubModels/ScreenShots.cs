using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models.SubModels
{
    public class ScreenShots
    {
        public int id { get; set; }
        public int AssociatedBug { get; set; }
        [MaxLength(400)]
        public string Url { get; set; }
        [MaxLength(200)]
        public string FileName { get; set; }
    }
}
