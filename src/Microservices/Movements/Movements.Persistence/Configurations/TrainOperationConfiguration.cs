using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movements.Domain.TrainOperations;
using Movements.Domain.TrainOperations.ValueObjects.TrainOperations;

namespace Movements.Persistence.Configurations;

public sealed class TrainOperationConfiguration : IEntityTypeConfiguration<TrainOperation>
{
    public void Configure(EntityTypeBuilder<TrainOperation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<Guid>(x => x.Identity, x => new TrainOperationId(x));
        builder.ComplexProperty(x => x.TrainIdentifier);
        builder.ComplexProperty(x => x.RailwaySectionFromIdentifier);
        builder.ComplexProperty(x => x.RailwaySectionToIdentifier);
    }
}
