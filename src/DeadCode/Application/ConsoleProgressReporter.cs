using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;

namespace DeadCode.App;

internal class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
{
    public void Report(ProjectLoadProgress loadProgress)
    {
        var projectDisplay = Path.GetFileName(loadProgress.FilePath);
        
        if (loadProgress.TargetFramework is { })
        {
            projectDisplay += $" ({loadProgress.TargetFramework})";
        }
        Console.WriteLine($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
    }
}