namespace AdventOfCode;

public class Day_24 : BaseDay
{
    private readonly string _input;
    private readonly (int SV1, int SV2)[] _instructionSpecialValues;

    public Day_24()
    {
        _input = File.ReadAllText(InputFilePath);
        var inputFileLines = _input.SplitNewLine();

        _instructionSpecialValues = Enumerable.Range(0, 14).Select(i => (SV1: int.Parse(inputFileLines[i * 18 + 5][6..]), SV2: int.Parse(inputFileLines[i * 18 + 15][6..]))).ToArray();
    }


    private long GenerateValidNumbers(bool smallest)
    {
        // 14 Numbers each have 2 special cases (6th row and 15th row from last)
        var instructionSpecialValues = _instructionSpecialValues;

        var stack = new Stack<(int, int)>();
        var keys = new Dictionary<int, (int index, int value)>();

        foreach (var (specialValuePair, i) in instructionSpecialValues.Select((pair, i) => (pair, i)))
        {
            if (specialValuePair.SV1 > 0)
            {
                stack.Push((i, specialValuePair.SV2));
            }
            else
            {
                var (i2, sv2) = stack.Pop();
                keys[i] = (i2, sv2 + specialValuePair.SV1);
            }
        }

        var finalAnswer = new int[14];
        foreach (var (key, val) in keys)
        {
            finalAnswer[key] = !smallest ? Math.Min(9, 9 + val.value) : Math.Max(1, 1 + val.value);
            finalAnswer[val.index] = !smallest ? Math.Min(9, 9 - val.value) : Math.Max(1, 1 - val.value);
        }

        return long.Parse(string.Join("", finalAnswer));
    }

    public override ValueTask<string> Solve_1() => new(GenerateValidNumbers(false).ToString());
    
    public override ValueTask<string> Solve_2() => new(GenerateValidNumbers(true).ToString());
}