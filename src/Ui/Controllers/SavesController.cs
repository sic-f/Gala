using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavesController(ApplicationDbContext context) : ControllerBase
    {

        // GET: api/Saves
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Saved>>> GetSaved()
        {
            return await context.Saved.ToListAsync();
        }

        // GET: api/Saves/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Saved>> GetSaved(Guid id)
        {
            var saved = await context.Saved.FindAsync(id);

            if (saved == null)
            {
                return NotFound();
            }

            return saved;
        }

        // PUT: api/Saves/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Saves
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Saved>> PostSaved(Saved saved)
        {
            context.Saved.Add(saved);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetSaved", new { id = saved.Id }, saved);
        }

        // DELETE: api/Saves/5
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
