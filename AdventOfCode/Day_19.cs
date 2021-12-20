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
        }).ToHashSet();

        return new Scanner(name, vectors);
    }

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

    public override ValueTask<string> Solve_1()
    {
        var (finalMap, _) = GetFinalMap();
        return new ValueTask<string>(finalMap.Scans.Count.ToString());
    }

    private (Scanner finalScanner, List<Vector3> partScanner) GetFinalMap()
    {
        var scannersToDo = new Queue<Scanner>(_scanners);
        var finalScanner = new Scanner("Final", new HashSet<Vector3>());
        var scannersDone = new List<Vector3>();

        finalScanner.Scans.UnionWith(scannersToDo.Dequeue().Scans);
        while (scannersToDo.TryDequeue(out var currentScanner))
        {
            var result = TryAlignScannerToMap(finalScanner, currentScanner);
            if (result.HasValue)
            {
                var (aligned, transform) = result.Value;
                finalScanner.Scans.UnionWith(aligned.Scans);
                scannersDone.Add(transform);
            }
            else
            {
                scannersToDo.Enqueue(currentScanner);
            }
        }

        return (finalScanner, scannersDone);
    }

    private (Scanner, Vector3)? TryAlignScannerToMap(Scanner finalScanner, Scanner currentScanner)
    {
        foreach (var transform in GetTransforms())
        {
            var transformedScans = currentScanner with
            {
                Scans = currentScanner.Scans.Select(x => transform(x)).ToHashSet(),
            };

            foreach (var scan in finalScanner.Scans)
            {
                foreach (var transformedScan in transformedScans.Scans)
                {
                    var delta = scan.Delta(transformedScan);
                    //Try translate all based on this scan
                    var movedTransformedScan = transformedScans.Translate(delta);

                    var tmp = new HashSet<Vector3>();
                    tmp.UnionWith(movedTransformedScan.Scans);
                    tmp.IntersectWith(finalScanner.Scans);
                    if (tmp.Count >= 12)
                    {
                        return (movedTransformedScan, delta);
                    }
                }
            }
        }

        return null;
    }


    public override ValueTask<string> Solve_2()
    {
        var (_, scanners) = GetFinalMap();

        var maxManhatten = scanners
            .SelectMany((value, i) => scanners.Skip(1 + i), (one, two) => (one, two))
            .Select(x => x.one.Manhatten(x.two))
            .Max();

        return new ValueTask<string>(maxManhatten.ToString());
    }
}

internal record struct Vector3(int X, int Y, int Z)
{
    public Vector3 Translate(Vector3 translate) => this with
    {
        X = X + translate.X,
        Y = Y + translate.Y,
        Z = Z + translate.Z,
    };

    public Vector3 Delta(Vector3 translate) => this with
    {
        X = X - translate.X,
        Y = Y - translate.Y,
        Z = Z - translate.Z,
    };

    public int Manhatten(Vector3 target)
    {
        var (x, y, z) = Delta(target);
        return Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
    }
}

internal record Scanner(string Name, HashSet<Vector3> Scans)
{
    public Scanner Translate(Vector3 translate) => this with
    {
        Scans = Scans.Select(x => x.Translate(translate)).ToHashSet(),
    };
}