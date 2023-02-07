using System.Threading.Tasks;

namespace DeadCode.App;

static internal class Program
{
    static Task Main(string[] args) => DeadCodeAnalyzer.Run(args);
}
