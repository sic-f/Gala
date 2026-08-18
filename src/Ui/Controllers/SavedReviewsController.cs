using Context;
using Core.Dto;
using Core.SavedAggregate;
using Microsoft.AspNetCore.Mvc;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/Saved-Reviews")]
    [ApiController]
    public class SavedReviewsController(Bookmark bookmark) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<SavedDto> GetSavedReviews(DateTimeOffset offset, int limit)
        {
            return bookmark.GetSavedReviews(offset, limit, this.BroId(User));
        }

        //[HttpGet("{id}")]
        //public async ActionResult<SavedReview> GetSavedReview(Guid id)
        //{
        //}

        [HttpPost]
        public async Task<ActionResult<SavedReview>> PostSavedReview(Core.Dto.Saved saved)
        {
            SavedReview savedReview = await bookmark.SaveReviewAsync(saved.SaveableId, this.BroId(User));

            return CreatedAtAction(null, savedReview);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteSavedReview(Guid id)
        //{
        //var savedReview = await context.SavedReview.FindAsync(id);
        //if (savedReview == null)
        //{
        //    return NotFound();
        //}

        //context.SavedReview.Remove(savedReview);
        //await context.SaveChangesAsync();

        //return NoContent();
        //}
    }
}
