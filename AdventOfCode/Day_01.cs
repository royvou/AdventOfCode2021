namespace AdventOfCode;

public class Day_01 : BaseDay
{
    private readonly string _input;
    private readonly int[] _numberList;

    public Day_01()
    {
        _input = File.ReadAllText(InputFilePath);
        _numberList = _input.Split("\n").Select(int.Parse).ToArray();
    }

    public override ValueTask<string> Solve_1() => new(Solve_1_Sync());

    private string Solve_1_Sync()
        => Count(_numberList);

    public override ValueTask<string> Solve_2() => new(Solve_2_Sync());

    private string Solve_2_Sync()
        => Count(_numberList.Take(_numberList.Length -2).Select((_, index) => GeListForIndex(index))).ToString();

    private string Count(IEnumerable<int> numberList)
        => numberList.Aggregate<int, (int Count, int? Previous)>((Count: 0, Previous: null), (acc, current) =>
        {
            if (acc.Previous.HasValue && acc.Previous < current)
            {
                return (acc.Count + 1, current);
            }

            return (acc.Count, current);
        }).Count.ToString();

    private int GeListForIndex(int index)
        => _numberList[index..(index + 3)].Sum();
}