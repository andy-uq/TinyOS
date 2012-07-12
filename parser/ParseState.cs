using System.Collections.Generic;
using System.Diagnostics;

namespace Andy.TinyOS.Parser
{
	public class ParserState
	{
		#region private fields

		private readonly Stack<ParseNode> _nodes = new Stack<ParseNode>();
		private readonly string _text;

		#endregion

		#region public fields

		public string Text { get { return _text; } }
		public int Index { get; set; }

		#endregion

		#region public properties

		/// <summary>
		/// Indicates whether nodes should be created. If set, 
		/// it will be applied recursively to all child nodes.
		/// </summary>
		public bool CreateNodes { get; set; }

		/// <summary>
		/// Outputs a number of characters before the current parser position.
		/// When debugging, this property helps us see where we are in the input stream.
		/// </summary>
		internal string DebugPrefixContext
		{
			get
			{
				int cnt = 25;
				int begin = Index - cnt;
				if (begin < 0)
				{
					begin = 0;
					cnt = Index - begin;
				}
				return "... " + Text.Substring(begin, cnt);
			}
		}

		/// <summary>
		/// Outputs a number of characters after the current parser position.
		/// When debugging, this property helps us see where we are in the input stream
		/// </summary>
		internal string DebugSuffixContext
		{
			get
			{
				int cnt = 25;
				if (Index + cnt > Text.Length)
					cnt = Text.Length - Index;
				return Text.Substring(Index, cnt) + " ... ";
			}
		}

		#endregion

		/// <summary>
		/// Constructs a parser state, that manages a pointer to the text.
		/// </summary>
		/// <param name="text"></param>
		public ParserState(string text)
		{
			CreateNodes = true;
			_text = text;
			var root = new ParseNode(null, null, text, 0);
			root.Complete(text.Length);
			_nodes.Push(root);
		}

		/// <summary>
		/// Returns true if the index is at the end of the input string
		/// </summary>
		/// <returns></returns>
		public bool AtEndOfInput()
		{
			return Index == Text.Length;
		}

		/// <summary>
		/// Returns the root node in the abstract syntax tree
		/// </summary>
		/// <returns></returns>
		public ParseNode GetRoot()
		{
			Trace.Assert(_nodes.Count == 1);
			return _nodes.Peek();
		}

		/// <summary>
		/// Pushes a node onto the AST stack, which is used for constructing the tree
		/// </summary>
		/// <param name="x"></param>
		public void Push(ParseNode x)
		{
			Trace.Assert(x != null);
			Trace.Assert(x.GetParent() == Peek());
			_nodes.Push(x);
		}

		/// <summary>
		/// Returns the top node on the AST stack.
		/// </summary>
		/// <returns></returns>
		public ParseNode Peek()
		{
			return _nodes.Peek();
		}

		/// <summary>
		/// Removes the top node from the AST stack
		/// </summary>
		public void Pop()
		{
			_nodes.Pop();
		}

		/// <summary>
		/// Will force completion of the parse tree, by completing all nodes on the stack immediately .
		/// Used in the case of an exception to produces a partially complete  tree.
		/// </summary>
		public void ForceCompletion()
		{
			while (_nodes.Count > 1)
			{
				Peek().Complete(Index);
				Pop();
			}
		}

		public ParserState Exception(ParsingException parsingException)
		{
			ForceCompletion();
			ParseException = parsingException;

			return this;
		}

		public ParsingException ParseException { get; set; }
	}
}