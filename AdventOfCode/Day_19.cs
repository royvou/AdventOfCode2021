using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day_19 : BaseDay
{
    private readonly string _input;
    private readonly List<Scanner> _scanners;

    public Day_19()
    {
        _input = File.ReadAllText(InputFilePath);
        _scanners = _input.SplitDoubleNewLine().Select(ParseScanner).ToList();
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

        //var fingerprint = Scanner.GenerateFingerprint(vectors);

        return new Scanner(name, vectors);
    }


    public override ValueTask<string> Solve_1()
    {
        var scanner = ResolveMap();
        return new ValueTask<string>(scanner.Beacons.Count.ToString());
    }

    private Scanner ResolveMap()
    {
        var scannersCount = _scanners.Count;
        var scannersWithOrientation = _scanners.Select(scanner => ScannerWithOrientations.FromScanner(scanner)).ToArray();
        var orientation = new Scanner[scannersCount];
        var position = new Vector3?[scannersCount];

        orientation[0] = scannersWithOrientation[0].Variations[0];
        position[0] = new Vector3(0, 0, 0);

        var frontier = new Queue<int>();
        frontier.Enqueue(0);

        while (frontier.TryDequeue(out var front))
        {
            for (var i = 0; i < scannersCount; i++)
            {
                if (position[i].HasValue)
                {
                    continue;
                }

                var current = scannersWithOrientation[front];
                var toCheck = scannersWithOrientation[i];
                if (!current.FingerprintMatch(toCheck))
                {
                    continue;
                }

                var match = orientation[front].Match(toCheck);
                if (!match.HasValue)
                {
                    continue;
                }

                orientation[i] = match.Value.Item1; // correct orientation!
                position[i] = position[front].Value + match.Value.Item2;
                frontier.Enqueue(i);
            }
        }

        var result = new Scanner(orientation[0].Name, new List<Vector3>(orientation[0].Beacons));
        for (var i = 1; i < scannersWithOrientation.Length; i++)
        {
            result.AddBeacons(orientation[i].Beacons, position[i].Value);
        }

        Console.Write(result.ToString());
        return result;
    }

    public override ValueTask<string> Solve_2() => new("");
}

public readonly record struct Vector3(int X, int Y, int Z)
{
    public int Manhatteen(Vector3 other) => Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
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
            // FP is same with Transform :)
            var transformed = sc.Beacons.Select(beacon => transform(beacon)).ToList();
            // var transformedFp = Scanner.GenerateFingerprint(transformed);
            var @new = new Scanner(sc.Name, transformed);
            result.Add(@new);
        }

        var fingerprint = GenerateFingerprint(result[0].Beacons);

        return new ScannerWithOrientations(result.ToArray(), fingerprint);
    }

    public static int[][] GenerateFingerprint(IList<Vector3> beacons)
    {
        var beaconsCount = beacons.Count;
        var fingerprint = new int[beaconsCount][];
        for (var i = 0; i < beaconsCount; i++)
        {
            fingerprint[i] = new int[beaconsCount];
            for (var j = 0; j < beaconsCount; j++)
            {
                fingerprint[i][j] = beacons[i].Manhatteen(beacons[j]);
            }

            //Sort Array[i]
            Array.Sort(fingerprint[i]);
        }

        return fingerprint;
    }

    public bool FingerprintMatch(ScannerWithOrientations other)
    {
        var beaconsCount = Variations[0].Beacons.Count;
        var otherBeaconsCount = other.Variations[0].Beacons.Count;

        for (var i = 0; i < beaconsCount; i++)
        {
            for (var j = 0; j < otherBeaconsCount; j++)
            {
                var p1 = Fingerprint[i];
                var p2 = other.Fingerprint[j];
                // check if fingerprint matches
                var x = 0;
                var y = 0;
                var count = 0;
                while (x < p1.Length && y < p2.Length)
                {
                    if (p1[x] == p2[y])
                    {
                        x++;
                        y++;
                        count++;
                        if (count >= 12)
                        {
                            return true;
                        }
                    }
                    else if (p1[x] > p2[y])
                    {
                        y++;
                    }
                    else if (p1[x] < p2[y])
                    {
                        x++;
                    }
                }
            }
        }

        return false;
    }
}

public readonly record struct Scanner(string Name, List<Vector3> Beacons)
{
    private Vector3? Test(Scanner other)
    {
        var beaconsCount = Beacons.Count;
        var otherBeaconsCount = other.Beacons.Count;

        for (var i = 11; i < beaconsCount; i++)
        {
            for (var j = 0; j < otherBeaconsCount; j++)
            {
                var mine = Beacons[i];
                var their = other.Beacons[j];
                var relx = their.X - mine.X;
                var rely = their.Y - mine.Y;
                var relz = their.Z - mine.Z;
                var count = 0;
                for (var k = 0; k < beaconsCount; k++)
                {
                    if (count + beaconsCount - k < 12)
                    {
                        break; // not possible
                    }

                    for (var l = 0; l < otherBeaconsCount; l++)
                    {
                        var m = Beacons[k];
                        var n = other.Beacons[l];
                        if (relx + m.X != n.X || rely + m.Y != n.Y || relz + m.Z != n.Z)
                        {
                            continue;
                        }

                        count++;
                        if (count >= 12)
                        {
                            return new Vector3(relx, rely, relz);
                        }

                        break;
                    }
                }
            }
        }

        return null;
    }

    public (Scanner, Vector3)? Match(ScannerWithOrientations other)
    {
        foreach (var variation in other.Variations)
        {
            var match = Test(variation);
            if (match.HasValue)
            {
                return (variation, match.Value);
            }
        }

        return null;
    }

    public void AddBeacons(List<Vector3> beacons, Vector3 offset)
    {
        foreach (var beacon in beacons)
        {
            // new Vector3(beacon.X + offset.X, beacon.Y + offset.Y, beacon.Z + offset.Z);
            var modifiedPosition = beacon - offset;

            if (!Beacons.Contains(modifiedPosition))
            {
                Beacons.Add(modifiedPosition);
            }
        }
    }
}