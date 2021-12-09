using System.Runtime.CompilerServices;

namespace AdventOfCode;

public class Day_09 : BaseDay
{
    private readonly string _input;
    private readonly int[,] _parsedInput;
    private readonly int _parsedInputXLength;
    private readonly int _parsedInputYLength;


    public Day_09()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = ParseInput(_input);
        _parsedInputXLength = _parsedInput.GetLength(0);
        _parsedInputYLength = _parsedInput.GetLength(1);
    }

    private int[,] ParseInput(string input)
    {
        var lines = input.SplitNewLine();
        var multiArray = new int[lines[0].Length, lines.Length];
        for (var y = 0; y < lines.Length; y++)
        {
            var currentLine = lines[y];
            for (var x = 0; x < currentLine.Length; x++)
            {
                var c = currentLine[x];
                multiArray[x, y] = c.AsInt();
            }
        }

        return multiArray;
    }

    public override ValueTask<string> Solve_1()
    {
        var riskLevel = 0;
        for (var y = 0; y < _parsedInputYLength; y++)
        {
            for (var x = 0; x < _parsedInputXLength; x++)
            {
                var lowestSurrounding = GetSurrounding(x, y).Where(x => x >= 0).OrderBy(x => x).FirstOrDefault();

                var currentLocation = GetItemAt(x, y);
                if (currentLocation < lowestSurrounding)
                {
                    riskLevel += 1 + currentLocation;
                }
            }
        }

        return new ValueTask<string>(riskLevel.ToString());
    }

    private IEnumerable<int> GetSurrounding(int x, int y)
    {
        yield return GetItemAt(x + -1, y + 0);
        yield return GetItemAt(x + 1, y + 0);
        yield return GetItemAt(x + 0, y + -1);
        yield return GetItemAt(x + 0, y + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetItemAt(int currX, int currY)
        => currX >= 0 && currY >= 0 && currX < _parsedInputXLength && currY < _parsedInputYLength ? _parsedInput[currX, currY] : -1;

    public override ValueTask<string> Solve_2() => new(string.Empty);
}