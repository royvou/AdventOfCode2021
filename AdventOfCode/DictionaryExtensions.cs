namespace AdventOfCode;

public static class DictionaryExtensions
{
    public static IDictionary<TKey, int> GroupByCount<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var result = new Dictionary<TKey, int>();

        foreach (var current in source.Select(keySelector))
        {
            result[current] = result.GetValueOrDefault(current) + 1;
        }

        return result;
    }
}