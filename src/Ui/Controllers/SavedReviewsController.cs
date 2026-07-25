using Context;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/Saved-Reviews")]
    [ApiController]
    public class SavedReviewsController(ApplicationDbContext context, Bookmark bookmark) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedReview>>> GetSavedReview()
        {
            return await context.SavedReview.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SavedReview>> GetSavedReview(Guid id)
        {
            var savedReview = await context.SavedReview.FindAsync(id);

            if (savedReview == null)
            {
                return NotFound();
            }

            return savedReview;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSavedReview(Guid id, SavedReview savedReview)
        {
            if (id != savedReview.Id)
            {
                return BadRequest();
            }

            context.Entry(savedReview).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SavedReviewExists(id))
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

        [HttpPost]
        public async Task<ActionResult<SavedReview>> PostSavedReview(Core.Dto.Saved saved)
        {
            SavedReview savedReview = await bookmark.SaveReviewAsync(saved.SaveableId, this.BroId(User));

            return CreatedAtAction("GetSavedReview", new { id = savedReview.Id }, savedReview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSavedReview(Guid id)
        {
            var savedReview = await context.SavedReview.FindAsync(id);
            if (savedReview == null)
            {
                return NotFound();
            }

            context.SavedReview.Remove(savedReview);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool SavedReviewExists(Guid id)
        {
            return context.SavedReview.Any(e => e.Id == id);
        }
    }
}
