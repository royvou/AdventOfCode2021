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
        // Assume its between  1/4 and 3/4th
        var oneFourth = _horizontalPositions.Length / 4;
        var min = _horizontalPositions[oneFourth];
        var max = _horizontalPositions[^oneFourth];

        return new ValueTask<string>(Enumerable.Range(min, max - min).Select(CalculateConstantScoreForIndex).OrderBy(x => x).First().ToString());
    }

    private int CalculateConstantScoreForIndex(int i)
        => _horizontalPositions.Select(pos => Math.Abs(pos - i)).Sum();

    public override ValueTask<string> Solve_2()
    {
        // Assume its between  1/4 and 3/4th
        var oneFourth = _horizontalPositions.Length / 4;
        var min = _horizontalPositions[oneFourth];
        var max = _horizontalPositions[^oneFourth];

        return new ValueTask<string>(Enumerable.Range(min, max - min).Select(CalculateIncrementalScoreForIndex).OrderBy(x => x).First().ToString());
    }

    private int CalculateIncrementalScoreForIndex(int i)
        => _horizontalPositions.Select(pos => CalculateIncrementalScore(Math.Abs(pos - i))).Sum();

    private int CalculateIncrementalScore(int abs)
        => abs * (abs + 1) / 2;
}