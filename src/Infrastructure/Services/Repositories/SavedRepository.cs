using Core.Entities;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.SavedAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class SavedRepository(ApplicationDbContext context) : ISavedRepository
{
    public IOrderedEnumerable<Core.Dto.SavedDto> FetchSaveds(DateTimeOffset offset, int limit, Guid broId)
    {
        IQueryable<Saved> saveds = context.Saved.AsNoTracking()
            .Where(saved => saved.CreatedAt < offset)
            .OrderByDescending(saved => saved.CreatedAt)
            .Where(saved => saved.SaverId == broId)
            .Take(limit);

        List<Core.Dto.SavedDto> savedDtos = [];

        savedDtos.AddRange(saveds.AsEnumerable().Join(
            context.Place,
            saved => saved.SaveableId,
            place => place.Id,
            (saved, place) => new Core.Dto.SavedDto
            {
                SaveableId = place.Id,
                Headline = place.Name,
                Picture = place.Picture?.Normal,
                Kind = saved.Discriminator,
                CreatedAt = saved.CreatedAt
            }));

        savedDtos.AddRange(
            saveds.AsEnumerable().Join(
                context.Review.AsEnumerable().LeftJoin(
                    context.AccountAvatar,
                    review => review.ReviewerId,
                    avatar => avatar.BroId,
                    (review, avatar) => new
                    {
                        review.Id,
                        Headline = review.Content,
                        Picture = avatar?.Normal,
                    }),
                saved => saved.SaveableId,
                review => review.Id,
                (saved, review) => new Core.Dto.SavedDto
                {
                    SaveableId = review.Id,
                    Headline = review.Headline,
                    Picture = review.Picture,
                    Kind = saved.Discriminator,
                    CreatedAt = saved.CreatedAt
                }));

        return savedDtos.OrderByDescending(savedDto => savedDto.CreatedAt);
    }

    public IEnumerable<Core.Dto.SavedDto> GetSavedPlaces(DateTimeOffset offset, int limit, Guid broId)
    {
        return Saveds<SavedPlace>(offset, limit, broId).AsEnumerable().Join(
            context.Place.Include(place => place.ProfilePictures),
            savedPlace => savedPlace.SaveableId,
            place => place.Id,
            (savedPlace, place) => new Core.Dto.SavedDto
            {
                SaveableId = place.Id,
                Headline = place.Name,
                Picture = place.Picture?.Normal,
                Kind = savedPlace.Discriminator,
                CreatedAt = savedPlace.CreatedAt
            });
    }

    public IEnumerable<Core.Dto.SavedDto> GetSavedReviews(DateTimeOffset offset, int limit, Guid broId)
    {
        return Saveds<SavedReview>(offset, limit, broId).AsEnumerable().Join(
            context.Review.AsEnumerable().LeftJoin(
                context.AccountAvatar,
                review => review.ReviewerId,
                avatar => avatar.BroId,
                (review, avatar) => new
                {
                    review.Id,
                    Headline = review.Content,
                    Picture = avatar?.Normal,
                }),
            savedReview => savedReview.SaveableId,
            review => review.Id,
            (savedReview, review) => new Core.Dto.SavedDto
            {
                SaveableId = review.Id,
                Headline = review.Headline,
                Picture = review.Picture,
                Kind = savedReview.Discriminator,
                CreatedAt = savedReview.CreatedAt,
            });
    }

    public async Task<Saved> FetchSavedAsync(Guid savedId)
    {
        return await context.Saved.FirstAsync<Saved>(saved => saved.Id == savedId);
    }

    public async Task<SavedPlace> CreateSavedPlaceAsync(SavedPlace savedPlace)
    {
        SavedPlace saved = context.SavedPlace.Add(savedPlace).Entity;
        await context.SaveChangesAsync();

        return saved;
    }

    public async Task<Guid?> GetSavedIdAsync(Guid saveableId, Guid broId)
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

    public async Task DeleteSavedAsync(Saved saved)
    {
        context.Saved.Remove(saved);
        await context.SaveChangesAsync();
    }

    private IQueryable<T> Saveds<T>(DateTimeOffset offset, int limit, Guid broId) where T : Saved
    {
        return context.Set<T>().AsNoTracking()
            .Where(saved => saved.SaverId == broId)
            .Where(saved => saved.CreatedAt < offset)
            .OrderByDescending(saved => saved.CreatedAt)
            .Take(limit);
    }
}
