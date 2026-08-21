using Context;
using Core.Dto;
using Core.SavedAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/Saved-Reviews")]
    [ApiController]
    public class SavedReviewsController(Bookmark bookmark) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IEnumerable<SavedDto> GetSavedReviews(DateTimeOffset offset, int limit)
        {
            return bookmark.GetSavedReviews(offset, limit, this.BroId(User));
        }

        [HttpPost]
        public async Task<ActionResult<SavedReview>> PostSavedReview(Core.Dto.Saved saved)
        {
            SavedReview savedReview = await bookmark.SaveReviewAsync(saved.SaveableId, this.BroId(User));

            return CreatedAtAction(null, savedReview);
        }
    }
}
