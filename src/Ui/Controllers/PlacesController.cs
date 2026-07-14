using Context;
using Core.Dto;
using Core.Exceptions;
using Core.PlaceAggregate;
using Core.Values;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.Services;
using Ui.Services.Pictures;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlacesController(ApplicationDbContext context, Contribution contribution) : ControllerBase
{
    private Place Place { get; set; } = default!;

    [HttpGet]
    public async Task<IEnumerable<PlaceDetailsHomePage>> GetPlaceAsync(DateTimeOffset offset, int limit, Guid broId)
    {
        return await contribution.ShowPlaceInHomePageAsync(offset, limit, broId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlaceDetails>> GetPlace(Guid id, Guid broId)
    {
        return await contribution.DisplayPlaceAsync(id, broId);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlace(Guid id,
                                              Dto.PlaceDetails details,
                                              ParseToPictureUploads parseToPictureUploads)
    {
        try
        {
            await ModifyPlaceAsync(details, parseToPictureUploads, id, this.BroId(User));
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
                    Instance = $"PUT api/Places/${id}",
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
    public async Task<IActionResult> PostPlace(Dto.PlaceDetails details, ParseToPictureUploads parseToPictureUploads)
    {
        try
        {
            Place = await SubmitPlaceAsync(details, parseToPictureUploads);
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
    public async Task<IActionResult> DeletePlace(Guid id)
    {
        try
        {
            await contribution.DeleteAsync(id, this.BroId(User));
        }
        catch (UnauthorizedContributionAccessException ex)
        {
            return Problem(ex.Message,
                $"DELETE api/Places/{id}",
                403,
                "Unauthorized delete.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!PlaceExists(id))
            {
                return NotFound(new ProblemDetails
                {
                    Detail = ex.Message,
                    Instance = $"DELETE api/Places/${id}",
                    Title = "Db Update Concurrency Exception"
                });
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
                                               ParseToPictureUploads pictureUploadsParser)
    {
        var placePictureUploads = await PlacePictureUploadsAsync(pictureUploadsParser, details);

        return await contribution.SubmitAsync(details, placePictureUploads, this.BroId(User));
    }

    private async Task ModifyPlaceAsync(Dto.PlaceDetails details,
                                        ParseToPictureUploads pictureUploadsParser,
                                        Guid placeId,
                                        Guid broId)
    {
        var pictureUploads = await PlacePictureUploadsAsync(pictureUploadsParser, details);

        await contribution.ModifyAsync(details, pictureUploads, placeId, broId);
    }

    private async Task<PictureUploads> PlacePictureUploadsAsync(ParseToPictureUploads parseToPictureUploads, Dto.PlaceDetails details)
    {
        return await parseToPictureUploads.FromFormFilesAsync<Core.PlaceAggregate.PlacePicture>(
            new UrlSegments(Request.Scheme, Request.Host.ToString()),
            details.Picture,
            details.Pictures,
            ModelState);
    }
}