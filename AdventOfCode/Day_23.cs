namespace AdventOfCode;

public class Day_23 : BaseDay
{
    private readonly string _input;

    public Day_23()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var A = 2 + 9 + 3 + 8;
        var B = 6 + 4;
        var C = 2 + 3 + 7;
        var D = 7 + 7;

        var score = A * 1 + B * 10 + C * 100 + D * 1000;
        return new ValueTask<string>(score.ToString());
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}