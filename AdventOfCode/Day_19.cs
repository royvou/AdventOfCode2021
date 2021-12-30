using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_19 : BaseDay
{
    private readonly string _input;
    private readonly Scanner[] _scanners;

    public Day_19()
    {
        _input = File.ReadAllText(InputFilePath);
        _scanners = _input.SplitDoubleNewLine().Select(ParseScanner).ToArray();
    }

    private Scanner ParseScanner(string arg)
    {
        var lines = arg.SplitNewLine();
        var name = Regex.Match(lines[0], @"--- scanner (\d+) ---").Groups[1].Value;
        var vectors = lines.Skip(1).Select(x =>
        {
            var pos = x.Split(",");
            return new Vector3(pos[0].AsInt(), pos[1].AsInt(), pos[2].AsInt());
        }).ToList();

        return new Scanner(name, vectors);
    }


    public override ValueTask<string> Solve_1()
    {
        var (scanner, _) = ResolveMap();
        return new ValueTask<string>(scanner.Beacons.Count.ToString());
    }

    private (Scanner FinalScanner, Vector3[] ScannerOffsets) ResolveMap()
    {
        var scannersCount = _scanners.Length;
        var scannersWithOrientation = _scanners.Select(static scanner => ScannerWithOrientations.FromScanner(scanner)).ToArray();
        var orientation = new Scanner[scannersCount];
        var position = new Vector3?[scannersCount];

        // Resolve 1
        orientation[0] = scannersWithOrientation[0].Variations[0];
        position[0] = new Vector3(0, 0, 0);

        var frontier = new Queue<int>();
        frontier.Enqueue(0);

        while (frontier.TryDequeue(out var currentNextToCheck))
        {
            for (var i = 0; i < scannersCount; i++)
            {
                // Did we resolve this one already?
                if (position[i] != default)
                {
                    continue;
                }

                // Check if fingerprint between these 2 options match, if this match, do the "Real' match
                var current = scannersWithOrientation[currentNextToCheck];
                var toCheck = scannersWithOrientation[i];
                if (!current.FingerprintMatch(toCheck))
                {
                    continue;
                }

                // Real match with changing orientation
                var match = orientation[currentNextToCheck].Match(toCheck);
                if (!match.HasValue)
                {
                    continue;
                }

                orientation[i] = match.Value.Item1; // correct orientation!
                position[i] = position[currentNextToCheck] + match.Value.Offset;

                frontier.Enqueue(i);
            }
        }

        var result = new Scanner("Final Map", new List<Vector3>(scannersWithOrientation[0].Variations[0].Beacons.Count * scannersWithOrientation.Length));
        for (var i = 0; i < scannersWithOrientation.Length; i++)
        {
            result.AddUniqueBeacons(orientation[i].Beacons, position[i].Value);
        }

        return (result, Array.ConvertAll(position, value => value.Value));
    }
    
    public override ValueTask<string> Solve_2()
    {
        var (_, positions) = ResolveMap();

        var maxManhatten = long.MaxValue;
        for (var x = 0; x < positions.Length; x++)
        {
            for (var y = x + 1; y < positions.Length; y++)
            {
                var result = positions[x].Manhatten(positions[y]);
                if (result < maxManhatten)
                {
                    maxManhatten = result;
                }
            }
        }

        /*
        var maxManhatten = positions
            .SelectMany((value, i) => positions[(i + 1)..], (one, two) => (one, two))
            .Select(x => x.one.Manhatten(x.two))
            .Max();
        */
        return new ValueTask<string>(maxManhatten.ToString());
    }
}

public readonly record struct Vector3(int X, int Y, int Z) : IEquatable<Vector3>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Manhatten(Vector3 other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(Vector3 a) => new(-a.X, -a.Y, -a.Z);
}

