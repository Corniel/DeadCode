using CommandLine;

namespace DeadCode.Application;

public sealed class DeadCodeAnalyzerOptions
{
    [Option('s', "solution", Required = true, HelpText = "Solution file to be processed.")]
    public string Solution { get; init; } = string.Empty;
}
