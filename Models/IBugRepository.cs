using BugTrackerProject.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public interface IBugRepository
    {
        BugAttributes Add(BugAttributes bug);
        BugAttributes Delete(int id);
        IEnumerable<BugAttributes> GetAllProjectBugs(int projectId);
        List<BugAttributes> GetAllAssigneeBugs(string userId);
        BugAttributes GetBug(int id);
        BugAttributes Update(BugAttributes bugChanges);
        List<ScreenShots> AddScreenShots(List<ScreenShots> addedScreenShots);
        List<ScreenShots> ScreenShots(int bug);
        ScreenShots DeleteScreenShots(int screenShotId);
        List<Comment> Comments(int bug);
        Comment AddComment(Comment comment);
        Comment DeleteComment(int id);

        Comment UpdateComment(Comment comment);
        BugHistory AddHistoryEntry(BugHistory changes);
        List<BugHistory> GetBugHistories(int bugId);

    }
}
