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
            return new Line(ParsePoint(splittedLine[0]), ParsePoint(splittedLine[1]));
        }).ToArray();
    }

    public Point ParsePoint(string point)
    {
        var loc = point.Split(",");
        return new Point(loc[0].AsInt(), loc[1].AsInt());
    }

    public override ValueTask<string> Solve_1()
        => new(SolvePuzzle(_lines.Where(line => IsHorizontal(line) || IsVertical(line))).ToString());

    public override ValueTask<string> Solve_2()
        => new(SolvePuzzle(_lines).ToString());

    private int SolvePuzzle(IEnumerable<Line> validLines)
        => validLines.SelectMany(static line => GetPointsForLine(line)).GroupByCount(x => x).Values.Count(x => x > 1);

    private static IEnumerable<Point> GetPointsForLine(Line line)
    {
        var xDirection = line.Start.X == line.End.X ? 0 : line.End.X > line.Start.X ? 1 : -1;
        var yDirection = line.Start.Y == line.End.Y ? 0 : line.End.Y > line.Start.Y ? 1 : -1;
        var distance = Math.Max(Math.Abs(line.Start.X - line.End.X), Math.Abs(line.Start.Y - line.End.Y)) + 1;

        for (var e = 0; e < distance; e++)
        {
            yield return new Point(line.Start.X + e * xDirection, line.Start.Y + e * yDirection);
        }
        //return Enumerable.Range(0, distance).Select(e => new Point(line.Start.X + e * xDirection, line.Start.Y + e * yDirection));
    }

    private static bool IsHorizontal(Line line)
        => line.Start.X == line.End.X;

    private static bool IsVertical(Line line)
        => line.Start.Y == line.End.Y;

    public struct Line
    {
        public Point Start;
        public Point End;

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }
}