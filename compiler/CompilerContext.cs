using System;
using Andy.TinyOS.Parser;

namespace Andy.TinyOS.Compiler
{
	public class CompilerContext
	{
		public CompilerContext Parent { get; set; }
		public ParseNode Node { get; set; }
		public CodeStream Code { get; set; }

		private Func<CompilerContext, CodeStream> _compiler;

		public Func<CompilerContext, CodeStream> Compiler
		{
			get { return _compiler ?? Parent.Compiler; }
			set { _compiler = value; }
		}

		public CompilerContext Push(ParseNode node)
		{
			if ( node == null )
				throw new ArgumentNullException("node");

			return new CompilerContext
			{
				Code = new CodeStream(),
				Node = node,
				Parent = this
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