using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models.SubModels
{
    public class ScreenShots
    {
        public int id { get; set; }
        public int AssociatedBug { get; set; }
        public string FilePath { get; set; }
    }
}
