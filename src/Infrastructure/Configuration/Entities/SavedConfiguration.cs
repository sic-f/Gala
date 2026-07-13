using Core.SavedAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration.Entities;

public class SavedConfiguration : IEntityTypeConfiguration<Saved>
{
    public void Configure(EntityTypeBuilder<Saved> builder)
    {
        builder.Property(s => s.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.HasIndex(s => s.SaveableId);
    }
}