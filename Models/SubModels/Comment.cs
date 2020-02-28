using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models.SubModels
{
    public class Comment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int AssociatedBugId { get; set; }
        public string CommentText { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
