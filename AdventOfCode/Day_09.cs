using System.Drawing;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public class Day_09 : BaseDay
{
    private readonly string _input;
    private readonly int[,] _parsedInput;
    private readonly int _parsedInputXLength;
    private readonly int _parsedInputYLength;


    public Day_09()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = ParseInput(_input);
        _parsedInputXLength = _parsedInput.GetLength(0);
        _parsedInputYLength = _parsedInput.GetLength(1);
    }

    private int[,] ParseInput(string input)
    {
        var lines = input.SplitNewLine();
        var multiArray = new int[lines[0].Length, lines.Length];
        for (var y = 0; y < lines.Length; y++)
        {
            var currentLine = lines[y];
            for (var x = 0; x < currentLine.Length; x++)
            {
                var c = currentLine[x];
                multiArray[x, y] = c.AsInt();
            }
        }

        return multiArray;
    }

    public override ValueTask<string> Solve_1()
        => new(GetLowestPoints().Select(pos => GetHeightAt(pos.X, pos.Y)).Aggregate(0, (acc, curr) => acc + curr + 1).ToString());

    private IEnumerable<(int X, int Y)> GetLowestPoints()
    {
        for (var y = 0; y < _parsedInputYLength; y++)
        {
            for (var x = 0; x < _parsedInputXLength; x++)
            {
                var lowestSurrounding = GetSurroundingHeight(x, y).Where(x => x >= 0).OrderBy(x => x).FirstOrDefault();

                var currentLocation = GetHeightAt(x, y);
                if (currentLocation < lowestSurrounding)
                {
                    yield return (x, y);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<int> GetSurroundingHeight(int x, int y)
    {
        yield return GetHeightAt(x + -1, y + 0);
        yield return GetHeightAt(x + 1, y + 0);
        yield return GetHeightAt(x + 0, y + -1);
        yield return GetHeightAt(x + 0, y + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetHeightAt(int currX, int currY)
        => currX >= 0 && currY >= 0 && currX < _parsedInputXLength && currY < _parsedInputYLength ? _parsedInput[currX, currY] : -1;

    public override ValueTask<string> Solve_2()
        => new(GetLowestPoints().Select(pos => GetBasinSizeAt(pos.X, pos.Y)).OrderByDescending(x => x).Take(3).Aggregate(1, (acc, curr) => acc * curr).ToString());

    // Depth first search
    private int GetBasinSizeAt(int x, int y, HashSet<Point> visited = null)
    {
        visited ??= new HashSet<Point>();
        var stack = new Stack<Point>();

        stack.Push(new Point(x, y));
        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);

            var height = GetHeightAt(current.X, current.Y);
            foreach (var neighbour in ValidNeighBours(current.X, current.Y))
            {
                if (visited.Contains(neighbour))
                {
                    continue;
                }

                var neightBourHeight = GetHeightAt(neighbour.X, neighbour.Y);
                if (height < neightBourHeight && neightBourHeight < 9)
                {
                    stack.Push(neighbour);
                }
            }
        }

        return visited.Count;
    }

    //996170 incorrect
    public IEnumerable<Point> ValidNeighBours(int x, int y)
        => Neighbours(x, y).Where(pos => pos.X >= 0 && pos.X < _parsedInputXLength && pos.Y >= 0 && pos.Y < _parsedInputYLength);

    public IEnumerable<Point> Neighbours(int x, int y)
    {
        yield return new Point(x + -1, y + 0);
        yield return new Point(x + 1, y + 0);
        yield return new Point(x + 0, y + -1);
        yield return new Point(x + 0, y + 1);
    }
}