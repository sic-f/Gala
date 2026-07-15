using Core.Dto;
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
        public ActionResult<IEnumerable<PlaceBookmark>> GetSaved(DateTimeOffset offset, int limit, Guid broId)
        {
            return bookmark.FetchBookmarks(offset, limit, broId).ToList();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SavedPlace>> PostSaved(Core.Dto.Saved saved)
        {
            _savedPlace = await bookmark.SavePlaceAsync(saved.PlaceId, this.BroId(User));

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
