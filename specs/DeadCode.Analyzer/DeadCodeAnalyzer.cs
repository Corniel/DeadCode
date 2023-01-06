using DeadCode.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadCode.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeadCodeAnalyzer : CodeAnalysis.DeadCodeAnalyzer
{

}