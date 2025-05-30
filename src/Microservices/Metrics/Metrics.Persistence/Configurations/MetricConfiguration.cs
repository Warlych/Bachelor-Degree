using Metrics.Domain.Metrics;
using Metrics.Domain.Metrics.Entities;
using Metrics.Domain.Metrics.ValueObjects.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metrics.Persistence.Configurations;

public sealed class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<Guid>(x => x.Identity, x => new MetricId(x));
        builder.ComplexProperty(x => x.DateRange);
        builder.ComplexProperty(x => x.Metrics);

        builder.HasMany<Train>("_trains")
               .WithMany()
               .UsingEntity(x => x.ToTable("metric_trains"));

        builder.Ignore(m => m.Trains);

        builder.HasOne(x => x.From)
               .WithMany()
               .HasForeignKey("MetricId");

        builder.HasOne(x => x.To)
               .WithMany()
               .HasForeignKey("MetricId");
    }
}
