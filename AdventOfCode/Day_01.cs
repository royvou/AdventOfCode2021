using AoCHelper;

namespace AdventOfCode
{
    public class Day_01 : BaseDay
    {
        private readonly string _input;
        private readonly List<int> _numberList;

        public Day_01()
        {
            _input = File.ReadAllText(InputFilePath);            
            _numberList = _input.Split("\n").Select(int.Parse).ToList();

        }

        public override ValueTask<string> Solve_1() => new(Solve_1_Sync());
        
        private string Solve_1_Sync()
        {
            return _numberList.Aggregate<int, (int Count, int? Previous)>((Count: 0, Previous: null), (acc, current) =>
            {
                if (acc.Previous.HasValue && acc.Previous < current)
                    return (acc.Count + 1, current);
                return (acc.Count, current);
            }).Count.ToString();
        }
        public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }
}
