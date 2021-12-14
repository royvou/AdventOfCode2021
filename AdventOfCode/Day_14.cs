namespace AdventOfCode;

public class Day_14 : BaseDay
{
    private readonly char[] _initialList;
    private readonly string _input;
    private readonly Dictionary<(char, char), char> _insertionMapping;

    public Day_14()
    {
        _input = File.ReadAllText(InputFilePath);
        var splitGroups = _input.SplitDoubleNewLine();

        _initialList = splitGroups[0].ToCharArray();
        _insertionMapping = splitGroups[1].SplitNewLine().ToDictionary(x => (x[0], x[1]), y => y[^1]);
    }

    public override ValueTask<string> Solve_1()
        => new(ExecuteLoops(10).ToString());

    private long ExecuteLoops(int loopCount)
    {
        Dictionary<(char, char), long> dictionary = new();
        InitializeDictionary(dictionary);

        for (var i = 0; i < loopCount; i++)
        {
            Dictionary<(char, char), long> nextDictionary = new();

            foreach (var dictionaryItem in dictionary)
            {
                // In case there is no mapping (likely the last empty value
                if (!_insertionMapping.TryGetValue(dictionaryItem.Key, out var toInsert))
                {
                    nextDictionary[dictionaryItem.Key] = nextDictionary.GetValueOrDefault(dictionaryItem.Key, 0) + dictionaryItem.Value;
                    continue;
                }

                var key = (dictionaryItem.Key.Item1, toInsert);
                nextDictionary[key] = nextDictionary.GetValueOrDefault(key, 0) + dictionaryItem.Value;

                var key2 = (toInsert, dictionaryItem.Key.Item2);
                nextDictionary[key2] = nextDictionary.GetValueOrDefault(key2, 0) + dictionaryItem.Value;
            }


            dictionary = nextDictionary;
        }

        var characterCount = CountCharacters(dictionary);

        return characterCount.Max(x => x.Value) - characterCount.Min(x => x.Value);
    }

    private IDictionary<char, long> CountCharacters(Dictionary<(char, char), long> dictionary)
    {
        var result = new Dictionary<char, long>();
        foreach (var item in dictionary)
        {
            result[item.Key.Item1] = result.GetValueOrDefault(item.Key.Item1, 0) + item.Value;
        }

        return result;
    }

    private void InitializeDictionary(Dictionary<(char, char), long> dictionary)
    {
        for (var i = 0; i < _initialList.Length - 1; i++)
        {
            var key = (_initialList[i], _initialList[i + 1]);
            dictionary[key] = dictionary.GetValueOrDefault(key, 0) + 1;
        }

        // Add last character with no next :)
        dictionary[(_initialList[^1], default)] = 1;
    }

    public override ValueTask<string> Solve_2()
        => new(ExecuteLoops(40).ToString());
}