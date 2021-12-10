namespace AdventOfCode;

public class Day_10 : BaseDay
{
    private static readonly Dictionary<char, char> ClosingCharLookup = new()
    {
        ['('] = ')',
        ['['] = ']',
        ['{'] = '}',
        ['<'] = '>',
    };

    private static readonly Dictionary<char, int> ScoreLookup = new()
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137,
    };


    private readonly string _input;

    private readonly char[][] _parsedInput;

    public Day_10()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = _input.SplitNewLine().Select(x => x.ToCharArray()).ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        return new ValueTask<string>(_parsedInput.Select(CalculateScore).Sum().ToString());
    }

    private int CalculateScore(char[] chars)
    {
        var stack = new Stack<char>();

        var popScore = (char @char) =>
        {
            var popped = stack.Pop();
            var expected = ClosingCharLookup[popped];

            if (@char != expected)
            {
                return ScoreLookup.TryGetValue(@char, out var result) ? result : 0;
            }

            return -1;
        };

        var pushScore = (char @char) =>
        {
            stack.Push(@char);
            return -1;
        };

        foreach (var @char in chars)
        {
            var score = @char switch
            {
                '(' => pushScore('('),
                '[' => pushScore('['),
                '{' => pushScore('{'),
                '<' => pushScore('<'),
                ')' => popScore(')'),
                ']' => popScore(']'),
                '}' => popScore('}'),
                '>' => popScore('>'),
                _ => -1,
            };

            if (score >= 0)
            {
                return score;
            }
        }

        return 0;
    }

    private bool IsInvalid(char[] chars)
        => chars.Select(x => ClosingCharLookup.TryGetValue(x, out var inverseX) ? inverseX : x).GroupBy(x => x).Count(x => x.Count() % 2 != 0) == 2;

    public override ValueTask<string> Solve_2() => new(string.Empty);
}