namespace Weather.ProcessingService.BL.Models.Dtos;

public record PeriodDto
{
    public DateTime From { get; init; }
    public DateTime To { get; init; }
}
