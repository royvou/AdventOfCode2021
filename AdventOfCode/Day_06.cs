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

    public override ValueTask<string> Solve_1() => new(SimulateDays(80).ToString());

    public override ValueTask<string> Solve_2() => new(SimulateDays(256).ToString());

    private long SimulateDays(int days)
    {
        var sea = new Sea();
        sea.Initialize(_initialState);

        for (var i = 0; i < days; i++)
        {
            sea.SimulateDay();
        }

        return sea.Count;
    }

    public class Sea
    {
        public Sea()
        {
            FishPerLifespan = Enumerable.Range(0, 9).Select(x => 0l).ToArray();
        }

        public long[] FishPerLifespan { get; set; }


        public void SimulateDay()
        {
            var respawn = FishPerLifespan[0];
            for (var i = 0; i < FishPerLifespan.Length - 1; i++)
            {
                FishPerLifespan[i] = FishPerLifespan[i + 1];
            }

            FishPerLifespan[6] += respawn;
            FishPerLifespan[8] = respawn;
        }

        public long Count => FishPerLifespan.Sum();

        public void Initialize(List<long> initialState)
        {
            foreach (var group in initialState.GroupBy(x => x))
            {
                FishPerLifespan[group.Key] = group.Count();
            }
        }
    }
}