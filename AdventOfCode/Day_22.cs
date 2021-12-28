using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_22 : BaseDay
{
    private readonly string _input;
    private readonly List<RebootStep> _rebootInstructions;

    public Day_22()
    {
        //_input = File.ReadAllText("./Inputs/22tests.txt");
        _input = File.ReadAllText(InputFilePath);

        _rebootInstructions = _input.SplitNewLine().Select(ParseRebootStep).ToList();
    }

    private RebootStep ParseRebootStep(string arg)
    {
        var result = Regex.Match(arg, @"(on|off) x=([-\d]+)\.\.([-\d]+),y=([-\d]+)\.\.([-\d]+),z=([-\d]+)\.\.([-\d]+)");

        return new (result.Groups[2].Value.AsInt(), result.Groups[4].Value.AsInt(), result.Groups[6].Value.AsInt(), result.Groups[3].Value.AsInt(), result.Groups[5].Value.AsInt(), result.Groups[7].Value.AsInt(), result.Groups[1].Value == "on");
    }

    public override ValueTask<string> Solve_1()
        => RunRebootProcedure(_rebootInstructions.Where(x => x.IsInitializationStep()));

    private ValueTask<string> RunRebootProcedure(IEnumerable<RebootStep> rebootSteps)
    {
        List<RebootStep> map = new(1_000);
        var parsedSteps = rebootSteps.Aggregate(map, (currentMap, next) =>
        {
            // Add inverted variant for each overlap
            currentMap.AddRange(currentMap.Where(location => location.DoesIntersect(next)).Select(x => x.Intersection(next)).ToList());
            if (next.On)
            {
                currentMap.Add(next);
            }

            return currentMap;
        });

        var volume = parsedSteps.Aggregate(0L, (curr, next) => curr + next.Volume * (next.On ? 1L : -1L));

        return new ValueTask<string>(volume.ToString());
    }

    public override ValueTask<string> Solve_2()
        => RunRebootProcedure(_rebootInstructions);
}


public record struct RebootStep(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool On)
{
    public bool IsInitializationStep()
        => minX > -50 && maxX < 50 ||
           minY > -50 && maxY < 50 ||
           minZ > -50 && maxZ < 50;
    
    public long Volume => (maxX - minX + 1l) * (maxY - minY + 1l) * (maxZ - minZ + 1l);

    public bool DoesIntersect(RebootStep rebootStep) =>
        !(
            minX > rebootStep.maxX || maxX < rebootStep.minX ||
            minY > rebootStep.maxY || maxY < rebootStep.minY ||
            minZ > rebootStep.maxZ || maxZ < rebootStep.minZ
        );

    public RebootStep Intersection(RebootStep rebootStep) =>
        new(
            Math.Max(minX, rebootStep.minX),
            Math.Max(minY, rebootStep.minY),
            Math.Max(minZ, rebootStep.minZ),
            Math.Min(maxX, rebootStep.maxX),
            Math.Min(maxY, rebootStep.maxY),
            Math.Min(maxZ, rebootStep.maxZ),
            !On);

}