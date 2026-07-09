using Microsoft.AspNetCore.Http;

namespace Infrastructure.Values;

public record Images(IFormFile? Picture, IFormFileCollection? Pictures);

