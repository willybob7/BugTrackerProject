using BugTrackerProject.Models.SubModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BugTrackerProject.Models
{
    public class SqlBugRepository : IBugRepository
    {
        private readonly AppDbContext context;

        public SqlBugRepository(AppDbContext context)
        {
            this.context = context;

        }
        public BugAttributes Add(BugAttributes bug)
        {
            context.Bugs.Add(bug);
            context.SaveChanges();
            return bug;
        }

        public BugAttributes Delete(int id)
        {
            //update this to include comments
            var bug = context.Bugs.Find(id);
            if(bug != null)
            {

                var associatedScreenshots = context.ScreenShots.Where(s => bug.BugId == s.AssociatedBug);
                foreach (var file in associatedScreenshots)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", file.FilePath);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    context.ScreenShots.Remove(file);
                }

                var bugScreenshots = context.Comments.Where(c => c.AssociatedBugId == id);
                if (bugScreenshots != null && bugScreenshots.Count() > 0)
                {
                    //foreach (var screenShot in bugScreenshots)
                    //{
                        context.Comments.RemoveRange(bugScreenshots);
                    //}
                }

                var bugHistory = context.BugHistory.Where(b => b.AssociatedBugId == id);
                context.BugHistory.RemoveRange(bugHistory);

                var projectBug = context.ProjectBugs.FirstOrDefault(b => b.BugId == id);

                context.ProjectBugs.Remove(projectBug);
                context.Bugs.Remove(bug);
                context.SaveChanges();
            }
            return bug;
        }

        public List<BugAttributes> GetAllAssigneeBugs(string userId)
        {
            return context.Bugs.Where(b => b.AssigneeUserId == userId).ToList();
        }

        public IEnumerable<BugAttributes> GetAllProjectBugs(int projectId)
        {
            return context.Bugs.Where(b => b.AssociatedProject == projectId);
        }

        public BugAttributes GetBug(int id)
        {
            
            //return context.Bugs.Find(id);
            return context.Bugs.AsNoTracking().First(b => b.BugId== id);
        }

        public BugAttributes Update(BugAttributes bugChanges)
        {
            var bug = context.Bugs.Attach(bugChanges);
            bug.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return bugChanges;
        }

        public List<ScreenShots> ScreenShots(int bug)
        {
            return context.ScreenShots.Where(s => s.AssociatedBug == bug).ToList();
        }

        public List<ScreenShots> AddScreenShots(List<ScreenShots> addedScreenShots)
        {
            context.ScreenShots.AddRange(addedScreenShots);
            context.SaveChanges();
            return addedScreenShots;
        }

        public ScreenShots DeleteScreenShots(int screenShotId)
        {
            var screenShot = context.ScreenShots.Find(screenShotId);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshots", screenShot.FilePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            context.Remove(screenShot);
            context.SaveChanges();
            return screenShot;
        }

        public List<Comment> Comments(int bug)
        {
            return context.Comments.Where(c => c.AssociatedBugId == bug).ToList();
        }

        public Comment AddComment (Comment comment)
        {
            context.Comments.Add(comment);
            context.SaveChanges();
            return comment;
        }

        public Comment DeleteComment (int id)
        {
            var comment = context.Comments.Find(id);
            context.Remove(comment);
            context.SaveChanges();
            return comment;
        }

        public Comment UpdateComment (Comment comment)
        {
            var updatedComment = context.Comments.Attach(comment);
            updatedComment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return comment;
        }

        public BugHistory AddHistoryEntry (BugHistory newEntry)
        {
            context.BugHistory.Add(newEntry);
            context.SaveChanges();
            return newEntry;
        }

        public List<BugHistory> GetBugHistories (int bugId)
        {
            return context.BugHistory.Where(b => b.AssociatedBugId == bugId).ToList();
        }




    }
}
