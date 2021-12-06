namespace AdventOfCode;

public class Day_06 : BaseDay
{
    private readonly List<long> _initialState;
    private readonly string _input;

    public Day_06()
    {
        _input = File.ReadAllText(InputFilePath);

        _initialState = _input.Split(",").AsLong().ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var state = new LinkedList<long>(_initialState);
        for (var i = 0; i < 80; i++)
        {
            state = SimulateDay(state);
        }

        return new ValueTask<string>(state.Count().ToString());
    }

    private LinkedList<long> SimulateDay(LinkedList<long> fishes)
    {
        var result = new LinkedList<long>();
        foreach (var fish in fishes)
        {
            if (fish == 0)
            { 
                result.AddLast(6);
                result.AddLast(8);
            }
            else
            {
                result.AddLast(fish - 1);
            }
        }

        return result;
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}