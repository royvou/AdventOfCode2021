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
        => new(WalkCave(new List<Cave> { _caves["start"], }, false).Count().ToString());

    private IEnumerable<IList<Cave>> WalkCave(List<Cave> list, bool maxCountIsTwo)
    {
        var current = list[^1];
        if (current.CaveType == Day12CaveType.Finish)
        {
            yield return list;
            yield break;
        }

        var containsDuplicates = maxCountIsTwo && ContainsDuplicateSingleCave(list);
        var maxCount = maxCountIsTwo && !containsDuplicates;

        foreach (var option in GetOptions(list, maxCount ? 2 : 1))
        {
            var path = new List<Cave>();
            path.AddRange(list);
            path.Add(option);
            var walkedOptions = WalkCave(path, maxCount);

            foreach (var walkOption in walkedOptions)
            {
                yield return walkOption;
            }
        }
    }

  
    public static IEnumerable<Cave> GetOptions(IList<Cave> path, int maxCount)
    {
        var current = path[^1];
        return current.Connections
            .Where(currentCave => currentCave switch
            {
                { CaveType: Day12CaveType.Start, } => false,
                { CaveType: Day12CaveType.Finish, } => true,
                { CaveType: Day12CaveType.Large, } => true,
                { CaveType: Day12CaveType.Small, } => path.Count(x => x.CaveType == Day12CaveType.Small && x.Name == currentCave.Name) < maxCount,
                _ => false,
            });
    }

    private static bool ContainsDuplicateSingleCave(IList<Cave> path)
    {
        var unique = new HashSet<Cave>();
        foreach (var cave in path)
        {
            if (cave.CaveType != Day12CaveType.Small)
            {
                continue;
            }

            if (unique.Contains(cave))
            {
                return true;
            }

            unique.Add(cave);
        }

        return false;

        //return !path.Where(x => x.CaveType == Day12CaveType.Small).GroupBy(x => x).Any(x => x.Count() >= 2);
    }

    public override ValueTask<string> Solve_2()
        => new(WalkCave(new List<Cave> { _caves["start"], }, true).Count().ToString());


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