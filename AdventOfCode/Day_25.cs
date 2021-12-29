using System.Text;

namespace AdventOfCode;

public class Day_25 : BaseDay
{
    private readonly string _input;
    private readonly Occupier[,] _seaCucumberLair;
    private readonly int _seaCucumberLairHeight;
    private readonly int _seaCucumberLairWidth;

    public Day_25()
    {
        //_input = File.ReadAllText("./Inputs/25tests.txt");
        _input = File.ReadAllText(InputFilePath);

        _seaCucumberLair = ParseInput(_input);
        _seaCucumberLairWidth = _seaCucumberLair.GetLength(0);
        _seaCucumberLairHeight = _seaCucumberLair.GetLength(1);
    }

    private Occupier[,] ParseInput(string input)
    {
        var parse = (char c) => c switch
        {
            '.' => Occupier.Empty,
            '>' => Occupier.Right,
            'v' => Occupier.Down,
            _ => Occupier.Empty,
        };

        var lines = input.SplitNewLine();
        var multiArray = new Occupier[lines[0].Length, lines.Length];
        for (var y = 0; y < lines.Length; y++)
        {
            var currentLine = lines[y];
            for (var x = 0; x < currentLine.Length; x++)
            {
                var c = currentLine[x];
                multiArray[x, y] = parse(c);
            }
        }

        return multiArray;
    }

    public override ValueTask<string> Solve_1()
    {
        var currentWorld = _seaCucumberLair;

        var i = 1;
        for (;; i++)
        {
            var result = ExecuteMove(in currentWorld);
            currentWorld = result.returnWorld;
            if (result.moves == 0)
            {
                break;
            }
        }

        return new ValueTask<string>(i.ToString());
    }

    private (Occupier[,] returnWorld, int moves) ExecuteMove(in Occupier[,] currentWorld)
    {
        var moves = 0;
        var returnWorld = new Occupier[_seaCucumberLairWidth, _seaCucumberLairHeight];
        for (var y = 0; y < _seaCucumberLairHeight; y++)
        {
            for (var x = 0; x < _seaCucumberLairWidth; x++)
            {
                if (currentWorld[x, y] == Occupier.Right)
                {
                    if (currentWorld[(x + 1) % _seaCucumberLairWidth, y % _seaCucumberLairHeight] == Occupier.Empty)
                    {
                        returnWorld[(x + 1) % _seaCucumberLairWidth, y % _seaCucumberLairHeight] = Occupier.Right;
                        moves++;
                    }
                    else
                    {
                        returnWorld[x, y] = Occupier.Right;
                    }
                }
            }
        }

        for (var y = 0; y < _seaCucumberLairHeight; y++)
        {
            for (var x = 0; x < _seaCucumberLairWidth; x++)
            {
                if (currentWorld[x, y] == Occupier.Down)
                {
                    var currentWorldItem = currentWorld[x % _seaCucumberLairWidth, (y + 1) % _seaCucumberLairHeight];
                    var returnWorldItem = returnWorld[x % _seaCucumberLairWidth, (y + 1) % _seaCucumberLairHeight];
                    // Not moved by ">" and not blocked by "V"
                    if (returnWorldItem == Occupier.Empty && currentWorldItem != Occupier.Down)
                    {
                        returnWorld[x % _seaCucumberLairWidth, (y + 1) % _seaCucumberLairHeight] = Occupier.Down;
                        moves++;
                    }
                    else
                    {
                        returnWorld[x, y] = Occupier.Down;
                    }
                }
            }
        }

        return (returnWorld, moves);
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);

    private string PrintMap(Occupier[,] occupiers)
    {
        var sb = new StringBuilder();
        for (var y = 0; y < occupiers.GetLength(1); y++)
        {
            for (var x = 0; x < occupiers.GetLength(0); x++)
            {
                var item = occupiers[x, y] switch
                {
                    Occupier.Down => 'v',
                    Occupier.Right => '>',
                    _ => '.',
                };
                sb.Append(item);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public enum Occupier
{
    Empty = 0,
    Down,
    Right,
}