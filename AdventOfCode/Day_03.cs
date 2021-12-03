namespace AdventOfCode;

public class Day_03 : BaseDay
{
    private readonly string _input;
    private readonly int[] _parsedInputPart2;
    private readonly int bitLength;

    public Day_03()
    {
        _input = File.ReadAllText(InputFilePath);
        bitLength = _input.SplitNewLine()[0].Length;

        _parsedInputPart2 = _input.SplitNewLine().Select(x => Convert.ToInt32(x, 2)).ToArray();
    }

    public override ValueTask<string> Solve_1() => new(Solve_1_Sync());

    private string Solve_1_Sync()
    {
        var result = Enumerable.Range(0, bitLength).Reverse().Aggregate(new Day03Part1Record(0, 0), (acc, current) =>
        {
            var (one, zero) = CountNum(_parsedInputPart2, current);

            return new Day03Part1Record((acc.Gamma << 1) ^ (one > zero ? 1 : 0), (acc.Epsilon << 1) ^ (one > zero ? 0 : 1));
        });
        return (result.Epsilon * result.Gamma).ToString();
    }


    public override ValueTask<string> Solve_2() => new(Solve_2_Sync());

    private string Solve_2_Sync()
        => (CalculateRating(tuple => tuple.one >= tuple.zero) * CalculateRating(tuple => tuple.one < tuple.zero)).ToString();

    private int CalculateRating(Func<(int one, int zero), bool> check)
    {
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

    private (int One, int Zero) CountNum(IEnumerable<int> validLines, int i)
        => validLines.Aggregate((One: 0, Zero: 0), (acc, curr) =>
            (curr & (1 << i)) != 0
                ? (acc.One += 1, acc.Zero)
                : (acc.One, acc.Zero += 1));

    public record Day03Part1Record(int Gamma, int Epsilon);
}