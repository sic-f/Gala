using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Dto;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, UserManager<Bro> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Place>>> GetPlace()
    {
        return await context.Place.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Place>> GetPlace(Guid id)
    {
        Place? place = context.Place.AsNoTracking().FirstOrDefault(p => p.Id == id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        return place;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlace(Guid id, IPlaceDetailer details)
    {
        Place? place = context.Place.Find(id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

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

        return NoContent();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Place>> PostPlace(PlaceDetails details)
    {
        Bro? bro = await userManager.GetUserAsync(User);

        if (bro is null)
        {
            return NotFound("Bro doesn't exist.");
        }

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
            LastModified = DateTimeOffset.UtcNow,
        };

        context.Place.Add(place);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPlace),
            new { place.Id },
            new { Message = "Submit successful", place });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlace(Guid id)
    {
        var place = await context.Place.FindAsync(id);
        if (place == null)
        {
            return NotFound();
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