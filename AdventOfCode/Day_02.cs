namespace AdventOfCode;

public class Day_02 : BaseDay
{
    public enum Day02Direction
    {
        forward,
        down,
        up,
    }

    private readonly string _input;
    private readonly Day02Data[] _inputData;

    public Day_02()
    {
        _input = File.ReadAllText(InputFilePath);

        _inputData = _input.SplitNewLine().Select(line =>
        {
            var data = line.Split(' ');
            return new Day02Data(Enum.Parse<Day02Direction>(data[0]), int.Parse(data[1]));
        }).ToArray();
    }

    public override ValueTask<string> Solve_1() => new(Solve_1_Sync());

    private string Solve_1_Sync()
    {
        var (x, y, aim) = _inputData.Aggregate(new Day02Position(0, 0, 0), (position, current) =>
            current.Direction switch
            {
                Day02Direction.down => position with { Y = position.Y + current.Amount, },
                Day02Direction.up => position with { Y = position.Y - current.Amount, },
                Day02Direction.forward => position with { X = position.X + current.Amount, },
            });
        return (x * y).ToString();
    }

    public override ValueTask<string> Solve_2() => new(Solve_2_Sync());

    private string Solve_2_Sync()
    {
        var (x, y, aim) = _inputData.Aggregate(new Day02Position(0, 0, 0), (position, current) =>
            current.Direction switch
            {
                Day02Direction.down => position with { Aim = position.Aim + current.Amount, },
                Day02Direction.up => position with { Aim = position.Aim - current.Amount, },
                Day02Direction.forward => position with { X = position.X + current.Amount, Y = position.Y + position.Aim * current.Amount, },
            });

        return (x * y).ToString();
    }

    public record Day02Data(Day02Direction Direction, int Amount)
    {
    }

    public record Day02Position(long X, long Y, long Aim)
    {
    }
}