using Context;
using Core.Dto;
using Core.Exceptions;
using Core.PlaceAggregate;
using Core.Values;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, UserManager<Bro> userManager) : ControllerBase
{
    private Place Place { get; set; } = default!;

    [HttpGet]
    public IEnumerable<PlaceDetailsHomePage> GetPlace(DateTimeOffset offset, int limit)
    {
        return context.Place.AsNoTracking()
                            .Include(p => p.ProfilePictures)
                            .Where(p => p.CreatedAt < offset)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(limit)
                            .ToList()
                            .Select(p =>
                            {
                                PlacePictureLean? picture = default;

                                if (p.Picture is not null)
                                {
                                    picture = new(Id: p.Picture.Id,
                                                  Small: p.Picture.Small,
                                                  PlaceId: p.Picture.PlaceId);
                                }
                                return new PlaceDetailsHomePage(Id: p.Id,
                                                                Name: p.Name,
                                                                Description: p.Description,
                                                                Information: p.Information,
                                                                Address: p.Address,
                                                                Mobile: p.Mobile,
                                                                Landline: p.Landline,
                                                                Email: p.Email,
                                                                Website: p.Website,
                                                                Facebook: p.Facebook,
                                                                Instagram: p.Instagram,
                                                                BroId: p.BroId,
                                                                Picture: picture,
                                                                CreatedAt: p.CreatedAt);
                            });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlaceDetails>> GetPlace(Guid id)
    {
        Place? place = context.Place.AsNoTracking()
                                    .Include(p => p.ProfilePictures)
                                    .Include(p => p.Pictures)
                                    .FirstOrDefault(p => p.Id == id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        PlacePictureLean? picture = default;
        IEnumerable<PlacePictureLean> pictures = [];

        if (place.Picture is not null)
        {
            picture = new(Id: place.Picture.Id, Small: place.Picture.Small, PlaceId: place.Picture.PlaceId);
        }

        if (place.Pictures.Count != 0)
        {
            pictures = place.Pictures.Select(p =>
                new PlacePictureLean(Id: p.Id, Small: p.Small, PlaceId: p.PlaceId)).ToList();
        }

        return new PlaceDetails(Id: place.Id,
                                Name: place.Name,
                                Description: place.Description,
                                Information: place.Information,
                                Address: place.Address,
                                Mobile: place.Mobile,
                                Landline: place.Landline,
                                Email: place.Email,
                                Website: place.Website,
                                Facebook: place.Facebook,
                                Instagram: place.Instagram,
                                BroId: place.BroId,
                                Picture: picture,
                                Pictures: pictures);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlace(Guid id,
                                              Dto.PlaceDetails details,
                                              ParseToPictureUploads parseToPictureUploads,
                                              Contribution contribution)
    {
        try
        {
            await ModifyPlaceAsync(details, contribution, parseToPictureUploads, id, this.BroId(User));
        }
        catch (ImageTypeException)
        {
            return ValidationProblem(ModelState);
        }
        catch (UnauthorizedContributionAccessException ex)
        {
            return Problem(ex.Message,
                           $"PUT /places/{id}",
                           403,
                           "You do not have permission to modify this resource.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!PlaceExists(id))
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message,
                    Instance = $"PUT api/Places/${Place.Id}",
                    Title = "Db Update Concurrency Exception"
                });
            }
            else
            {
                throw;
            }
        }

        return Ok(new { Message = "Updated successfuly" });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostPlace(Dto.PlaceDetails details,
                                               Contribution contribution,
                                               ParseToPictureUploads parseToPictureUploads)
    {
        try
        {
            Place = await SubmitPlaceAsync(details, parseToPictureUploads, contribution);
        }
        catch (ImageTypeException)
        {
            return ValidationProblem(ModelState);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!PlaceExists(Place.Id))
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message,
                    Instance = $"DELETE api/Places/${Place.Id}",
                    Title = "Db Update Concurrency Exception"
                });
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction(nameof(GetPlace), new { Place.Id }, new { Message = "Submit successful", Place.Id });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlace(Guid id, DeleteImage deleteImage)
    {
        var place = await context.Place.FindAsync(id);
        if (place == null)
        {
            return NotFound("❌🏤 Can't delete a place that does not exist.");
        }

        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is not null && bro.Id != place.BroId)
        {
            return BadRequest("❌👦🏣 Bro not allowed to delete other people's places");
        }

        IEnumerable<PlacePicture> placePictures = context.PlacePicture.Where(pp => pp.PlaceId == place.Id);

        await deleteImage.DeleteAllAsync(placePictures);
        context.Place.Remove(place);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlaceExists(id))
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

    private bool PlaceExists(Guid id)
    {
        return context.Place.Any(e => e.Id == id);
    }

    private async Task<Place> SubmitPlaceAsync(Dto.PlaceDetails details,
                                               ParseToPictureUploads pictureUploadsParser,
                                               Contribution contribution)
    {
        var placePictureUploads = await PlacePictureUploadsAsync(pictureUploadsParser, details);

        return await contribution.SubmitAsync(details, placePictureUploads, this.BroId(User));
    }

    private async Task ModifyPlaceAsync(Dto.PlaceDetails details,
                                        Contribution contribution,
                                        ParseToPictureUploads parseToPictureUploads,
                                        Guid placeId,
                                        Guid broId)
    {
        var pictureUploads = await PlacePictureUploadsAsync(parseToPictureUploads, details);

        await contribution.ModifyAsync(details, pictureUploads, placeId, broId);
    }

    private async Task<PictureUploads> PlacePictureUploadsAsync(ParseToPictureUploads pictureUploadsParser, Dto.PlaceDetails details)
    {
        return await pictureUploadsParser.FromFormFilesAsync<PlacePicture>(
            new UrlSegments(Request.Scheme, Request.Host.ToString()),
            details.Picture,
            details.Pictures,
            ModelState);
    }
}