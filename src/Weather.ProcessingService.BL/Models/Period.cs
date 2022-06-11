namespace Weather.ProcessingService.BL.Models;

public record Period
{
    public DateTime From { get; init; }
    public DateTime To { get; init; }
}
