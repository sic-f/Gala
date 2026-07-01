using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Dto;
using static Core.ValueObjects.General;
using static Infrastructure.ValueObjects.ImageValues;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, UserManager<Bro> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Place>>> GetPlace(DateTimeOffset offset, int limit)
    {
        return await context.Place.Where(p => p.CreatedAt < offset)
                                  .OrderByDescending(p => p.CreatedAt)
                                  .Take(limit)
                                  .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Place>> GetPlace(Guid id)
    {
        Place? place = context.Place.AsNoTracking().FirstOrDefault(p => p.Id == id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        return place;
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlace(Guid id, PlaceDetails details)
    {
        Place? place = context.Place.Find(id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is not null && place.BroId != bro.Id)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "❌👦🏣 Bro not allowed to update other people's places.");
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

        return Ok(new { Message = "Updated successfuly " });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Place>> PostPlace(PlaceDetails details,
                                                     AttachImages attachImages,
                                                     GetImagesPath getImagesPath)
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

        Images images = new(details.Picture, details.Pictures);

        await attachImages.ComputeAsync(place,
                                        images,
                                        getImagesPath,
                                        new UrlSegments(Request.Scheme,
                                                        Request.Host.ToString()));

        context.Place.Add(place);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPlace),
            new { place.Id },
            new { Message = "Submit successful", place });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlace(Guid id)
    {
        var place = await context.Place.FindAsync(id);
        if (place == null)
        {
            return NotFound("❌🏤 Place does not exist.");
        }

        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is not null && bro.Id != place.BroId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "❌👦🏣 Bro not allowed to delete other people's places");
        }

        context.Place.Remove(place);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool PlaceExists(Guid id)
    {
        return context.Place.Any(e => e.Id == id);
    }
}