public readonly record struct ScannerWithOrientations(Scanner[] Variations, int[][] Fingerprint)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static IEnumerable<Func<Vector3, Vector3>> GetTransforms()
    {
        yield return v => new Vector3(v.X, v.Y, v.Z);
        yield return v => new Vector3(v.X, -v.Y, -v.Z);
        yield return v => new Vector3(v.X, -v.Z, v.Y);
        yield return v => new Vector3(v.X, v.Z, -v.Y);

        yield return v => new Vector3(-v.X, -v.Y, v.Z);
        yield return v => new Vector3(-v.X, v.Y, -v.Z);
        yield return v => new Vector3(-v.X, -v.Z, -v.Y);
        yield return v => new Vector3(-v.X, v.Z, v.Y);

        yield return v => new Vector3(-v.Y, v.X, v.Z);
        yield return v => new Vector3(v.Z, v.X, v.Y);
        yield return v => new Vector3(v.Y, v.X, -v.Z);
        yield return v => new Vector3(-v.Z, v.X, -v.Y);

        yield return v => new Vector3(v.Y, -v.X, v.Z);
        yield return v => new Vector3(v.Z, -v.X, -v.Y);
        yield return v => new Vector3(-v.Y, -v.X, -v.Z);
        yield return v => new Vector3(-v.Z, -v.X, v.Y);

        yield return v => new Vector3(-v.Z, v.Y, v.X);
        yield return v => new Vector3(v.Y, v.Z, v.X);
        yield return v => new Vector3(v.Z, -v.Y, v.X);
        yield return v => new Vector3(-v.Y, -v.Z, v.X);

        yield return v => new Vector3(-v.Z, -v.Y, -v.X);
        yield return v => new Vector3(-v.Y, v.Z, -v.X);
        yield return v => new Vector3(v.Z, v.Y, -v.X);
        yield return v => new Vector3(v.Y, -v.Z, -v.X);
    }

    public static ScannerWithOrientations FromScanner(Scanner sc)
    {
        var result = new List<Scanner>();
        foreach (var transform in GetTransforms())
        {
            var transformed = sc.Beacons.Select(beacon => transform(beacon)).ToList();
            var @new = new Scanner(sc.Name, transformed);
            result.Add(@new);
        }

        var fingerprint = GenerateFingerprint(result[0].Beacons);

        return new ScannerWithOrientations(result.ToArray(), fingerprint);
    }

    private static int[][] GenerateFingerprint(IList<Vector3> beacons)
    {
        var beaconsCount = beacons.Count;
        var fingerprint = new int[beaconsCount][];
        for (var i = 0; i < beaconsCount; i++)
        {
            fingerprint[i] = new int[beaconsCount];
            for (var j = 0; j < beaconsCount; j++)
            {
                fingerprint[i][j] = beacons[i].Manhatten(beacons[j]);
            }

            Array.Sort(fingerprint[i]);
        }

        return fingerprint;
    }

    public bool FingerprintMatch(in ScannerWithOrientations other)
    {
        var beaconsCount = Variations[0].Beacons.Count;
        var otherBeaconsCount = other.Variations[0].Beacons.Count;

        for (var x = 0; x < beaconsCount; x++)
        {
            var currentFingerprint = Fingerprint[x];
            for (var y = 0; y < otherBeaconsCount; y++)
            {
                var currentOtherFingerprint = other.Fingerprint[y];
                var currentFingerprintIndex = 0;
                var currentOtherFingerprintIndex = 0;
                var matches = 0;
                while (currentFingerprintIndex < currentFingerprint.Length && currentOtherFingerprintIndex < currentOtherFingerprint.Length)
                {
                    // Early exit if we are unable to match due to no X spots left
                    if (matches + currentFingerprint.Length - currentFingerprintIndex < 12)
                    {
                        break;
                    }

                    if (currentFingerprint[currentFingerprintIndex] == currentOtherFingerprint[currentOtherFingerprintIndex])
                    {
                        currentFingerprintIndex++;
                        currentOtherFingerprintIndex++;
                        matches++;
                        if (matches >= 12)
                        {
                            return true;
                        }
                    }
                    else if (currentFingerprint[currentFingerprintIndex] > currentOtherFingerprint[currentOtherFingerprintIndex])
                    {
                        currentOtherFingerprintIndex++;
                    }
                    else if (currentFingerprint[currentFingerprintIndex] < currentOtherFingerprint[currentOtherFingerprintIndex])
                    {
                        currentFingerprintIndex++;
                    }
                }
            }
        }

        return false;
    }
}

public readonly record struct Scanner(string Name, List<Vector3> Beacons)
{
    private Vector3? Test(in Scanner other)
    {
        var beacons = Beacons;
        var otherBeacons = other.Beacons;

        var beaconsCount = beacons.Count;
        var otherBeaconsCount = otherBeacons.Count;

        // Allow faster compare
        HashSet<Vector3> beaconComparer = new(beacons);

        for (var x = 0; x < beacons.Count; x++)
        {
            var myBeacon = beacons[x];
            for (var y = 0; y < otherBeacons.Count; y++)
            {
                var otherBeacon = otherBeacons[y];
                var delta = otherBeacon - myBeacon;

                var matches = 0;
                for (var currentChecked = 0; currentChecked < otherBeaconsCount; currentChecked++)
                {
                    // Failure: Can never reach enough matches
                    if (matches + otherBeaconsCount - currentChecked < 12)
                    {
                        break;
                    }

                    var otherBeacon2 = otherBeacons[currentChecked];
                    var toCheck = otherBeacon2 - delta;
                    if (beaconComparer.Contains(toCheck))
                    {
                        matches++;
                    }

                    // Success: Enough matches
                    if (matches >= 12)
                    {
                        return -delta;
                    }
                }
            }
        }

        return null;
    }

    public (Scanner MatchedScannerVariant, Vector3 Offset)? Match(in ScannerWithOrientations other)
    {
        var otherVariationsLength = other.Variations.Length;
        for (var i = 0; i < otherVariationsLength; i++)
        {
            var variation = other.Variations[i];
            var match = Test(variation);
            if (match.HasValue)
            {
                return (variation, match.Value);
            }
        }

        return null;
    }

    public void AddUniqueBeacons(in List<Vector3> beacons, in Vector3 offset)
    {
        for (var i = 0; i < beacons.Count; i++)
        {
            var beacon = beacons[i];
            var modifiedPosition = beacon + offset;
            if (!Beacons.Contains(modifiedPosition))
            {
                Beacons.Add(modifiedPosition);
            }
        }
    }
}