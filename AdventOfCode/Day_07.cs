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
    // Median
        => new(CalculateConstantScoreForIndex(_horizontalPositions[_horizontalPositions.Length / 2]).ToString());

    private int CalculateConstantScoreForIndex(int i)
        => _horizontalPositions.Select(pos => Math.Abs(pos - i)).Sum();

    public override ValueTask<string> Solve_2()
    {
       //Mean
       var mean = _horizontalPositions.Sum() / _horizontalPositions.Length;

       //   Try below/above value
       return new ValueTask<string>(Enumerable.Range(0,2).Select(x => CalculateIncrementalScoreForIndex(x + mean)).OrderBy(x =>  x).First().ToString());
    }

    private int CalculateIncrementalScoreForIndex(int i)
        => _horizontalPositions.Select(pos => CalculateIncrementalScore(Math.Abs(pos - i))).Sum();

    private int CalculateIncrementalScore(int abs)
        => abs * (abs + 1) / 2;
}