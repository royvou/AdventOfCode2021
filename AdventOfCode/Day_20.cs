using System.Text;

namespace AdventOfCode;

public class Day_20 : BaseDay
{
    private readonly bool[] _imageEnhancementAlgorithm;
    private readonly Dictionary<(int X, int Y), bool> _initialMap;
    private readonly string _input;

    public Day_20()
    {
        //_input = File.ReadAllText("./Inputs/20tests.txt");
        _input = File.ReadAllText(InputFilePath);

        var lines = _input.SplitNewLine();

        _imageEnhancementAlgorithm = lines[0].Select(x => x == '#').ToArray();

        _initialMap = lines[1..].SelectMany((line, y) => line.Select((@char, x) => (x, y, @char))).ToDictionary(x => (x.x, x.y), x => x.@char == '#');
    }

    public override ValueTask<string> Solve_1()
    {
        var currentMap = _initialMap;

        var checkSize = 1;
        var minPosition = (X: 0, Y: 0);
        var maxPosition = (X: currentMap.Max(x => x.Key.X), Y: currentMap.Max(x => x.Key.Y));
        for (var i = 1; i <= 2; i++)
        {
            currentMap = NextMap(currentMap, (minPosition.X - checkSize * i, minPosition.Y - checkSize * i), (maxPosition.X + checkSize * i, maxPosition.Y + checkSize * i), i % 2 == 0);
        }

        return new ValueTask<string>(currentMap.Values.Count(x => x).ToString());
    }

    private Dictionary<(int X, int Y), bool> NextMap(Dictionary<(int X, int Y), bool> previousMap, (int X, int Y) min, (int X, int Y) max, bool isCheckedDefault)
    {
        var newMap = new Dictionary<(int X, int Y), bool>();

        for (var y = min.Y; y <= max.Y; y++)
        {
            for (var x = min.X; x <= max.X; x++)
            {
                var number = GetNumberFromNeighbours(previousMap, (x, y), isCheckedDefault);

                newMap[(x, y)] = _imageEnhancementAlgorithm[number];
            }
        }

        return newMap;
    }

    private int GetNumberFromNeighbours(Dictionary<(int X, int Y), bool> map, (int x, int y) position, bool isCheckedDefault)
    {
        var result = 0;
        foreach (var neighbour in GetNeighbours(position.x, position.y))
        {
            result <<= 1;
            if (map.TryGetValue(neighbour, out var @checked))
            {
                result |= @checked ? 1 : 0;
                continue;
            }

            if (isCheckedDefault)
            {
                result |= 1;
            }
        }

        return result;
    }

    public static IEnumerable<(int X, int Y)> GetNeighbours(int x, int y)
    {
        yield return new ValueTuple<int, int>(x + -1, y + -1);
        yield return new ValueTuple<int, int>(x + 0, y + -1);
        yield return new ValueTuple<int, int>(x + 1, y + -1);

        yield return new ValueTuple<int, int>(x + -1, y + 0);
        yield return new ValueTuple<int, int>(x + 0, y + 0);
        yield return new ValueTuple<int, int>(x + 1, y + 0);

        yield return new ValueTuple<int, int>(x + -1, y + 1);
        yield return new ValueTuple<int, int>(x + 0, y + 1);
        yield return new ValueTuple<int, int>(x + 1, y + 1);
    }

    private string PrintMap(Dictionary<(int X, int Y), bool> currentMap)
    {
        var sb = new StringBuilder();
        var minX = currentMap.Min(x => x.Key.X);
        var minY = currentMap.Min(x => x.Key.Y);
        var maxY = currentMap.Max(x => x.Key.X);
        var maxX = currentMap.Max(x => x.Key.Y);
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                //█
                sb.Append(currentMap.TryGetValue((x, y), out var hit) ? hit ? "#" : "." : ".");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}