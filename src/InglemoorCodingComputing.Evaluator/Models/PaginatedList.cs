namespace InglemoorCodingComputing.Evaluator.Models;

using System.Collections;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Paginated List
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class PaginatedList<T> : IReadOnlyList<T>
{
    private readonly IReadOnlyList<T> backing;
    /// <summary>
    /// Current Index.
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// Total Number of pages.
    /// </summary>
    public int TotalPages { get; private set; }

    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        backing = items;
    }

    /// <summary>
    /// Whether there are previous pages.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Whether there are more pages.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <inheritdoc />
    public int Count => backing.Count;

    /// <inheritdoc />
    public T this[int index] => backing[index];

    /// <summary>
    /// Creates a new paginated list form a data source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => backing.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)backing).GetEnumerator();
}