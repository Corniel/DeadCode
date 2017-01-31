using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DeadCode.CodeAnalysis
{
	public abstract class DeadCodeWalker
	{
		protected DeadCodeWalker(CodeParts parts)
		{
			Parts = Guard.NotNull(parts, nameof(parts));
		}

		public CodeParts Parts { get; }

		protected SemanticModel Model { get; set; }
		public void Visit(SyntaxNode node, SemanticModel model)
		{
			Model = Guard.NotNull(model, nameof(model));
			Visit(node);
		}
		public abstract void Visit(SyntaxNode node);
	}

	public abstract class DeadCodeWalker<ClassDeclarationSyntaxType, IdentifierSyntaxType> : DeadCodeWalker
		where ClassDeclarationSyntaxType : SyntaxNode
		where IdentifierSyntaxType : SyntaxNode
	{
		protected DeadCodeWalker(CodeParts parts) : base(parts) { }

		public override void Visit(SyntaxNode node)
		{
			if (node is ClassDeclarationSyntaxType)
			{
				VisitClass((ClassDeclarationSyntaxType)node);
			}

		}
		public void VisitClass(ClassDeclarationSyntaxType node)
		{
			var symbol = Model.GetDeclaredSymbol(node);
			var cls = Parts.GetClass(symbol);
			var nodes = node.ChildNodes().ToList();
			//VisitConstructorDeclerations(node, cls);
			VisitPropertyDeclarations(node, cls);
			VisitMethodDeclerations(node, cls);
		}
		protected void VisitConstructorDeclerations(ClassDeclarationSyntaxType node, CodeClass cls)
		{
			foreach (var child in GetConstructors(node))
			{
				var symbol = Model.GetSymbolInfo(child);
				//var prop = Parts.GetProperty(cls, symbol);
				//VisitChildren(child, meth);
			}
		}
		protected void VisitPropertyDeclarations(ClassDeclarationSyntaxType node, CodeClass cls)
		{
			foreach (var child in GetChildProperties(node))
			{
				var symbol = (IPropertySymbol)Model.GetDeclaredSymbol(child);
				var prop = Parts.GetProperty(cls, symbol);
				VisitChildren(child, prop);
			}
		}

		protected void VisitMethodDeclerations(ClassDeclarationSyntaxType node, CodeClass cls)
		{
			foreach (var child in GetChildMethods(node))
			{
				var symbol = (IMethodSymbol)Model.GetDeclaredSymbol(child);
				CodeMethod meth = Parts.GetMethod(cls, symbol);
				VisitChildren(child , meth);
			}
		}

		private void VisitChildren(SyntaxNode root, CodeMember caller)
		{
			foreach(var child in root.ChildNodes().Where(ch => !(ch is IdentifierSyntaxType)))
			{
				VisitImplementation(child, caller);
			}
		}

		private void VisitImplementation(SyntaxNode node, CodeMember caller)
		{
			var identifier = node as IdentifierSyntaxType;
			if (identifier != null)
			{
				AddCallTo(identifier, caller);
			}
			foreach (var child in node.ChildNodes())
			{
				VisitImplementation(child, caller);
			}
		}

		private void AddCallTo(IdentifierSyntaxType node, CodeMember caller)
		{
			var symbol = Model.GetSymbolInfo(node).Symbol;

			if (symbol?.ContainingType == null) { return; }

			var cls = Parts.TryGetClass(symbol.ContainingType);

			if (symbol is IPropertySymbol)
			{
				var prop = Parts.GetProperty(cls, (IPropertySymbol)symbol);
				caller.AddCallTo(prop);
			}
			else if (symbol is IMethodSymbol)
			{
				var ms = (IMethodSymbol)symbol;
				if (ms.MethodKind == MethodKind.PropertyGet || ms.MethodKind == MethodKind.PropertySet)
				{
					var prop = (IPropertySymbol)ms.AssociatedSymbol;
					caller.AddCallTo(Parts.GetProperty(cls, prop));
				}
				else
				{
					var meth = Parts.GetMethod(cls, ms);
					caller.AddCallTo(meth);
				}
			}
		}


		protected abstract IEnumerable<SyntaxNode> GetConstructors(ClassDeclarationSyntaxType node);
		protected abstract IEnumerable<SyntaxNode> GetChildProperties(ClassDeclarationSyntaxType node);
		protected abstract IEnumerable<SyntaxNode> GetChildMethods(ClassDeclarationSyntaxType node);
	}
}
