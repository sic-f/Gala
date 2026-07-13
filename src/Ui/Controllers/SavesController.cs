using Core.Dto;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavesController(ApplicationDbContext context, Context.Saved bookmark) : ControllerBase
    {
        private SavedPlace _savedPlace = default!;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Saved>>> GetSaved()
        {
            return await context.Saved.ToListAsync();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaved(Guid id, Saved saved)
        {
            if (id != saved.Id)
            {
                return BadRequest();
            }

            context.Entry(saved).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SavedExists(id))
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
        public async Task<ActionResult<Saved>> PostSaved(SavedDto saved)
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
