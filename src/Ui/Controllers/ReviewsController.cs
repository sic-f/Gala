using Core.Dto;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController(ApplicationDbContext context) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReview()
        {
            return await context.Review.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(Guid id)
        {
            var review = await context.Review.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(Guid id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            context.Entry(review).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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
        public async Task<ActionResult<Review>> PostReview(ReviewDetails details)
        {
            var review = new Review();
            review.SetDetails(details, this.BroId(User));

            context.Review.Add(review);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = review.Id }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var review = await context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            context.Review.Remove(review);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(Guid id)
        {
            return context.Review.Any(e => e.Id == id);
        }
    }
}
