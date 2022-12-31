namespace InglemoorCodingComputing.Evaluator.ExecutionRunner;

internal record PistonResult
{
    internal record StageResult
    {
        public required string Stdout { get; init; }
        public required string Stderr { get; init; }
        public required int Code { get; init; }
        public required string Output { get; init; }
    }

    public required string Language { get; init; }
    public required string Version { get; init; }
    public required StageResult Run { get; init; }
    public StageResult? Compile { get; init; }
}
