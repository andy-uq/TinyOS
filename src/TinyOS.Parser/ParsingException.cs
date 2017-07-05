using System;

namespace Andy.TinyOS.Parser
{
	public class ParsingException
		: Exception
	{
		public ParsingException(ParserState ps, Rule rule, ParseNode node = null, string message = null)
		{
			UserMessage = message;

			// Store the failed node, the parent node (which should be named), and the associated rule
			ParentNode = node;
			if (ParentNode != null)
				ParentNode = ParentNode.GetNamedParent();

			FailedRule = rule;
			if (ParentNode != null)
				ParentRule = ParentNode.GetRule();

			// set the main text variables
			Text = ps.Text;

			// set the index into the text
			Index = ps.Index;
			if (Index >= Text.Length)
				Index = Text.Length - 1;

			// initialize a bunch of values 
			LineStart = 0;
			Column = 0;
			Row = 0;
			int i = 0;

			// Compute the column, row, and lineStart
			for (; i < Index; ++i)
			{
				if (Text[i] == '\n')
				{
					LineStart = i + 1;
					Column = 0;
					++Row;
				}
				else
				{
					++Column;
				}
			}

			// Compute the line end
			while (i < Text.Length)
				if (Text[i++] == '\n')
					break;
			LineEnd = i;

			// Compute the line length 
			LineLength = LineEnd - LineStart;

			// Get the line text (don't include the new line)
			Line = Text.Substring(LineStart, LineLength - 1);

			// Assume Tabs of length of four
			string tab = "    ";

			// Compute the pointer (^) line will be
			// based on the fact that we will be replacing tabs 
			// with spaces.
			string tmp = Line.Substring(0, Column);
			tmp = tmp.Replace("\t", tab);
			Ptr = new String(' ', tmp.Length);
			Ptr += "^";

			// Replace tabs with spaces
			Line = Line.Replace("\t", tab);
		}

		public string UserMessage { get; set; }
		public ParseNode ParentNode { get; set; }
		public Rule FailedRule { get; set; }
		public Rule ParentRule { get; set; }
		public int Column { get; set; }
		public int Row { get; set; }
		public int Index { get; set; }
		public int LineStart { get; set; }
		public int LineEnd { get; set; }
		public int LineLength { get; set; }
		public string Line { get; set; }
		public string Text { get; set; }
		public string Ptr { get; set; }

		public string Location
		{
			get
			{
				string s = $"line number {Row}, and character number {Column}\n";
				s += Line + "\n";
				s += Ptr + "\n";

				return s;
			}
		}

		public override string Message => ToString();

		public override string ToString()
		{
			string s = UserMessage ?? "parsing exception occured";

			if (ParentRule != null)
			{
				s += $" while parsing '{ParentRule}' ";
			}

			if (FailedRule != null)
			{
				s += $" expected '{FailedRule}' ";
			}

			s += " at \n";
			s += Location;

			return s;
		}
	}
}