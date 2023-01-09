namespace InglemoorCodingComputing.Evaluator.Services;

/// <inheritdoc/>
public class ResultCheckingService : IResultCheckingService
{
    private static string Clean(string x)
    {
        var lines = x.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Where(x => !string.IsNullOrEmpty(x));
        return string.Join('\n', lines);
    }


    /// <inheritdoc/>
    public bool Verify(string expected, string generated)
    {
        if (expected == generated)
            return true;
        var x = Clean(expected);
        var y = Clean(generated);
        return x == y;
    }
}
