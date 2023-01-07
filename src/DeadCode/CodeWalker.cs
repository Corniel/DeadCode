using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace DeadCode;

public class CodeWalker : CSharpSyntaxWalker
{
    public CodeWalker(CodeBase codeBase)
    {
        CodeBase = codeBase;
    }

    public CodeBase CodeBase { get; }
    public SemanticModel? Model { get; private set; }

    public void Visit(SyntaxNode node, SemanticModel model)
    {
        Model = Guard.NotNull(model, nameof(model));
        Visit(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } type)
        {
            CodeBase.SetIdentifier(type, node.Identifier);
        }
        base.VisitClassDeclaration(node);
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } ctor)
        {
            CodeBase.SetIdentifier(ctor, node.Identifier);
        }
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } method)
        {
            CodeBase.SetIdentifier(method, node.Identifier);
        }
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (Model.GetDeclaredSymbol(node) is { } property)
        {
            CodeBase.SetIdentifier(property, node.Identifier);
        }
        base.VisitPropertyDeclaration(node);
    }
}
