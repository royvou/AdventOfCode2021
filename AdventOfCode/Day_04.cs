namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly List<Day04Map> _bingoCards;
    private readonly int[] _bingoDrawOrder;
    private readonly string _input;

    public Day_04()
    {
        _input = File.ReadAllText(InputFilePath);

        var lines = _input.SplitDoubleNewLine();

        _bingoDrawOrder = lines[0].Split(",").AsInt().ToArray();
        _bingoCards = lines.Skip(1).Select(CreateMap).ToList();
    }

    private Day04Map CreateMap(string mapData)
    {
        var map = mapData
            .SplitNewLine()
            .Select((line, y) => line.SplitSpace().Select((@char, x) => ((x, y), (@char.Trim().AsLong(), false))))
            .SelectMany(x => x)
            .ToDictionary(x => x.Item1, y => y.Item2);

        return new Day04Map(map);
    }

    public override ValueTask<string> Solve_1()
    {
        foreach (var bingoDraw in _bingoDrawOrder)
        {
            foreach (var map in _bingoCards)
            {
                map.MarkNumber(bingoDraw);
            }

            var winner = _bingoCards.FirstOrDefault(map => map.HasWon());
            if (winner != default)
            {
                return new ValueTask<string>((winner.Score * bingoDraw).ToString());
            }
        }

        return new ValueTask<string>(string.Empty);
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);

    public record Day04Map(IDictionary<(int X, int Y), (long number, bool Checked)> Map)
    {
        public long Score => Map.Values.Where(value => !value.Checked).Select(value => value.number).Sum();

        public (int X, int Y)? ContainsNumber(long number)
        {
            var hits = Map.Where(x => x.Value.number == number).Take(1).ToList();
            return hits.Count > 0 ? hits[0].Key : null;
        } 

        public void MarkNumber(int number)
        {
            var postition = ContainsNumber(number);
            if (postition.HasValue)
            {
                Map[postition.Value] = (number, true);
            }
        }

        public bool HasWon()
        {
            return Map.GroupBy(x => x.Key.X).Any(x => x.All(y => y.Value.Checked)) ||
                   Map.GroupBy(x => x.Key.Y).Any(x => x.All(y => y.Value.Checked));
        }
    }
}