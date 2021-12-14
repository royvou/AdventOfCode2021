namespace AdventOfCode;

public class Day_04 : BaseDay
{
    private readonly List<BingoCard> _bingoCards;
    private readonly List<BingoCard> _bingoCardWinners;
    private readonly Queue<int> _bingoDrawOrder;
    private readonly string _input;

    public Day_04()
    {
        _input = File.ReadAllText(InputFilePath);

        var lines = _input.SplitDoubleNewLine();

        _bingoDrawOrder = new Queue<int>(lines[0].Split(",").AsInt());
        _bingoCards = lines.Skip(1).Select(CreateMap).ToList();
        _bingoCardWinners = new List<BingoCard>();
    }

    private BingoCard CreateMap(string mapData)
    {
        var map = mapData
            .SplitNewLine()
            .Select((line, y) => line.SplitSpace().Select((@char, x) => ((x, y), (@char.Trim().AsLong(), false))))
            .SelectMany(x => x)
            .ToDictionary(x => x.Item1, y => y.Item2);

        return new BingoCard(map);
    }

    public override ValueTask<string> Solve_1()
    {
        while (_bingoDrawOrder.TryDequeue(out var currentDraw))
        {
            _bingoCardWinners.AddRange(_bingoCards.Where(bingoCard => !bingoCard.HasWon && bingoCard.MarkNumber(currentDraw)));

            if (_bingoCardWinners.Count == 1)
            {
                return new ValueTask<string>((_bingoCardWinners[^1].Score * currentDraw).ToString());
            }
        }

        return new ValueTask<string>("");
    }

    public override ValueTask<string> Solve_2()
    {
        while (_bingoDrawOrder.TryDequeue(out var currentDraw))
        {
            _bingoCardWinners.AddRange(_bingoCards.Where(bingoCard => !bingoCard.HasWon && bingoCard.MarkNumber(currentDraw)));

            if (_bingoCards.Count == _bingoCardWinners.Count)
            {
                return new ValueTask<string>((_bingoCardWinners[^1].Score * currentDraw).ToString());
            }
        }

        return new ValueTask<string>("");
    }

    public class BingoCard
    {
        public BingoCard(IDictionary<(int X, int Y), (long number, bool Checked)> Map)
        {
            this.Map = Map;
            // Map Number to position
            ReverseMap = this.Map.ToDictionary(x => x.Value.number, y => y.Key);
        }

        private Dictionary<long, (int X, int Y)> ReverseMap { get; }

        public long Score => Map.Values.Where(value => !value.Checked).Select(value => value.number).Sum();
        public IDictionary<(int X, int Y), (long number, bool Checked)> Map { get; }

        public bool HasWon { get; private set; }

        public (int X, int Y)? ContainsNumber(long number)
            => ReverseMap.TryGetValue(number, out var result) ? result : null;

        // Marks the number
        public bool MarkNumber(int number)
        {
            var postition = ContainsNumber(number);
            if (!postition.HasValue)
            {
                return false;
            }

            Map[postition.Value] = (number, true);

            return HasWon = ValidateHasWon(postition.Value);
        }

        private bool ValidateHasWon((int X, int Y) lastPosition)
            => Map.Count(coor => coor.Key.X == lastPosition.X && coor.Value.Checked) == 5 ||
               Map.Count(coor => coor.Key.Y == lastPosition.Y && coor.Value.Checked) == 5;

        public void Deconstruct(out IDictionary<(int X, int Y), (long number, bool Checked)> Map)
        {
            Map = this.Map;
        }
    }
}