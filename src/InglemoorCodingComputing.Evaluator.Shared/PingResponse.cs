namespace InglemoorCodingComputing.Evaluator.Shared;

public record PingResponse
{
    public required Guid Instance { get; init; }
    public required DateTime Date { get; init; }
    public required string Spec { get; init; }
}
