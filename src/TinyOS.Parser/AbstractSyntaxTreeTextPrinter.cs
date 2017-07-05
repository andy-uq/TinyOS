using System;
using System.Collections.Generic;

namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// A base class that contains common functionality for printing 
	/// an AST tree to text.
	/// </summary>
	public abstract class AbstractSyntaxTreeTextPrinter
		: IAbstractSyntaxTreePrinter
	{
		private readonly List<string> _lines = new List<string>();
		protected string Indent { get; set; }

		#region IAstPrinter Members

		public void Clear()
		{
			_lines.Clear();
		}

		public void Print(ParseNode node)
		{
			Add(node, 0);
		}

		protected abstract void Add(ParseNode node, int depth);

		#endregion

		public String[] GetStrings()
		{
			return _lines.ToArray();
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, _lines.ToArray());
		}

		/// <summary>
		/// Puts the string 
		/// </summary>
		/// <param name="s"></param>
		protected void WriteLine(string s = "")
		{
			_lines.Add(Indent + s);
		}

		/// <summary>
		/// Used to set a single breakpoint for an assertion failure. Facilitates debugging.  
		/// </summary>
		/// <param name="condition"></param>
		protected void Assert(bool condition)
		{
			if (!condition)
				throw new Exception("Assertion failed");
		}
	}
}