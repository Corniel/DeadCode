using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class DeadCodeCSharpWalker : DeadCodeWalker<ClassDeclarationSyntax, IdentifierNameSyntax>
	{
		public DeadCodeCSharpWalker(CodeParts parts) : base(parts) { }

		protected override IEnumerable<SyntaxNode> GetConstructors(ClassDeclarationSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is ConstructorDeclarationSyntax);
		}
		protected override IEnumerable<SyntaxNode> GetChildProperties(ClassDeclarationSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is PropertyDeclarationSyntax);
		}

		protected override IEnumerable<SyntaxNode> GetChildMethods(ClassDeclarationSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is MethodDeclarationSyntax);
		}
	}
}
