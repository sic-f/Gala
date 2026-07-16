using Core.Dto;
using Core.Exceptions;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ui.Controllers
{
    [Route("api/Place-Pictures")]
    [ApiController]
    public class PlacePicturesController(ApplicationDbContext context, Context.PlacePicture placePicture) : ControllerBase
    {
        private PlacePicture Picture { get; set; } = default!;

        [HttpGet("{id}")]
        public async Task<ActionResult<PlacePictureDto>> GetPlacePicture(Guid id, int slide)
        {
            return placePicture.DisplayPicture(id, slide);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlacePicture(Guid id, PictureCaption pictureCaption)
        {
            try
            {
                Picture = placePicture.ModifyCaption(id, pictureCaption.Caption);
            }
            catch (BlankPictureCaptionException ex)
            {

                ModelState.AddModelError(nameof(PlacePicture.Caption),
                                         ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PlacePictureExists(id))
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = ex.Message,
                        Instance = $"PUT api/PlacePictures/${id}",
                        Title = "Db Update Concurrency Exception"
                    });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ModifyPlacePictureCaption(Id: id,
                                                    Caption: Picture.Caption,
                                                    UploaderId: Picture.UploaderId,
                                                    Message: "Updated successfuly."));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlacePicture(Guid id)
        {
            try
            {
                placePicture.DeletePicture(id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PlacePictureExists(id))
                {
                    return Problem($"{ex.Message}",
                                   $"DELETE /place-pictures/{id}",
                                   404,
                                   "Something went wrong.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PlacePictureExists(Guid id)
        {
            return context.PlacePicture.Any(e => e.Id == id);
        }
    }
}
