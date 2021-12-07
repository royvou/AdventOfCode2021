namespace AdventOfCode;

public class Day_07 : BaseDay
{
    private readonly int[] _horizontalPositions;
    private readonly string _input;

    public Day_07()
    {
        _input = File.ReadAllText(InputFilePath);

        _horizontalPositions = _input.Split(",").AsInt().OrderBy(x => x).ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        var min = _horizontalPositions[0];
        var max = _horizontalPositions[^1];

        return new ValueTask<string>(Enumerable.Range(min, max - min).Select(CalculateScoreForIndex).OrderBy(x => x).First().ToString());
    }

    private int CalculateScoreForIndex(int i)
        => _horizontalPositions.Select(pos => Math.Abs(pos - i)).Sum();

    public override ValueTask<string> Solve_2() => new(string.Empty);
}