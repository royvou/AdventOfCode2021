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

        _targetArea = (Start: (X: match.Groups[1].Value.AsInt(), Y: match.Groups[4].Value.AsInt()), End: (X: match.Groups[2].Value.AsInt(), Y: match.Groups[3].Value.AsInt()));

        var minX = _targetArea.Start.X;
        var minY = _targetArea.End.Y;
        var maxX = _targetArea.End.X;
        var maXY = _targetArea.Start.Y;
    }

    public override ValueTask<string> Solve_1()
    {
        var (maxHeight, _) = Simulate();
        return new ValueTask<string>(maxHeight.ToString());
    }


    public override ValueTask<string> Solve_2()
    {
        var (_, count) = Simulate();
        return new ValueTask<string>(count.ToString());
    }

    private (int MaxHeight, int Count) Simulate()
    {
        int maxHeight = 0, count = 0;
        for (var vx = 0; vx <= _targetArea.End.X; vx++)
        {
            for (var vy = -5_000; vy < 5_000; vy++)
            {
                var (possibleMaxHeight, hit) = CalculateWithVelocity((0, 0), _targetArea, (vx, vy));
                if (!hit)
                {
                    continue;
                }

                maxHeight = Math.Max(possibleMaxHeight, maxHeight);
                count++;
            }
        }

        return (maxHeight, count);
    }

    private static (int maxHeight, bool hit) CalculateWithVelocity((int, int) startPoint, ((int X, int Y) Start, (int X, int Y) End) targetArea, (int x, int y) velocity)
    {
        var (x, y) = startPoint;
        var (currentXVelocity, currentYVelocity) = velocity;

        var maxHeight = 0;
        for (var i = 0; i < 1000; i++)
        {
            if (currentXVelocity == 0 &&  x < targetArea.Start.X)
            {
                // Left side of targetArea but no X velocity
                break;
            }
            
            if (x > targetArea.End.X)
            {
                // right side of targetArea
                break;
            }

            if (y < targetArea.End.Y)
            {
                // Below lowestp point
                break;
            }

            if (IsBetween(targetArea, (x, y)))
            {
                return (maxHeight, true);
            }

            (x, y) = (x + currentXVelocity, y + currentYVelocity);
            (currentXVelocity, currentYVelocity) = (currentXVelocity >= 1 ? currentXVelocity - 1 : currentXVelocity, currentYVelocity - 1);
            maxHeight = Math.Max(maxHeight, y);
        }

        return (-1, false);
    }

    private static bool IsBetween(((int X, int Y) Start, (int X, int Y) End) targetArea, (int x, int y) position)
        => position.x >= targetArea.Start.X && position.x <= targetArea.End.X &&
           position.y >= targetArea.End.Y && position.y <= targetArea.Start.Y;
}