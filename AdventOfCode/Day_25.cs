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
        var temporaryWorld = new Occupier[_seaCucumberLairWidth, _seaCucumberLairHeight];


        var i = 1;
        for (;; i++)
        {
            if (!ExecuteMove(in currentWorld, ref temporaryWorld))
            {
                break;
            }

            (currentWorld, temporaryWorld) = (temporaryWorld, currentWorld);
        }

        return new ValueTask<string>(i.ToString());
    }

    private bool ExecuteMove(in Occupier[,] currentWorld, ref Occupier[,] temporaryWorld)
    {
        var moves = false;
        var returnWorld = temporaryWorld;
        Array.Clear(temporaryWorld, 0, temporaryWorld.Length);

        // Handle >
        for (var y = 0; y < _seaCucumberLairHeight; y++)
        {
            for (var x = 0; x < _seaCucumberLairWidth; x++)
            {
                if (currentWorld[x, y] != Occupier.Right)
                {
                    continue;
                }

                var xToCheck = (x + 1) % _seaCucumberLairWidth;

                // If right is empty in current world, we can safely move :)
                if (currentWorld[xToCheck, y] == Occupier.Empty)
                {
                    returnWorld[xToCheck, y] = Occupier.Right;
                    moves = true;
                }
                else
                {
                    returnWorld[x, y] = Occupier.Right;
                }
            }
        }

        // Handle V
        for (var y = 0; y < _seaCucumberLairHeight; y++)
        {
            for (var x = 0; x < _seaCucumberLairWidth; x++)
            {
                if (currentWorld[x, y] != Occupier.Down)
                {
                    continue;
                }

                var yToCheck = (y + 1) % _seaCucumberLairHeight;

                var currentWorldItem = currentWorld[x, yToCheck];
                var returnWorldItem = returnWorld[x, yToCheck];
                // Not moved by ">" and not blocked by "V"
                if (returnWorldItem == Occupier.Empty && currentWorldItem != Occupier.Down)
                {
                    returnWorld[x, yToCheck] = Occupier.Down;
                    moves = true;
                }
                else
                {
                    returnWorld[x, y] = Occupier.Down;
                }
            }
        }

        return moves;
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

public enum Occupier : byte
{
    Empty = 0,
    Down,
    Right,
}