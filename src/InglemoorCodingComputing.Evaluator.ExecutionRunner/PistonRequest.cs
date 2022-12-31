namespace InglemoorCodingComputing.Evaluator.ExecutionRunner;

internal record PistonRequest
{
    internal record File
    {
        public required string Content { get; init; }
    }

    public required string Language { get; init; }
    public required string Version { get; init; }
    public required IReadOnlyList<File> Files { get; init; }
    public required string Stdin { get; init; }
}