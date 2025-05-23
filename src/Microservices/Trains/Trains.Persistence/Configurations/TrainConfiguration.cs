using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trains.Domain.Trains;
using Trains.Domain.Trains.ValueObjects.Trains;

namespace Trains.Persistence.Configurations;

public sealed class TrainConfiguration : IEntityTypeConfiguration<Train>
{
    public void Configure(EntityTypeBuilder<Train> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<Guid>(x => x.Identity, x => new TrainId(x));
        builder.ComplexProperty(x => x.ExternalIdentifier);
        builder.ComplexProperty(x => x.Parameters);
    }
}
