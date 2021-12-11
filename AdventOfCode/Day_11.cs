namespace AdventOfCode;

public class Day_11 : BaseDay
{
    private readonly int[,] _initialMap;
    private readonly int _initialMapXLength;
    private readonly int _initialMapYLength;
    private readonly string _input;

    public Day_11()
    {
        _input = File.ReadAllText(InputFilePath);
        _initialMap = ParseMap(_input);
        _initialMapXLength = _initialMap.GetLength(0);
        _initialMapYLength = _initialMap.GetLength(1);
    }

    private int[,] ParseMap(string input)
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
    {
        var newMap = (int[,])_initialMap.Clone();

        var score = Enumerable.Range(0, 100).Aggregate(0, (acc, curr) => acc + SimulateStep(ref newMap));

        return new ValueTask<string>(score.ToString());
    }

    private int SimulateStep(ref int[,] currentMap)
    {
        var seen = new HashSet<(int X, int Y)>();
        var toCheck = new Queue<(int X, int Y)>();

        for (var y = 0; y < _initialMapYLength; y++)
        {
            for (var x = 0; x < _initialMapXLength; x++)
            {
                currentMap[x, y] += 1;

                if (currentMap[x, y] == 10)
                {
                    toCheck.Enqueue((x, y));
                }
            }
        }

        var score = 0;
        while (toCheck.Any())
        {
            var (x, y) = toCheck.Dequeue();
            if (seen.Contains((x, y)))
            {
                continue;
            }

            triggerExplosion(ref currentMap, ref toCheck, ref seen, x, y);
            seen.Add((x, y));
        }

        return seen.Count;
    }

    private void triggerExplosion(ref int[,] currentMap, ref Queue<(int X, int Y)> toCheck, ref HashSet<(int X, int Y)> seen, int toCheckX, int toCheckY)
    {
        currentMap[toCheckX, toCheckY] = 0;
        foreach (var (x, y) in ValidNeighbours(toCheckX, toCheckY))
        {
            var currentEnery = currentMap[x, y];

            if (currentEnery > 0)
            {
                currentMap[x, y] += 1;
            }

            var newEnery = currentMap[x, y];
            if (newEnery > 9 && !seen.Contains((x, y)))
            {
                toCheck.Enqueue((x, y));
            }
        }
    }

    public IEnumerable<(int X, int Y)> ValidNeighbours(int x, int y)
        => Neighbours(x, y).Where(pos => pos.X >= 0 && pos.Y >= 0 && pos.X < _initialMapXLength && pos.Y < _initialMapYLength);

    public IEnumerable<(int X, int Y)> Neighbours(int x, int y)
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

    public override ValueTask<string> Solve_2()
    {
        var newMap = (int[,])_initialMap.Clone();

        for (var i = 1;; i++)
        {
            if (SimulateStep(ref newMap) == newMap.Length)
            {
                return new ValueTask<string>(i.ToString());
            }
        }

        return new ValueTask<string>("");
    }
}