namespace AdventOfCode;

public class Day_21 : BaseDay
{
    private const int WINNER_SCORE = 1000;
    private readonly string _input;
    private readonly List<PersonOnPosition> _playerStarting;

    public Day_21()
    {
        //_input = File.ReadAllText("./Inputs/21tests.txt");
        _input = File.ReadAllText(InputFilePath);

        _playerStarting = _input.SplitNewLine().Select((x, index) => new PersonOnPosition(index.ToString())
        {
            Index = x[^1].AsInt(),
        }).ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        var i = 0;
        for (;; i++)
        {
            var person = _playerStarting[i % 2];
            var ahead = (3 * (i + 1) - 1) * 3;
            person.Index = (person.Index + ahead - 1) % 10 + 1;
            person.Score += person.Index;
            
            if (person.Score >= WINNER_SCORE)
            {
                break;
            }
        }

        var loser = _playerStarting.MinBy(x => x.Score);
        return new ValueTask<string>((loser.Score * (i + 1) * 3).ToString());
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);
}

public record PersonOnPosition(string Name)
{
    public int Index { get; set; }

    public int Score { get; set; }
}