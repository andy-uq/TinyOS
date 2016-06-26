using System;
using Andy.TinyOS.Parser;

namespace Andy.TinyOS.Compiler
{
	public class CompilerContext
	{
		public CompilerContext Parent { get; set; }
		public ParseNode Node { get; set; }
		public CodeStream Code { get; set; }

		public SymbolTable SymbolTable
		{
			get { return _symbolTable ?? Parent.SymbolTable; }
			set { _symbolTable = value; }
		}

		private SymbolTable _symbolTable;
		private Func<CompilerContext, CodeStream> _compiler;

		public Func<CompilerContext, CodeStream> Compiler
		{
			get { return _compiler ?? Parent.Compiler; }
			set { _compiler = value; }
		}

		private OpCode _jumpExpression;
		public OpCode JumpExpression
		{
			get { return _jumpExpression; }
			set
			{
				_jumpExpression = value;
				if ( Parent != null )
					Parent.JumpExpression = value;
			}
		}

		public CompilerContext Push(ParseNode node)
		{
			if ( node == null )
				throw new ArgumentNullException(nameof(node));

			return new CompilerContext
			{
				Code = new CodeStream(),
				Node = node,
				Parent = this,
				JumpExpression = OpCode.Noop,
			};
		}

		public CodeStream Compile(ParseNode node)
		{
			var child = Push(node);
			Code += Compiler(child);

			return Code;
		}
	}

	public static class FluentExtensions
	{
		public static FluentWriter AsFluent(this CompilerContext context)
		{
			return new FluentWriter(context.Code);
		}
	}
}