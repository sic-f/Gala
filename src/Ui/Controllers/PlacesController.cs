using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Configuration.Storage.Options;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static Core.ValueObjects.General;
using static Infrastructure.ValueObjects.ImageValues;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, UserManager<Bro> userManager) : ControllerBase
{
    [HttpGet]
    public IEnumerable<Core.Dto.PlaceDetailsHomePage> GetPlace(DateTimeOffset offset, int limit)
    //public IQueryable<Place> GetPlace(DateTimeOffset offset, int limit)
    {
        PlaceGeneralPictureDetails? picture = default;
        IEnumerable<PlaceGeneralPictureDetails> pictures = Enumerable.Empty<PlaceGeneralPictureDetails>();

        return context.Place.AsNoTracking()
                            .Include(p => p.ProfilePictures)
                            .Include(p => p.Pictures)
                            .Where(p => p.CreatedAt < offset)
                            .OrderByDescending(p => p.CreatedAt)
                            .Take(limit).ToList()
                            .Select<Core.PlaceAggregate.Place, PlaceDetailsHomePage>(p =>
                            {
                                if (p.Picture is not null)
                                {
                                    picture = new(Id: p.Picture.Id,
                                                  Caption: p.Picture.Caption,
                                                  Normal: p.Picture.Normal,
                                                  Small: p.Picture.Small,
                                                  PlaceId: p.Picture.PlaceId);
                                }

                                if (p.Pictures.Count != 0)
                                {
                                    pictures = p.Pictures.Select(pep =>
                                        new PlaceGeneralPictureDetails(Id: pep.Id,
                                                                       Caption: pep.Caption,
                                                                       Normal: pep.Normal,
                                                                       Small: pep.Small,
                                                                       PlaceId: pep.PlaceId)).ToList();
                                }
                                return new Core.Dto.PlaceDetailsHomePage(
                                                            Id: p.Id,
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
                                                            Pictures: pictures,
                                                            CreatedAt: p.CreatedAt);
                            });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Core.Dto.PlaceDetails>> GetPlace(Guid id)
    {
        Place? place = context.Place.AsNoTracking()
                                    .Include(p => p.ProfilePictures)
                                    .Include(p => p.Pictures)
                                    .FirstOrDefault(p => p.Id == id);

        if (place is null) return NotFound("❌🏤 Place does not exist.");

        PlaceGeneralPictureDetails? picture = default;
        IEnumerable<PlaceGeneralPictureDetails> pictures = Enumerable.Empty<PlaceGeneralPictureDetails>();

        if (place.Picture is not null)
        {
            picture = new(Id: place.Picture.Id,
                          Caption: place.Picture.Caption,
                          Normal: place.Picture.Normal,
                          Small: place.Picture.Small,
                          PlaceId: place.Picture.PlaceId);
        }

        if (place.Pictures.Count != 0)
        {
            pictures = place.Pictures.Select(p =>
                new PlaceGeneralPictureDetails(Id: p.Id,
                                               Caption: p.Caption,
                                               Normal: p.Normal,
                                               Small: p.Small,
                                               PlaceId: p.PlaceId)).ToList();
        }

        return new Core.Dto.PlaceDetails(Id: place.Id,
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
    public async Task<IActionResult> PutPlace(Guid id, Dto.PlaceDetails details)
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
    public async Task<ActionResult<Place>> PostPlace(Dto.PlaceDetails details,
                                                     AttachImages attachImages,
                                                     IOptionsSnapshot<PlaceImageOptions> options)
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

        IPathSwapElements paths = options.Get(PlaceImageOptions.Place);

        try
        {
            await attachImages.ComputeAsync(place,
                                            images,
                                            new GetImagesPath(paths),
                                            new UrlSegments(Request.Scheme,
                                                            Request.Host.ToString()));
        }
        catch (ImageTypeException ex)
        {
            return BadRequest(new { Errors = new { Picture = new string[] { ex.Message } } });
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