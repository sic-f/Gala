using Core.Dto;
using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ui.Controllers
{
    [Route("api/Place-Pictures")]
    [ApiController]
    public class PlacePicturesController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<PlacePictureDto>> GetPlacePicture(Guid id, int slide)
        {
            var placePicture = await context.PlacePicture.FindAsync(id);

            if (placePicture is null)
            {
                return NotFound("Place picture not found.");
            }

            return new PlacePictureDto(Id: placePicture.Id,
                                       Normal: placePicture.Normal,
                                       PlaceId: placePicture.PlaceId,
                                       UploaderId: placePicture.UploaderId,
                                       Index: slide,
                                       Caption: placePicture?.Caption);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlacePicture(Guid id, PictureCaption pictureCaption)
        {
            PlacePicture? picture = await context.PlacePicture.FindAsync(id);

            if (picture is null) return NotFound("Picture does not exist.");
            if (string.IsNullOrEmpty(picture.Caption) && string.IsNullOrEmpty(pictureCaption.Caption))
            {
                ModelState.AddModelError(nameof(PlacePicture.Caption), "Content is blank. Either input content or cancel.");
                return ValidationProblem(ModelState);
            }

            picture.Caption = pictureCaption.Caption;
            picture.LastModified = DateTimeOffset.UtcNow;

            context.PlacePicture.Update(picture);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacePictureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Picture = picture, Message = "Updated successfuly." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlacePicture(Guid id, IDeletePicture deleteImage)
        {
            PlacePicture? placePicture = await context.PlacePicture.FindAsync(id);
            if (placePicture is null) return NotFound("Picture not found.");

            try
            {
                await deleteImage.ComputeAsync(placePicture);
                await context.SaveChangesAsync();
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
