namespace Weather.ProcessingService.Options;

public record AggregatorSettings
{
    public int WaitingTime { get; init; }
    public int AggregationPeriod { get; init; }
}
