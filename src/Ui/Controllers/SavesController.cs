using Core.Dto;
using Core.Exceptions;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavesController(ApplicationDbContext context, Context.Bookmark bookmark) : ControllerBase
    {
        private SavedPlace _savedPlace = default!;

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<SavedDto>> GetSavedAsync(DateTimeOffset offset, int limit)
        {
            return await bookmark.FetchBookmarksAsync(offset, limit, this.BroId(User));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SavedPlace>> PostSaved(Core.Dto.Saved saved)
        {
            try
            {

                _savedPlace = await bookmark.SavePlaceAsync(saved.SaveableId, this.BroId(User));
            }
            catch (SaveOwnPlaceException ex)
            {
                ModelState.AddModelError(nameof(SavedPlace), ex.Message);

                return Problem(
                    ex.Message,
                    "/POST /saves/{id}",
                    422,
                    "Operation not allowed on resource.");
            }
            return CreatedAtAction("GetSaved", new { id = _savedPlace.Id }, _savedPlace);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaved(Guid id)
        {
            var saved = await context.Saved.FindAsync(id);
            if (saved == null)
            {
                return NotFound();
            }

            context.Saved.Remove(saved);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool SavedExists(Guid id)
        {
            return context.Saved.Any(e => e.Id == id);
        }
    }
}
