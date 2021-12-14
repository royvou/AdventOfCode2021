namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly char[] _initialList;
    private readonly string _input;
    private readonly Dictionary<(char, char), char> _insertionMapping;

    public Day_14()
    {
        _input = File.ReadAllText(InputFilePath);
        var splitGroups = _input.SplitDoubleNewLine();

        _initialList = splitGroups[0].ToCharArray();

        _insertionMapping = splitGroups[1].SplitNewLine().ToDictionary(x => (x[0], x[1]), y => y[^1]);
    }

    public override ValueTask<string> Solve_1()
    {
        var list = new LinkedList<char>(_initialList);

        for (int i = 0; i < 10; i++)
        {
            var current = list.First;
            while (current != default && current.Next != default)
            {
                var next = current.Next;

                if (_insertionMapping.TryGetValue((current.Value, next.Value), out var toInsert))
                {
                    list.AddAfter(current, toInsert);
                }


                current = next;
            }

        }
        
        var grouping = list.GroupBy(x => x);
        return new ValueTask<string>((grouping.Max(x => x.Count()) - grouping.Min(x => x.Count())).ToString());
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}