namespace InglemoorCodingComputing.Evaluator.Services;

/// <inheritdoc/>
public class ResultCheckingService : IResultCheckingService
{
    private static string Clean(string x)
    {
        var lines = x.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        if (lines.Length is 0)
            return string.Empty;

        var start = 0;
        for (; start < lines.Length; start++)
        {
            if (!string.IsNullOrEmpty(lines[start]))
                break;
        }
        var end = lines.Length - 1;
        for (; end > -1; end--)
        {
            if (!string.IsNullOrEmpty(lines[end]))
                break;
        }
        return string.Join('\n', lines[start..(end + 1)]);
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
