namespace AdventOfCode;

public class Day_12 : BaseDay
{
    public enum Day12CaveType
    {
        Start,
        Large,
        Small,
        Finish,
    }

    private readonly Dictionary<string, Cave> _caves;
    private readonly string _input;

    public Day_12()
    {
        _input = File.ReadAllText(InputFilePath);

        _caves = GetCaveSystem(_input);
    }

    public Dictionary<string, Cave> GetCaveSystem(string input)
    {
        var connections = input.SplitNewLine().Select(x =>
        {
            var split = x.Split("-");
            return (Name: split[0], Connections: split[1]);
        }).GroupBy(x => x.Name, y => y.Connections).ToDictionary(x => x.Key, y => y.ToList());

        var caves = connections.Keys.Select(x => x).Concat(connections.Values.SelectMany(connection => connection)).Distinct().Select(caveName => new Cave(caveName, new List<Cave>())).ToDictionary(x => x.Name, y => y);

        foreach (var cave in caves.Values)
        {
            // Direct connections
            if (connections.TryGetValue(cave.Name, out var caveConnections))
            {
                cave.Connections.AddRange(caveConnections.Select(caveName => caves[caveName]));
            }

            // Reverse Connections
            cave.Connections.AddRange(connections.Where(x => x.Value.Contains(cave.Name)).Select(connection => caves[connection.Key]));
        }

        return caves;
    }


    public override ValueTask<string> Solve_1()
        => new(WalkCave(_caves["start"]).ToString());

    private int WalkCave(Cave current, Dictionary<string, int> visited = null, bool maxCountIsTwo = false)
    {
        visited ??= new Dictionary<string, int>
        {
            [current.Name] = 1,
        };

        var result = 0;
        foreach (var option in current.Connections)
        {
            if (option.CaveType == Day12CaveType.Start)
            {
                continue;
            }

            if (option.CaveType == Day12CaveType.Small)
            {
                var maxCount = maxCountIsTwo ? 2 : 1;
                if (visited.TryGetValue(option.Name, out var visitCount) && visitCount >= maxCount)
                {
                    continue;
                }
            }

            if (option.CaveType == Day12CaveType.Finish)
            {
                result += 1;
                continue;
            }


            var currentVisitAmount = visited.TryGetValue(option.Name, out var count) ? count + 1 : 1;
            visited[option.Name] = currentVisitAmount;
            var walkedOptions = WalkCave(option, visited, maxCountIsTwo && (option.CaveType != Day12CaveType.Small || currentVisitAmount < 2));
            visited[option.Name] = currentVisitAmount - 1;
            result += walkedOptions;
        }

        return result;
    }


    public static IEnumerable<Cave> GetOptions(Cave current, IDictionary<string, int> visited, int maxCount)
        => current.Connections
            .Where(currentCave => currentCave switch
            {
                { CaveType: Day12CaveType.Start, } => false,
                { CaveType: Day12CaveType.Finish, } => true,
                { CaveType: Day12CaveType.Large, } => true,
                { CaveType: Day12CaveType.Small, } => !visited.TryGetValue(currentCave.Name, out var count) || count < maxCount,
                _ => false,
            });

    public override ValueTask<string> Solve_2()
        => new(WalkCave(_caves["start"], null, true).ToString());


    private static Day12CaveType GetCaveType(string name)
        => name switch
        {
            "start" => Day12CaveType.Start,
            "end" => Day12CaveType.Finish,
            _ when char.IsUpper(name[0]) => Day12CaveType.Large,
            _ => Day12CaveType.Small,
        };

    public record Cave(string Name, List<Cave> Connections)
    {
        public Day12CaveType CaveType { get; } = GetCaveType(Name);
    }
}