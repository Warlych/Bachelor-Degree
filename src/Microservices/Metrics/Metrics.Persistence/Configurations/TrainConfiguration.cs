using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.ValueObjects.Trains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metrics.Persistence.Configurations;

public sealed class TrainConfiguration : IEntityTypeConfiguration<Train>
{
    public void Configure(EntityTypeBuilder<Train> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<Guid>(x => x.Identity, x => new TrainId(x));
        builder.ComplexProperty(x => x.ExternalIdentifier);
    }
}
