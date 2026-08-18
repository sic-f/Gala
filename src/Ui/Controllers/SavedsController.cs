using Core.Dto;
using Core.Exceptions;
using Core.SavedAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ui.Services;

namespace Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedsController(Context.Bookmark bookmark) : ControllerBase
    {
        private SavedPlace _savedPlace = default!;

        [Authorize]
        [HttpGet]
        public IOrderedEnumerable<SavedDto> GetSavedsAsync(DateTimeOffset offset, int limit)
        {
            return bookmark.FetchBookmarks(offset, limit, this.BroId(User));
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
                return Problem(
                    ex.Message,
                    "/POST /saves/{id}",
                    422,
                    "Operation not allowed on resource.");
            }

            return CreatedAtAction(null, _savedPlace);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaved(Guid id)
        {
            try
            {
                await bookmark.DeleteSavedAsync(savedId: id);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message,
                    Instance = "DELETE /Saveds/{id}",
                    Title = "Saved not found."
                });
            }

            return NoContent();
        }
    }
}
