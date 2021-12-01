if (Environment.GetEnvironmentVariable("DOTNET_WATCH") != default)
{
    Solver.SolveLast();
    return;
}

switch (args.Length)
{
    case 0:
        Solver.SolveLast();
        break;
    case 1 when args[0].Contains("all", System.StringComparison.CurrentCultureIgnoreCase):
        Solver.SolveAll();
        break;
    default:
    {
        var indexes = args.Select(arg => uint.TryParse(arg, out var index) ? index : uint.MaxValue);

        Solver.Solve(indexes.Where(i => i < uint.MaxValue));
        break;
    }
}
