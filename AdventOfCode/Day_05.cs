using System.Drawing;

namespace AdventOfCode;

public class Day_05 : BaseDay
{
    private readonly string _input;
    private readonly Line[] _lines;

    public Day_05()
    {
        _input = File.ReadAllText(InputFilePath);

        _lines = _input.SplitNewLine().Select(_line =>
        {
            var splittedLine = _line.Split("->");
            return new Line
            {
                Start = ParsePoint(splittedLine[0]),
                End = ParsePoint(splittedLine[1]),
            };
        }).ToArray();
    }

    public Point ParsePoint(string point)
    {
        var loc = point.Split(",");
        return new Point(loc[0].AsInt(), loc[1].AsInt());
    }

    public override ValueTask<string> Solve_1()
    {
        var map = new Dictionary<Point, int>();

        var addLocationToMap = (Point p) => { map[p] = map.TryGetValue(p, out var result) ? result + 1 : 1; };

        var addLocationsToMap = (IEnumerable<Point> points) =>
        {
            foreach (var point in points)
            {
                addLocationToMap(point);
            }
        };

        foreach (var location in _lines)
        {
            if (IsHorizontal(location))
            {
                var points = location.Start.Y < location.End.Y
                    ? Enumerable.Range(location.Start.Y, location.End.Y - location.Start.Y + 1).Select(y => new Point(location.Start.X, y))
                    : Enumerable.Range(location.End.Y, location.Start.Y - location.End.Y + 1).Select(y => new Point(location.Start.X, y));

                addLocationsToMap(points);
            }

            else if (IsVertical(location))
            {
                var points = location.Start.X < location.End.X
                    ? Enumerable.Range(location.Start.X, location.End.X - location.Start.X + 1).Select(x => new Point(x, location.Start.Y))
                    : Enumerable.Range(location.End.X, location.Start.X - location.End.X + 1).Select(x => new Point(x, location.Start.Y));

                addLocationsToMap(points);
            }
        }

        return new ValueTask<string>(map.Values.Where(count => count > 1).Count().ToString());
    }

    private bool IsHorizontal(Line line)
        => line.Start.X == line.End.X;

    private bool IsVertical(Line line)
        => line.Start.Y == line.End.Y;


    public override ValueTask<string> Solve_2() => new(string.Empty);

    public struct Line
    {
        public Point Start;
        public Point End;
    }
}