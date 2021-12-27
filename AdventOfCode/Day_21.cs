namespace AdventOfCode;

public class Day_21 : BaseDay
{
    private const int WINNER_SCORE = 1000;
    private readonly string _input;
    private readonly List<PersonOnPositionP1> _playerStartingP1;

    private readonly List<PersonOnPositionP2> _playerStartingP2;


    private readonly Dictionary<(int, long, int, long), (long, long)> Part2Cache = new();

    public (int Sum, int Count)[] DiracSums =
    {
        (3, 1), (4, 3), (5, 6), (6, 7), (7, 6), (8, 3), (9, 1),
    };


    public Day_21()
    {
        //_input = File.ReadAllText("./Inputs/21tests.txt");
        _input = File.ReadAllText(InputFilePath);

        _playerStartingP1 = _input.SplitNewLine().Select((x, index) => new PersonOnPositionP1(index.ToString())
        {
            Index = x[^1].AsInt(),
        }).ToList();

        _playerStartingP2 = _input.SplitNewLine().Select((x, index) => new PersonOnPositionP2(index.ToString(), x[^1].AsInt())).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var i = 0;
        for (;; i++)
        {
            var person = _playerStartingP1[i % 2];
            var ahead = (3 * (i + 1) - 1) * 3;
            person.Index = (person.Index + ahead - 1) % 10 + 1;
            person.Score += person.Index;

            if (person.Score >= WINNER_SCORE)
            {
                break;
            }
        }

        var loser = _playerStartingP1.MinBy(x => x.Score);
        return new ValueTask<string>((loser.Score * (i + 1) * 3).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var player1 = _playerStartingP2[0];
        var player2 = _playerStartingP2[1];

        var (scoreP1, scoreP2) = PlayPart2(player1, player2);
        return new ValueTask<string>((scoreP1 > scoreP2 ? scoreP1 : scoreP2).ToString());
    }

    public (long ScoreP1, long ScoreP2) PlayPart2(PersonOnPositionP2 player1, PersonOnPositionP2 player2)
    {
        var cacheKey = (player1.Index, player1.Score, player2.Index, player2.Score);
        if (Part2Cache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        if (player1.Score >= 21)
        {
            return (1, 0);
        }

        if (player2.Score >= 21)
        {
            return (0, 1);
        }

        var scoreP1 = 0l;
        var scoreP2 = 0l;
        foreach (var (sum, count) in DiracSums)
        {
            var newIndex = (player1.Index + sum - 1) % 10 + 1;
            var updatedActivePlayer = player1 with
            {
                Index = newIndex,
                Score = player1.Score + newIndex + 0,
            };

            // Swap players
            var (addedScore2, addedScore1) = PlayPart2(player2, updatedActivePlayer);

            scoreP1 += addedScore1 * count;
            scoreP2 += addedScore2 * count;
        }

        Part2Cache[cacheKey] = (scoreP1, scoreP2);

        return (scoreP1, scoreP2);
    }
}

public record PersonOnPositionP1(string Name)
{
    public int Index { get; set; }

    public int Score { get; set; }
}

public record PersonOnPositionP2(string Name, int Index, long Score = 0)
{
}