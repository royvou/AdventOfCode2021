namespace AdventOfCode;

public abstract class Day_00 : BaseDay
{
    private readonly string _input;

    public Day_00()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new(string.Empty);

    public override ValueTask<string> Solve_2() => new(string.Empty);
}