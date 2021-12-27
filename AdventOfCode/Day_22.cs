using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_22 : BaseDay
{
    private readonly string _input;
    private readonly List<RebootStep> _rebootInstructions;

    public Day_22()
    {
        // _input = File.ReadAllText("./Inputs/22tests.txt");
        _input = File.ReadAllText(InputFilePath);

        _rebootInstructions = _input.SplitNewLine().Select(ParseRebootStep).ToList();
    }

    private RebootStep ParseRebootStep(string arg)
    {
        var result = Regex.Match(arg, @"(on|off) x=([-\d]+)\.\.([-\d]+),y=([-\d]+)\.\.([-\d]+),z=([-\d]+)\.\.([-\d]+)");

        return new RebootStep(result.Groups[2].Value.AsInt(), result.Groups[4].Value.AsInt(), result.Groups[6].Value.AsInt(), result.Groups[3].Value.AsInt(), result.Groups[5].Value.AsInt(), result.Groups[7].Value.AsInt(), result.Groups[1].Value == "on");
    }

    public override ValueTask<string> Solve_1()
    {
        Dictionary<(int X, int Y, int Z), bool> map = new();
        _rebootInstructions.Where(x => x.minX > -50 && x.minX < 50).Aggregate(map, (curr, next) =>
        {
            for (var currZ = next.minZ; currZ <= next.maxZ; currZ++)
            {
                for (var currY = next.minY; currY <= next.maxY; currY++)
                {
                    for (var currX = next.minX; currX <= next.maxX; currX++)
                    {
                        if (next.On)
                        {
                            curr[(currX, currY, currZ)] = true;
                        }
                        else
                        {
                            curr.Remove((currX, currY, currZ));
                        }
                    }
                }
            }

            return curr;
        });
        return new ValueTask<string>(map.Values.Count.ToString());
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}

public record RebootStep(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool On);