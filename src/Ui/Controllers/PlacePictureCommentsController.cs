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
    [Route("api/Place-Pictures/{placePictureId}/Comments")]
    [ApiController]
    public class PlacePictureCommentsController(ApplicationDbContext context, Comments comments) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<CommentDto> GetPlacePictureComments(Guid placePictureId, DateTimeOffset offset, int limit)
        {
            return comments.GetPlacePictureComments(placePictureId, offset, limit);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlacePictureComment>> GetPlacePictureComment(Guid id)
        {
            var placePictureComment = await context.PlacePictureComment.FindAsync(id);

            if (placePictureComment == null)
            {
                return NotFound();
            }

            return placePictureComment;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlacePictureComment(Guid id, PlacePictureComment placePictureComment)
        {
            if (id != placePictureComment.Id)
            {
                return BadRequest();
            }

            context.Entry(placePictureComment).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacePictureCommentExists(id))
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
        public async Task<IActionResult> PostPlacePictureComment(Guid placePictureId, CommentDetails details)
        {
            await comments.WriteCommentAsync(details, placePictureId, this.BroId(User));

            return CreatedAtAction(null, null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlacePictureComment(Guid id)
        {
            var placePictureComment = await context.PlacePictureComment.FindAsync(id);
            if (placePictureComment == null)
            {
                return NotFound();
            }

            context.PlacePictureComment.Remove(placePictureComment);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlacePictureCommentExists(Guid id)
        {
            return context.PlacePictureComment.Any(e => e.Id == id);
        }
    }
}
