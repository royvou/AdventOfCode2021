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

    private static readonly Dictionary<char, int> Part1ScoreLookup = new()
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137,
    };


    private static readonly Dictionary<char, long> Part2ScoreLookup = new()
    {
        [')'] = 1l,
        [']'] = 2l,
        ['}'] = 3l,
        ['>'] = 4l,
    };


    private readonly string _input;

    private readonly char[][] _parsedInput;

    public Day_10()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = _input.SplitNewLine().Select(x => x.ToCharArray()).ToArray();
    }

    public override ValueTask<string> Solve_1()
        => new(_parsedInput.Select(CalculateScore).Sum().ToString());

    private int CalculateScore(char[] chars)
    {
        var stack = new Stack<char>();

        var popScore = (char @char) =>
        {
            var popped = stack.Pop();
            var expected = ClosingCharLookup[popped];

            if (@char != expected)
            {
                // 0 is invalid
                return Part1ScoreLookup.TryGetValue(@char, out var result) ? result : 0;
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

    public override ValueTask<string> Solve_2()
    {
        var incompleteLines = _parsedInput.Where(x => CalculateScore(x) == 0).Select(x => CalculateScore2(x)).OrderBy(x => x).ToList();

        return new ValueTask<string>(incompleteLines[incompleteLines.Count / 2].ToString());
    }

    private long CalculateScore2(char[] chars)
    {
        var stack = new Stack<char>();

        var popScore = (char @char) =>
        {
            var popped = stack.Pop();
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

        return stack.Select(x => ClosingCharLookup[x]).Aggregate(0l, (acc, curr) => 5 * acc + Part2ScoreLookup[curr]);
    }
}