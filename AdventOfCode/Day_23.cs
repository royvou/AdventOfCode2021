namespace AdventOfCode;

public class Day_23 : BaseDay
{
    private readonly AmphipodBurrow _amphipodBurrow1;
    private readonly AmphipodBurrow _amphipodBurrow2;
    private readonly string _input;


    public Day_23()
    {
        _input = File.ReadAllText(InputFilePath);

        var inputLines = _input.SplitNewLine();
        _amphipodBurrow1 = AmphipodBurrow.Parse(inputLines);
        string[] additionalLines =
        {
            "  #D#C#B#A#",
            "  #D#B#A#C#",
        };
        _amphipodBurrow2 = AmphipodBurrow.Parse(inputLines.Take(3).Concat(additionalLines).Concat(inputLines.Skip(3)));
    }

    public override ValueTask<string> Solve_1()
    {
        var A = 2 + 9 + 3 + 8;
        var B = 6 + 4;
        var C = 2 + 3 + 7;
        var D = 7 + 7;

        var score = A * 1 + B * 10 + C * 100 + D * 1000;
        return new ValueTask<string>(score.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        AmphipodBurrowOrganizer organizer = new(_amphipodBurrow2);
        var destinationState = organizer.StartState with { Rooms = string.Concat(organizer.StartState.Rooms.OrderBy(c => c)), };
        var result = organizer.CalculateMinEnergyRequired(destinationState);
        return new ValueTask<string>(result.ToString());
    }
}

public enum AmphipodType : ushort
{
    Amber = 'A',
    Bronze = 'B',
    Copper = 'C',
    Desert = 'D',
}

public record Amphipod(AmphipodType Type)
{
    public static Amphipod FromChar(char c)
        => c == '.' ? null : new Amphipod((AmphipodType)c);

    public int GetEnergyPerStep() => Type switch
    {
        AmphipodType.Amber => 1,
        AmphipodType.Bronze => 10,
        AmphipodType.Copper => 100,
        AmphipodType.Desert => 1000,
        _ => throw new ArgumentException($"No amphipod with character representation '{Type}'.", nameof(Type)),
    };
}

public class AmphipodBurrow
{
    public AmphipodBurrow(IReadOnlyList<Amphipod?> corridor, IReadOnlyList<IReadOnlyList<Amphipod?>> rooms, int roomSize)
    {
        Corridor = corridor;
        Rooms = rooms;
        RoomSize = roomSize;
    }

    public static int RoomCount { get; } = 4;
    public static IReadOnlyList<int> RoomPositions { get; } = new[] { 2, 4, 6, 8, };

    public static IReadOnlyList<AmphipodType> RoomTypes { get; } = new[]
    {
        AmphipodType.Amber,
        AmphipodType.Bronze,
        AmphipodType.Copper,
        AmphipodType.Desert,
    };

    public IReadOnlyList<Amphipod?> Corridor { get; }
    public IReadOnlyList<IReadOnlyList<Amphipod?>> Rooms { get; }
    public int RoomSize { get; }

    public AmphipodBurrowState State
    {
        get
        {
            var hallStr = string.Concat(Corridor.Select(amphipod => (char?)amphipod?.Type ?? '.'));
            var roomsStr = string.Concat(
                Rooms.Select(room =>
                    string.Concat(
                        room.Select(amphipod => (char?)amphipod?.Type ?? '.')
                    )
                )
            );
            return new AmphipodBurrowState(hallStr, roomsStr, RoomSize);
        }
    }

