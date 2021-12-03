namespace AdventOfCode;

public class Day_03 : BaseDay
{
    private readonly string _input;
    private readonly int[][] _parsedInput;

    public Day_03()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = _input.SplitNewLine().Select(x => x.Select(c => c.AsInt()).ToArray()).ToArray();
    }

    public override ValueTask<string> Solve_1() => new(Solve_1_Sync());

    private string Solve_1_Sync()
    {
        var result = Enumerable.Range(0, _parsedInput[0].Length).Aggregate(new Day03Record(0, 0), (acc, current) =>
        {
            var gamma = acc.Gamma;
            gamma <<= 1;
            var orderByDescending = _parsedInput.Select(x => x[current]).GroupBy(x => x).OrderByDescending(x => x.Count());
            gamma ^= orderByDescending.First().Key;
            var epsilon = acc.Epsilon;
            epsilon <<= 1;
            epsilon ^= orderByDescending.Skip(1).First().Key;

            return new Day03Record(gamma, epsilon);
        });
        return (result.Epsilon * result.Gamma).ToString();
    }


    public override ValueTask<string> Solve_2() => new(string.Empty);

    public record Day03Record(int Gamma, int Epsilon);
}