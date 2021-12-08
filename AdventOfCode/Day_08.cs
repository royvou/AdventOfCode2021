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
            return new Day08NoteEntry(splitted[0].Trim().Split(" ").Select(x => x.SortString()).ToArray(), splitted[1].Trim().Split(" ").Select(x => x.SortString()).ToArray());
        }).ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        // 1 4 7 8
        var validdPatternSizes = new List<int> { 2, 4, 3, 7, };
        var valid = _notes.SelectMany(x => x.Outputs).Select(x => x.Length).Where(patternLength => validdPatternSizes.Contains(patternLength));

        return new ValueTask<string>(valid.Count().ToString());
    }

    public override ValueTask<string> Solve_2()
        => new(_notes.Select(CountLine).Sum().ToString());

    private int CountLine(Day08NoteEntry note)
    {
        var mapping = new Dictionary<string, int>();

        var firstPatternOfLength = (int length) => note.Patterns.First(pattern => pattern.Length == length);
        var patternsOfLength = (int length) => note.Patterns.Where(pattern => pattern.Length == length);

        var foundNumber = (int foundNumber, string pattern) => mapping[pattern] = foundNumber;

        var pattern1 = firstPatternOfLength(2);
        var pattern4 = firstPatternOfLength(4);
        var pattern7 = firstPatternOfLength(3);
        var pattern8 = firstPatternOfLength(7);
        
        var segmentTop = pattern7.Except(pattern1);

        var length5Options = patternsOfLength(5).ToList();
        var length6Options = patternsOfLength(6).ToList();

        var pattern6 = length6Options.Single(x => x.Intersect(pattern1).Count() == 1);
        length6Options.Remove(pattern6);

        var bottomRight = pattern1.Intersect(pattern6).Single();
        var topRight = pattern1.Single(c => c != bottomRight);

        var pattern2 = length5Options.Single(p => p.Contains(topRight) && !p.Contains(bottomRight));
        length5Options.Remove(pattern2);
        var pattern5 = length5Options.Single(p => !p.Contains(topRight) && p.Contains(bottomRight));
        length5Options.Remove(pattern5);
        var pattern3 = length5Options.Single();
        length5Options.Remove(pattern3);

        var bottomLeft = pattern2.Except(pattern5).Single(x => x != topRight);

        var pattern0 = length6Options.Single(p => p.Contains(bottomLeft));
        length6Options.Remove(pattern0);
        var pattern9 = length6Options.Single();
        length6Options.Remove(pattern9);

        foundNumber(0, pattern0);
        foundNumber(1, pattern1);
        foundNumber(2, pattern2);
        foundNumber(3, pattern3);
        foundNumber(4, pattern4);
        foundNumber(5, pattern5);
        foundNumber(6, pattern6);
        foundNumber(7, pattern7);
        foundNumber(8, pattern8);
        foundNumber(9, pattern9);

        return note.Outputs.Select(x => mapping[x]).Aggregate(0, (acc, curr) => curr + 10 * acc);
    }

    public record Day08NoteEntry(string[] Patterns, string[] Outputs);
}