namespace AdventOfCode;

public class Day_08 : BaseDay
{
    private readonly string _input;
    private readonly Day08NoteEntry[] _notes;

    public Day_08()
    {
        _input = File.ReadAllText(InputFilePath);

        _notes = _input.SplitNewLine().Select(line =>
        {
            var splitted = line.Split("|");
            return new Day08NoteEntry(splitted[0].Split(" ".ToArray()), splitted[1].Split(" ").ToArray());
        }).ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        // 1 4 7 8
        var validdPatternSizes = new List<int> { 2, 4, 3, 7, };
        var valid = _notes.SelectMany(x => x.Output).Select(x => x.Length).Where(patternLength => validdPatternSizes.Contains(patternLength));

        return new ValueTask<string>(valid.Count().ToString());
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);

    public record Day08NoteEntry(string[] Pattern, string[] Output);
}