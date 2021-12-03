namespace AdventOfCode;

public class Day_03 : BaseDay
{
    private readonly string _input;
    private readonly int[][] _parsedInputPart1;
    private readonly int[] _parsedInputPart2;

    public Day_03()
    {
        _input = File.ReadAllText(InputFilePath);
        _parsedInputPart1 = _input.SplitNewLine().Select(x => x.Select(c => c.AsInt()).ToArray()).ToArray();

        _parsedInputPart2 = _input.SplitNewLine().Select(x => Convert.ToInt32(x, 2)).ToArray();
    }

    public override ValueTask<string> Solve_1() => new(Solve_1_Sync());

    private string Solve_1_Sync()
    {
        var result = Enumerable.Range(0, _parsedInputPart1[0].Length).Aggregate(new Day03Part1Record(0, 0), (acc, current) =>
        {
            var gamma = acc.Gamma;
            gamma <<= 1;
            var orderByDescending = _parsedInputPart1.Select(x => x[current]).GroupBy(x => x).OrderByDescending(x => x.Count()).ToList();
            gamma ^= orderByDescending[0].Key;
            var epsilon = acc.Epsilon;
            epsilon <<= 1;
            epsilon ^= orderByDescending[1].Key;

            return new Day03Part1Record(gamma, epsilon);
        });
        return (result.Epsilon * result.Gamma).ToString();
    }


    public override ValueTask<string> Solve_2() => new(Solve_2_Sync());

    private string Solve_2_Sync()
        => (CalculateRating(tuple => tuple.one >= tuple.zero) * CalculateRating(tuple => tuple.one < tuple.zero)).ToString();

    private int CalculateRating(Func<(int one, int zero), bool> check)
    {
        var bitLength = _parsedInputPart1[0].Length;

        var validLines = new List<int>(_parsedInputPart2);
        for (var i = bitLength - 1; i >= 0; i--)
        {
            var numCount = CountNum(validLines, i);
            if (check(numCount))
            {
                validLines.RemoveAll(line => (line & (1 << i)) == 0);
            }
            else
            {
                validLines.RemoveAll(line => (line & (1 << i)) != 0);
            }

            if (validLines.Count == 1)
            {
                return validLines[0];
            }
        }

        return -1;
    }

    private (int One, int Zero) CountNum(List<int> validLines, int i)
        => validLines.Aggregate((One: 0, Zero: 0), (acc, curr) =>
            (curr & (1 << i)) != 0
                ? (acc.One += 1, acc.Zero)
                : (acc.One, acc.Zero += 1));

    public record Day03Part1Record(int Gamma, int Epsilon);

    public record Day03Part2Record(int Gamma, int Epsilon);
}