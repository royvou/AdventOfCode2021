namespace AdventOfCode;

public static class StringExtensions
{
    public static int AsInt(this string input)
        => int.Parse(input);

    public static long AsLong(this string input)
        => long.Parse(input);
    
    public static int AsInt(this char input)
        => (int) char.GetNumericValue(input);


    public static IEnumerable<int> AsInt(this IEnumerable<string> input)
        => input.Select(x => int.Parse(x));
    
    public static IEnumerable<long> AsLong(this IEnumerable<string> input)
        => input.Select(x => x.AsLong());

    public static IEnumerable<int> ParseAsArray(this string input)
        => input.SplitNewLine().AsInt();

    public static IEnumerable<long> ParseAsLongArray(this string input)
        => input.SplitNewLine().Select(x => long.Parse(x));

    public static string[] SplitSpace(this string input)
        => input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    public static string[] SplitNewLine(this string input)
        => input.Split(new[]
        {
            Environment.NewLine,
            "\n" //Used as newline in unit tests
        }, StringSplitOptions.RemoveEmptyEntries);

    private static readonly string doubleNewLine = Environment.NewLine + Environment.NewLine;
    private static readonly string doubleNewLineCommand = "\n\n";

    public static string[] SplitDoubleNewLine(this string input)
        => input.Split(new[] {doubleNewLine, doubleNewLineCommand}, StringSplitOptions.RemoveEmptyEntries);
}