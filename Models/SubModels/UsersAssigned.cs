using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models.SubModels
{
    public class UsersAssigned
    {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AssignedUserId { get; set; }
    }
}
