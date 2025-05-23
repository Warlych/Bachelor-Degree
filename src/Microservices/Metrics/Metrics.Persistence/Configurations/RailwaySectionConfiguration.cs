using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.ValueObjects.RailwaySections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metrics.Persistence.Configurations;

public sealed class RailwaySectionConfiguration : IEntityTypeConfiguration<RailwaySection>
{
    public void Configure(EntityTypeBuilder<RailwaySection> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<Guid>(x => x.Identity, x => new RailwaySectionId(x));
        builder.ComplexProperty(x => x.ExternalIdentifier);
    }
}
