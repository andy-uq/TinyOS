using System;

namespace Andy.TinyOS.Parser
{
	public class AsmBaseGrammar : BaseGrammar
	{
		public Rule digit;
		public Rule lower_case_letter;
		public Rule upper_case_letter;
		public Rule letter;
		public Rule number;
		public Rule sign;

		public Rule space;
		public Rule tab;
		public Rule simple_ws;
		public Rule multiline_ws;
		public Rule ws;
		public Rule eol;

		public Rule opcode;
		public Rule register;
		public Rule memoryAddress;
		public Rule constant;
		public Rule comment;
		public Rule comment_content;
		public Rule operand;

		public AsmBaseGrammar()
		{
			space = CharSeq(" ");
			tab = CharSeq("\t");
			simple_ws = space | tab;
			eol = Opt(CharSeq("\r")) + CharSeq("\n");
			multiline_ws = simple_ws | eol;
			ws = Eat(multiline_ws);
			digit = CharRange('0', '9');
			number = Leaf(Plus(digit));
			lower_case_letter = CharRange('a', 'z');
			upper_case_letter = CharRange('A', 'Z');
			letter = lower_case_letter | upper_case_letter;
			sign = CharSet("$-");

			opcode = Leaf(letter + Star(letter));
			register = Leaf(CharSeq("r") + Plus(digit));
			memoryAddress = CharSeq("[") + register + CharSeq("]");
			constant = sign + number;
			operand = register | memoryAddress | constant;
			comment_content = Leaf(Star(AnythingBut(eol)));
			comment = CharSet(";") + comment_content;

			InitializeRules<AsmBaseGrammar>();
		}
	}
}