    public static AmphipodBurrow Parse(IEnumerable<string> lines)
    {
        const int NumberOfRooms = 4;
        const int CorridorLength = 11;
        Action<bool> AssertFormat = condition =>
        {
            if (!condition)
            {
                throw new FormatException("Input was in an invalid format.");
            }
        };
        var it = lines.GetEnumerator();
        AssertFormat(it.MoveNext());
        AssertFormat(it.Current.All(c => c == '#'));
        AssertFormat(it.MoveNext());
        var hall = it.Current.Trim('#');
        AssertFormat(hall.Length == CorridorLength && hall.All(c => c == '.'));
        var rooms = new string[NumberOfRooms] { "", "", "", "", };
        while (it.MoveNext() && !it.Current.Trim().All(c => c == '#'))
        {
            var roomLevel = it.Current.Trim().Trim('#').Split('#');
            AssertFormat(roomLevel.Length == NumberOfRooms && roomLevel.All(r => r.Length == 1));
            for (var i = 0; i < NumberOfRooms; i++)
            {
                rooms[i] += roomLevel[i];
            }
        }

        AssertFormat(it.Current.Trim().All(c => c == '#'));
        var roomSize = rooms[0].Length;
        AssertFormat(rooms.All(r => r.Length == roomSize));
        return new AmphipodBurrow(
            hall.Select(Amphipod.FromChar).ToList(),
            rooms.Select(room => room.Select(Amphipod.FromChar).ToList()).ToList(),
            roomSize
        );
    }
}

public readonly record struct AmphipodBurrowState(string Hall, string Rooms, int RoomSize)
{
    private static readonly Lazy<char[]> _amphipodTypes = new(() => { return AmphipodBurrow.RoomTypes.Select(t => (char)t).ToArray(); });

    public static char[] AmphipodTypes => _amphipodTypes.Value;

    public string GetRoom(int index) => Rooms.Substring(index * RoomSize, RoomSize);

    public IEnumerable<(AmphipodBurrowState state, int cost)> NextPossibleStates()
    {
        for (var i = 0; i < AmphipodBurrow.RoomCount; i++)
        {
            foreach (var position in CorridorPositionAvailable(i))
            {
                if (TryMoveFromRoomToCorridor(i, position, out var nextState, out var amphipod, out var steps))
                {
                    var energy = steps * Amphipod.FromChar(amphipod).GetEnergyPerStep();
                    yield return (nextState, energy);
                }
            }
        }

        for (var i = 0; i < Hall.Length; i++)
        {
            if (TryMoveFromCorridorToRoom(i, out var nextState, out var amphipod, out var steps))
            {
                var energy = steps * Amphipod.FromChar(amphipod).GetEnergyPerStep();
                yield return (nextState, energy);
            }
        }
    }

    private bool TryMoveFromRoomToCorridor(int roomIndex, int targetPosition, out AmphipodBurrowState nextState, out char amphipod, out int steps)
    {
        var room = GetRoom(roomIndex);
        var positionInRoom = room.IndexOfAny(AmphipodTypes);
        if (positionInRoom < 0)
        {
            // Room was empty
            nextState = default;
            amphipod = default;
            steps = default;
            return false;
        }

        steps = Math.Abs(targetPosition - AmphipodBurrow.RoomPositions[roomIndex]) + positionInRoom + 1;
        amphipod = room[positionInRoom];

        var newHall = Hall.Remove(targetPosition, 1).Insert(targetPosition, amphipod.ToString());
        var newRoom = room.Remove(positionInRoom, 1).Insert(positionInRoom, ".");
        var newRooms = Rooms.Remove(roomIndex * RoomSize, RoomSize).Insert(roomIndex * RoomSize, newRoom);
        nextState = this with { Hall = newHall, Rooms = newRooms, };
        return true;
    }

    private bool TryMoveFromCorridorToRoom(int corridorPosition, out AmphipodBurrowState nextState, out char amphipod, out int steps)
    {
        nextState = default;
        steps = default;

        amphipod = Hall[corridorPosition];
        var destinationRoomIndex = Array.IndexOf(AmphipodTypes, amphipod);
        if (destinationRoomIndex < 0)
        {
            return false;
        }

        var destinationPosition = AmphipodBurrow.RoomPositions[destinationRoomIndex];
        var start = destinationPosition > corridorPosition ? corridorPosition + 1 : corridorPosition - 1;
        var min = Math.Min(destinationPosition, start);
        var max = Math.Max(destinationPosition, start);
        if (Hall.Skip(min).Take(max - min + 1).Any(ch => ch != '.'))
        {
            return false;
        }

        var room = GetRoom(destinationRoomIndex);
        var type = amphipod;
        if (room.Any(ch => ch != '.' && ch != type))
        {
            return false;
        }

        var depth = room.LastIndexOf('.');
        steps = max - min + 1 + depth + 1;

        var newHall = Hall.Remove(corridorPosition, 1).Insert(corridorPosition, ".");
        var newRoom = room.Remove(depth, 1).Insert(depth, amphipod.ToString());
        var newRooms = Rooms.Remove(destinationRoomIndex * RoomSize, RoomSize).Insert(destinationRoomIndex * RoomSize, newRoom);
        nextState = this with { Hall = newHall, Rooms = newRooms, };
        return true;
    }

    private IEnumerable<int> CorridorPositionAvailable(int startingRoomIndex)
    {
        var roomPosition = AmphipodBurrow.RoomPositions[startingRoomIndex];
        for (var i = roomPosition - 1; i >= 0; --i)
        {
            if (Hall[i] != '.')
            {
                break;
            }

            if (!AmphipodBurrow.RoomPositions.Contains(i))
            {
                yield return i;
            }
        }

        for (var i = roomPosition + 1; i < Hall.Length; ++i)
        {
            if (Hall[i] != '.')
            {
                break;
            }

            if (!AmphipodBurrow.RoomPositions.Contains(i))
            {
                yield return i;
            }
        }
    }
}

public class AmphipodBurrowOrganizer
{
    private readonly AmphipodBurrow _amphipodBurrow;

    public AmphipodBurrowOrganizer(AmphipodBurrow amphipodBurrow)
    {
        _amphipodBurrow = amphipodBurrow;
        StartState = _amphipodBurrow.State;
    }

    public AmphipodBurrowState StartState { get; }

    public int CalculateMinEnergyRequired(AmphipodBurrowState destinationState)
    {
        Dictionary<AmphipodBurrowState, int> cache = new() { [StartState] = 0, };
        HashSet<AmphipodBurrowState> visitedStates = new();
        PriorityQueue<AmphipodBurrowState, int> queue = new();
        queue.Enqueue(StartState, 0);

        while (queue.TryDequeue(out var current, out var currentTotalEnergy))
        {
            if (!visitedStates.Contains(current))
            {
                if (current.Equals(destinationState))
                {
                    return currentTotalEnergy;
                }

                foreach ((var nextState, var energy) in current.NextPossibleStates())
                {
                    var nextTotalEnergy = currentTotalEnergy + energy;
                    if (!cache.TryGetValue(nextState, out var previousTotalEnergy) || nextTotalEnergy < previousTotalEnergy)
                    {
                        cache[nextState] = nextTotalEnergy;
                        queue.Enqueue(nextState, nextTotalEnergy);
                    }
                }

                visitedStates.Add(current);
            }
        }

        throw new InvalidOperationException("No organization resulting in given destination state found.");
    }
}