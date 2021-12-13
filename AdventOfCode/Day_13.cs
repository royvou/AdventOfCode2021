using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_13 : BaseDay
{
    public enum FoldDirection
    {
        X,
        Y,
    }

    private readonly IEnumerable<(FoldDirection, int)> _folds;

    private readonly HashSet<(int X, int Y)> _initialMap;
    private readonly string _input;

    public Day_13()
    {
        _input = File.ReadAllText(InputFilePath);

        var splitted = _input.SplitDoubleNewLine();

        _initialMap = splitted[0].SplitNewLine().Select(x =>
        {
            var split = x.Split(",");
            return (X: split[0].AsInt(), Y: split[1].AsInt());
        }).ToHashSet();

        _folds = splitted[1].SplitNewLine().Select(x =>
        {
            var matches = Regex.Match(x, "fold along ([x|y])=(\\d*)");
            return (FoldDirection: Enum.Parse<FoldDirection>(matches.Groups[1].Value, true), FoldPoint: matches.Groups[2].Value.AsInt());
        }).ToList();
    }

    public override ValueTask<string> Solve_1() => new(ExecuteFolds(_folds.Take(1)).Count.ToString());

    private HashSet<(int X, int Y)> ExecuteFolds(IEnumerable<(FoldDirection, int)> folds)
    {
        var currentMap = new HashSet<(int X, int Y)>(_initialMap);
        foreach (var (foldDirection, foldAmount) in folds)
        {
            currentMap = Fold(currentMap, foldDirection, foldAmount);
        }

        return currentMap;
    }

    private static HashSet<(int X, int Y)> Fold(HashSet<(int X, int Y)> currentMap, FoldDirection foldDirection, int foldAmount)
    {
        var result = new HashSet<(int X, int Y)>();
        foreach (var (x, y) in currentMap)
        {
            var newY = foldDirection == FoldDirection.Y && y > foldAmount ? foldAmount - (y - foldAmount) : y;
            var newX = foldDirection == FoldDirection.X && x > foldAmount ? foldAmount - (x - foldAmount) : x;

            result.Add((newX, newY));
        }

        return result;
    }

    public override ValueTask<string> Solve_2() => new(PrintMap(ExecuteFolds(_folds)));

    private string PrintMap(HashSet<(int X, int Y)> currentMap)
    {
        var sb = new StringBuilder();
        var maxY = currentMap.MaxBy(x => x.Y).Y;
        var maxX = currentMap.MaxBy(x => x.X).X;
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                sb.Append(currentMap.TryGetValue((x, y), out var hit) ? "█" : " ");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}