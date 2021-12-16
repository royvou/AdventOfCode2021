using System.Text;

namespace AdventOfCode;

public class Day_15 : BaseDay
{
    private readonly string _input;
    private readonly Dictionary<(int X, int Y), int> _map;
    private readonly (int, int) _map2End;
    private readonly (int, int) _mapEnd;
    private readonly Dictionary<(int X, int Y), int> _mapPart2;
    private readonly int _mapPart2Size;
    private readonly int _mapSize;
    private readonly (int, int) _start;


    public Day_15()
    {
        _input = File.ReadAllText(InputFilePath);
        _start = (0, 0);

        _map = ParseInput(_input);
        _mapSize = _map.Max(x => x.Key.X) + 1;
        _mapEnd = (_mapSize - 1, _mapSize - 1);

        _mapPart2 = GeneratePart2Map(_map, _mapSize, 5);
        _mapPart2Size = _mapSize * 5;
        _map2End = (_mapPart2Size - 1, _mapPart2Size - 1);
    }

    private static Dictionary<(int X, int Y), int> GeneratePart2Map(Dictionary<(int X, int Y), int> map, int mapSize, int bigger)
    {
        Dictionary<(int X, int Y), int> resultMap = new();
        {
            for (var y = 0; y < bigger; y++)
            {
                for (var x = 0; x < bigger; x++)
                {
                    foreach (var location in map)
                    {
                        var newValue = (map[location.Key] + x + y - 1) % 9 + 1;
                        var newPos = (location.Key.X + mapSize * x, location.Key.Y + mapSize * y);
                        resultMap[newPos] = newValue;
                    }
                }
            }
        }

        return resultMap;
    }

    private static Dictionary<(int X, int Y), int> ParseInput(string input)
    {
        var map = new Dictionary<(int X, int Y), int>();
        var lines = input.SplitNewLine();
        for (var y = 0; y < lines.Length; y++)
        {
            var currentLine = lines[y];
            for (var x = 0; x < currentLine.Length; x++)
            {
                var c = currentLine[x];
                map[(x, y)] = c.AsInt();
            }
        }

        return map;
    }

    public override ValueTask<string> Solve_1()
        => new(AStar(_map, (0, 0), _mapEnd).ToString());

    private static long AStar(Dictionary<(int X, int Y), int> map, (int X, int Y) start, (int X, int Y) target)
    {
        PriorityQueue<(int X, int Y), long> openNodes = new();

        Dictionary<(int X, int Y), ((int X, int Y)? Pos, int? Gscore, int? Fscore)> NodeInfo = new();

        NodeInfo[start] = (null, 0, 0);

        openNodes.Enqueue(start, NodeInfo[start].Fscore.GetValueOrDefault(0));

        while (openNodes.TryDequeue(out var current, out var _))
        {
            if (current == target)
            {
                return NodeInfo[target].Gscore.GetValueOrDefault();
            }

            foreach (var neighbour in PossibleNeighbours(current))
            {
                NodeInfo.TryGetValue(current, out var currentNodeInfo);
                var hasNeighbour = NodeInfo.TryGetValue(neighbour, out var neighbourInfo);
                //Not using int.max to avoid overflow
                var tentGScore = currentNodeInfo.Gscore.GetValueOrDefault() + map.GetValueOrDefault(neighbour, 10_000_000);
                if (tentGScore < (neighbourInfo.Gscore ?? int.MaxValue) && map.ContainsKey(neighbour))
                {
                    neighbourInfo.Pos = current;
                    neighbourInfo.Gscore = tentGScore;
                    var fScore = tentGScore + ManhattenDistance(current, target);
                    neighbourInfo.Fscore = fScore;
                    
                    NodeInfo[neighbour] = neighbourInfo;
                    
                    openNodes.Enqueue(neighbour, fScore);
                }
            }
        }

        return 0;
    }

    private static int ManhattenDistance((int X, int Y) start, (int X, int Y) end)
        => Math.Abs(end.X - start.X) + Math.Abs(end.Y - start.Y);

    private static IEnumerable<(int x, int y)> PossibleNeighbours((int X, int Y) element)
    {
        var (x, y) = element;
        yield return (x + 0, y + -1);
        yield return (x + -1, y + 0);
        yield return (x + 1, y + 0);
        yield return (x + 0, y + 1);
    }

    private static IEnumerable<(int X, int Y)> GetPath(Dictionary<(int X, int Y), (int X, int Y)> paths, (int X, int Y) current)
    {
        yield return current;
        var whileCurrent = current;
        while (paths.TryGetValue(whileCurrent, out var path))
        {
            yield return path;
            whileCurrent = path;
        }
    }

    public override ValueTask<string> Solve_2() => new(AStar(_mapPart2, _start, _map2End).ToString());

    private string PrintMap(Dictionary<(int X, int Y), int> currentMap)
    {
        var sb = new StringBuilder();
        var maxY = currentMap.Max(x => x.Key.Y);
        var maxX = currentMap.Max(x => x.Key.X);
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                sb.Append(currentMap.TryGetValue((x, y), out var hit) ? hit.ToString() : "█");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}