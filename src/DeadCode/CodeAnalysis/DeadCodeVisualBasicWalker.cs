using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DeadCode.CodeAnalysis
{
	public class DeadCodeVisualBasicWalker : DeadCodeWalker<ClassBlockSyntax, IdentifierNameSyntax>
	{
		public DeadCodeVisualBasicWalker(CodeParts parts) : base(parts) { }

		protected override IEnumerable<SyntaxNode> GetConstructors(ClassBlockSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is ConstructorBlockSyntax);
		}

		protected override IEnumerable<SyntaxNode> GetChildProperties(ClassBlockSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is PropertyStatementSyntax || ch is PropertyBlockSyntax);
		}

		protected override IEnumerable<SyntaxNode> GetChildMethods(ClassBlockSyntax node)
		{
			return node.ChildNodes().Where(ch => ch is MethodBlockSyntax);
		}
	}
}
