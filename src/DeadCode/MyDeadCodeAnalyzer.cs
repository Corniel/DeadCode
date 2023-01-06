using DeadCode.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DeadCode
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MyDeadCodeAnalyzer : DeadCodeAnalyzer
    {
    }
}
