using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService commentService;

        public CommentController()
        {
            commentService = new CommentService();
        }

        [Authorize(Roles = "user, admin")]
        [Route("comments")]
        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync(CommentREST comment)
        {
            var result = await commentService.CreateCommentAsync(new Comment(null, comment.EventId, comment.UserId, DateTime.UtcNow, comment.Message));

            if (result)
            {
                return Ok(comment);
            }

            return BadRequest("Comment not created.");
        }

        [Route("comments/{eventId}")]
        [HttpGet]
        public async Task<IActionResult> GetCommentsByEventIdAsync(string eventId)
        {
            var comments = await commentService.GetCommentsByEventIdAsync(eventId);

            if (comments != null)
            {
                return Ok(comments);
            }

            return BadRequest("Comments not found.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("comments/{commentId}")]
        [HttpPut]
        public async Task<IActionResult> UpdateCommentAsync(string id, CommentREST comment)
        {
            var existingComment = await commentService.GetCommentByIdAsync(id);

            if (existingComment == null)
            {
                return BadRequest("Comment not found.");
            }

            if (User.Claims.Any())
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id").Value;

                if (userId != existingComment.UserId)
                {
                    return BadRequest("You can't update other user's comments.");
                }
            }

            var result = await commentService.UpdateCommentAsync(new Comment(id, existingComment.EventId, existingComment.UserId, DateTime.Now, comment.Message));

            if(result)
            {
                return Ok(comment);
            }

            return BadRequest("Comments not updated.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("comments/{commentId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCommentAsync(string id)
        {
            var existingComment = await commentService.GetCommentByIdAsync(id);

            if (existingComment == null)
            {
                return BadRequest("Comment not found.");
            }

            if (User.Claims.Any())
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id").Value;

                if (userId != existingComment.UserId)
                {
                    return BadRequest("You can't delete other user's attendance.");
                }
            }

            var result = await commentService.DeleteCommentAsync(id);

            if (result)
            {
                return Ok("Comment deleted.");
            }

            return BadRequest("Comment not deleted.");
        }
    }

    public class CommentREST
    {
        public string UserId { get; set; }
        public string EventId { get; set; }
        public string Message { get; set; }
    }
}