using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_17 : BaseDay
{
    private readonly string _input;
    private readonly ((int X, int Y) Start, (int X, int Y) End) _targetArea;

    public Day_17()
    {
        _input = File.ReadAllText(InputFilePath);

        var match = Regex.Match(_input, @"target area: x=([-\d]+)\.\.([-\d]+), y=([-\d]+)\.\.([-\d]+)");

        _targetArea = (Start: (X: match.Groups[1].Value.AsInt(), Y: match.Groups[3].Value.AsInt()), End: (X: match.Groups[2].Value.AsInt(), Y: match.Groups[4].Value.AsInt()));
    }

    public override ValueTask<string> Solve_1()
    {
        var startPoint = (0, 0);

        var maxHeight = -1;
        for (var y = 0; y < 1_000; y++)
        {
            for (var x = 1; x < _targetArea.End.X; x++)
            {
                var (currentMaxHeight, hit) = CalculateWithVelocity(startPoint, _targetArea, (x, y));

                if (hit && currentMaxHeight > maxHeight)
                {
                    maxHeight = currentMaxHeight;
                }
            }
        }

        return new ValueTask<string>(maxHeight.ToString());
    }

    private (int maxHeight, bool hit) CalculateWithVelocity((int, int) startPoint, ((int X, int Y) Start, (int X, int Y) End) targetArea, (int x, int y) velocity)
    {
        var (x, y) = startPoint;
        var (currentXVelocity, currentYVelocity) = velocity;

        var maxHeight = 0;
        while (x < targetArea.End.X && y > targetArea.End.Y
                                    && (currentXVelocity > 0 || x > targetArea.Start.X && x < targetArea.End.X))
        {
            (x, y) = (x + currentXVelocity, y + currentYVelocity);
            (currentXVelocity, currentYVelocity) = (currentXVelocity > 1 ? currentXVelocity - 1 : currentXVelocity, currentYVelocity - 1);
            maxHeight = Math.Max(maxHeight, y);

            if (IsBetween(targetArea, (x, y)))
            {
                return (maxHeight, true);
            }
        }

        return (-1, false);
    }

    private bool IsBetween(((int X, int Y) Start, (int X, int Y) End) targetArea, (int x, int y) position)
        => position.x >= targetArea.Start.X && position.x <= targetArea.End.X &&
           position.y >= targetArea.Start.Y && position.y <= targetArea.End.Y;

    public override ValueTask<string> Solve_2() => new(string.Empty);
}