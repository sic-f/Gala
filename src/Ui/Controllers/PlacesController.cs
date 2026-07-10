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

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, UserManager<Bro> userManager) : ControllerBase
{
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
                                              AttachImages attachImages,
                                              ParseToPictureUploads parseToPictureUploads)
    {
        Place? place = await context.Place.Include(p => p.ProfilePictures).FirstOrDefaultAsync(p => p.Id == id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is not null && place.BroId != bro.Id)
        {
            return Problem("❌👦🏣 Bro not allowed to update other people's places.",
                           $"PUT /places/{id}",
                           403,
                           "Forbidden action");
        }

        try
        {
            await attachImages.ComputeAsync(
                place,
                await parseToPictureUploads.FromFormFilesAsync<PlacePicture>(
                    new UrlSegments(Request.Scheme, Request.Host.ToString()),
                    details.Picture,
                    details.Pictures,
                    ModelState));
        }
        catch (ImageTypeException)
        {
            return ValidationProblem(ModelState);
        }

        place.UpdateDetails(details);

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

        return Ok(new { Message = "Updated successfuly" });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Place>> PostPlace(Dto.PlaceDetails details,
                                                     AttachImages attachImages,
                                                     ParseToPictureUploads parseToPictureUploads)
    {
        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is null) return NotFound("Bro doesn't exist.");

        Place place = new()
        {
            Name = details.Name,
            Description = details.Description,
            Information = details.Information,
            Address = details.Address,
            Mobile = details.Mobile,
            Landline = details.Landline,
            Email = details.Email,
            Website = details.Website,
            Facebook = details.Facebook,
            Instagram = details.Instagram,
            BroId = bro.Id,
        };

        try
        {
            await attachImages.ComputeAsync(
                place,
                await parseToPictureUploads.FromFormFilesAsync<PlacePicture>(
                    new UrlSegments(Request.Scheme, Request.Host.ToString()),
                    details.Picture,
                    details.Pictures,
                    ModelState));
        }
        catch (ImageTypeException)
        {
            return ValidationProblem(ModelState);
        }

        context.Place.Add(place);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPlace),
            new { place.Id },
            new { Message = "Submit successful", place.Id });
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
}