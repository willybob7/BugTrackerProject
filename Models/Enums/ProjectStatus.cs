using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public enum ProjectStatus
    {
        Active, 
        OnTrack, 
        Delayed, 
        InTesting,
        OnHold, 
        Cancelled, 
        InPlanning, 
        Completed
    }
}
