using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BugTrackerProject.Models;
using BugTrackerProject.ViewModels;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using BugTrackerProject.Models.SubModels;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace BugTrackerProject.Controllers
{
    public class BugCommentController : Controller
    {

        private readonly ILogger<BugCommentController> _logger;
        private readonly IBugRepository _bugRepository;

        public BugCommentController(ILogger<BugCommentController> logger,
            IBugRepository bugRepository)
        {
            _logger = logger;
            _bugRepository = bugRepository;
        }




        [HttpPost]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult UploadComment(string comment, string userId, int associatedProject, int associatedBug)
        {
            try
            {
                var uploadedComment = new Comment
                {
                    AssociatedBugId = associatedBug,
                    ProjectId = associatedProject,
                    UserId = userId,
                    CommentText = comment,
                    CreatedDate = DateTime.Now
                };
                var newComment = _bugRepository.AddComment(uploadedComment);
                return Json(new
                {
                    status = "success",
                    comment = newComment.CommentText,
                    createdDate = newComment.CreatedDate,
                    userId = newComment.UserId,
                    commentId = newComment.Id,
                    actionUrl = Url.Action("deletecomment", "bugcomment", new { commentId = newComment.Id })
                });
            }
            catch (Exception ex)
            {
                // to do : log error
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "ManagerPolicy")]
        public IActionResult DeleteComment(int commentId)
        {
            try
            {
                _bugRepository.DeleteComment(commentId);
                return Json(new { status = "success", message = "comment deleted" });
            }
            catch (Exception ex)
            {
                // to do : log error
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "ManagerPolicy")]
        public IActionResult UpdateComment(string comment, string userId, int associatedProject, int associatedBug, int commentId)
        {
            try
            {
                var uploadedComment = new Comment
                {
                    AssociatedBugId = associatedBug,
                    ProjectId = associatedProject,
                    UserId = userId,
                    CommentText = comment,
                    CreatedDate = DateTime.Now,
                    Id = commentId

                };
                var newComment = _bugRepository.UpdateComment(uploadedComment);
                return Json(new
                {
                    status = "success",
                    comment = newComment.CommentText,
                    createdDate = newComment.CreatedDate,
                    userId = newComment.UserId,
                    commentId = newComment.Id,
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }


    }
}