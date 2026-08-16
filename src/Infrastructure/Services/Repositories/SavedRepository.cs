using Core.Entities;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class SavedRepository(ApplicationDbContext context) : ISavedRepository
{
    public (IEnumerable<Place>,
        IEnumerable<Saved>,
        IEnumerable<Review>,
        IEnumerable<Saved>) FetchSaveds(DateTimeOffset offset, int limit, Guid broId)
    {
        IList<Saved> saveds = [.. context.Saved.AsNoTracking()
            .Where(saved => saved.CreatedAt < offset)
            .OrderByDescending(saved => saved.CreatedAt)
            .Where(saved => saved.SaverId == broId)
            .Take(limit)];

        IEnumerable<Saved> savedPlaces = saveds.Where(savedPlace => savedPlace.Discriminator == "SavedPlace");
        IEnumerable<Saved> savedReviews = saveds.Where(savedReview => savedReview.Discriminator == "SavedReview");

        IList<Guid> savedPlaceIds = [.. saveds.Where(saved => saved.Discriminator == "SavedPlace")
            .Select(sianca => sianca.SaveableId)];

        IList<Guid> savedReviewIds = [.. saveds.Where(saved => saved.Discriminator == "SavedReview")
            .Select(sianca => sianca.SaveableId)];

        IEnumerable<Place> places = context.Place.Include(p => p.ProfilePictures)
            .Where(p => savedPlaceIds.Contains(p.Id))
            .ToList()
            .OrderBy(p => savedPlaceIds.IndexOf(p.Id));

        IEnumerable<Review> reviews = context.Review
            .Where(p => savedReviewIds.Contains(p.Id))
            .ToList()
            .OrderBy(p => savedReviewIds.IndexOf(p.Id));

        return (places, savedPlaces, reviews, savedReviews);
    }

    public async Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace)
    {
        SavedPlace saved = context.SavedPlace.Add(savedPlace).Entity;
        await context.SaveChangesAsync();

        return saved;
    }

    public async Task<Guid?> GetSavedId(Guid saveableId, Guid broId)
    {
        return (await context.Saved.AsNoTracking().FirstOrDefaultAsync<Saved>(sp =>
            sp.SaveableId == saveableId && sp.SaverId == broId))?.Id;
    }

    public async Task<SavedReview> CreateSavedReviewAsync(SavedReview savedReview)
    {
        SavedReview saved = context.SavedReview.Add(savedReview).Entity;
        await context.SaveChangesAsync();

        return saved;
    }

    public async Task<AccountAvatar?> GetReviewerAvatarAsync(Guid reviewId)
    {
        Review review = await context.Review.AsNoTracking()
            .FirstAsync<Review>(review => review.Id == reviewId);

        return await context.AccountAvatar.FirstOrDefaultAsync(avatar => avatar.BroId == review.ReviewerId);
    }
}
