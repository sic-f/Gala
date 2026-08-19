using Core.Dto;
using Core.Exceptions;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController(ApplicationDbContext context, Context.Review review) : ControllerBase
    {
        private Review _review = default!;

        [HttpGet]
        public async Task<IEnumerable<ReviewDto>> GetReview(int offset, int limit, Guid placeId)
        {
            string? broId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await review.FetchPaginatedReviews(offset, limit, placeId, broId);
        }

        [HttpGet("{id}")]
        public ReviewDto GetReview(Guid id)
        {
            return review.GetReview(id);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(Guid id, ReviewDetails details)
        {
            try
            {
                await review.ModifyAsync(id, details, this.BroId(User));
            }
            catch (UnauthorizedReviewModificationException ex)
            {
                return Problem(ex.Message,
                               $"PUT /reviews/{id}",
                               403,
                               "You do not have permission to modify this review.");
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
            _review = await review.WriteAsync(details, this.BroId(User));

            return CreatedAtAction(nameof(GetReview), new { _review.Id }, _review);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            await review.DeleteAsync(id);

            return NoContent();
        }

        private bool ReviewExists(Guid id)
        {
            return context.Review.Any(e => e.Id == id);
        }
    }
}
