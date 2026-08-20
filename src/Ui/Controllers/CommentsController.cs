using Context;
using Core.Dto;
using Core.Entities.Comment;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(ApplicationDbContext context, Comments comments) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComment()
        {
            return await context.Comment.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(Guid id)
        {
            var comment = await context.Comment.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            context.Entry(comment).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostComment(Guid placePictureId, CommentDetails details)
        {
            await comments.WriteCommentAsync(details, placePictureId, this.BroId(User));

            return CreatedAtAction(null, null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            context.Comment.Remove(comment);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(Guid id)
        {
            return context.Comment.Any(e => e.Id == id);
        }
    }
}
