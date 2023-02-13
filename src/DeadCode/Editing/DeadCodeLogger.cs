using DeadCode.Syntax;
using System.IO;
using System.Threading.Tasks;

namespace DeadCode.Editing;

public static class DeadCodeLogger
{
    public static TextWriter Writer { get; set; } = Console.Out;

    public static Task Apply(CodeBase codeBase)
    {
        foreach (var code in codeBase.Code.ToLog())
        {
            Writer.WriteLine();
            Writer.Write("* ");
            Writer.Write(code.Symbol);
            Writer.WriteLine(code.Status());
            foreach (var usedBy in code.UsedBy.ToLog())
            {
                Writer.Write("  - ");
                Writer.Write(usedBy.Symbol);
                Writer.WriteLine(code.Status());
            }
        }

        return Task.CompletedTask;
    }

    static string Status(this Code code)
    {
        if (code.IsEntryPoint)
        {
            return " (ENTRY POINT)";
        }
        else if (code.IsDead)
        {
            return " (DEAD)";
        }
        else return " (USED)";
    }

    static IEnumerable<Code> ToLog(this IEnumerable<Code> code)
        => code
        .OrderByDescending(c => c.Node is { })
        .ThenBy(c => c.Symbol.ToString());
}
