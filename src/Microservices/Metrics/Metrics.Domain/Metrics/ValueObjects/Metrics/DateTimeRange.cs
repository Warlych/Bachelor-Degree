namespace Metrics.Domain.Metrics.ValueObjects.Metrics;

public readonly record struct DateTimeRange
{
    public DateTime From { get; init; }
    public DateTime To { get; init; }

    public DateTimeRange(DateTime from, DateTime to)
    {
        if (from > to)
        {
            throw new InvalidOperationException("From cannot be greater than to");
        }
        
        From = from;
        To = to;
    }
}
