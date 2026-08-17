using Context;
using Core.Dto;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/Saved-Places")]
    [ApiController]
    public class SavedPlacesController(ApplicationDbContext context, Bookmark bookmark) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IEnumerable<SavedDto> GetSavedPlaces(DateTimeOffset offset, int limit)
        {
            return bookmark.GetSavedPlaces(offset, limit, broId: this.BroId(User));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SavedPlace>> GetSavedPlace(Guid id)
        {
            var savedPlace = await context.SavedPlace.FindAsync(id);

            if (savedPlace == null)
            {
                return NotFound();
            }

            return savedPlace;
        }

        [HttpPost]
        public async Task<ActionResult<SavedPlace>> PostSavedPlace(SavedPlace savedPlace)
        {
            context.SavedPlace.Add(savedPlace);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetSavedPlace", new { id = savedPlace.Id }, savedPlace);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSavedPlace(Guid id)
        {
            var savedPlace = await context.SavedPlace.FindAsync(id);
            if (savedPlace == null)
            {
                return NotFound();
            }

            context.SavedPlace.Remove(savedPlace);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
