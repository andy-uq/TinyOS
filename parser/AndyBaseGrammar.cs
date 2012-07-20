namespace Andy.TinyOS.Parser
{
	public class AndyBaseGrammar : BaseGrammar
	{
		#region operators

		public Rule unary_operator;
		public Rule addition_operator;
		public Rule multiply_operator;
		public Rule relational_operator;
		public Rule logical_operator;

		#endregion

		#region identifiers

		public Rule digit;
		public Rule ident_first_char;
		public Rule ident_next_char;
		public Rule identifier;
		public Rule identifier_extension;
		public Rule letter;
		public Rule lower_case_letter;
		public Rule upper_case_letter;

		#endregion

		#region numbers

		public Rule hex_digit;
		public Rule nonzero_digit;
		public Rule octal_digit;
		public Rule sign;

		#endregion numbers

		#region whitespace

		public Rule comment;
		public Rule eol;
		public Rule eos;
		public Rule ext_line;
		public Rule full_comment;
		public Rule full_comment_content;
		public Rule line_comment;
		public Rule line_comment_content;
		public Rule multiline_ws;
		public Rule semicolon;
		public Rule simple_ws;
		public Rule space;
		public Rule tab;
		public Rule until_eol;
		public Rule ws;

		#endregion

		#region literals

		public Rule boolean_literal;
		public Rule c_char;
		public Rule char_literal;
		public Rule dbl_quote;
		public Rule decimal_literal;
		public Rule digit_sequence;
		public Rule dot;
		public Rule escape_sequence;
		public Rule exponent;
		public Rule exponent_part;
		public Rule exponent_prefix;
		public Rule exponential_float;
		public Rule float_literal;
		public Rule float_suffix;
		public Rule hex_escape;
		public Rule hex_literal;
		public Rule hex_prefix;
		public Rule int_literal;
		public Rule integer_suffix;
		public Rule literal;
		public Rule long_suffix;
		public Rule octal_escape;
		public Rule octal_literal;
		public Rule quote;
		public Rule s_char;
		public Rule simple_escape;
		public Rule simple_float;
		public Rule string_literal;
		public Rule unsigned_float;
		public Rule unsigned_literal;
		public Rule unsigned_suffix;

		public Rule assignment_operator;
		public Rule mul_assign;
		public Rule div_assign;
		public Rule mod_assign;
		public Rule add_assign;
		public Rule sub_assign;

		#endregion

		public AndyBaseGrammar()
		{
			unary_operator = CharSet("-!~");
			multiply_operator = CharSet("*/%");
			addition_operator = CharSet("+-");
			logical_operator = CharSet("&|^");
			relational_operator = CharSet("<>") | CharSeq("<=") | CharSeq(">=");

			#region identifiers

			digit = CharRange('0', '9');
			lower_case_letter = CharRange('a', 'z');
			upper_case_letter = CharRange('A', 'Z');
			letter = lower_case_letter | upper_case_letter;
			ident_first_char = CharSet("_") | letter;
			ident_next_char = ident_first_char | digit;
			identifier_extension = CharSeq("::") + Recursive(() => identifier);
			identifier = Leaf(ident_first_char + Star(ident_next_char) + Star(identifier_extension));

			#endregion

			#region numbers

			octal_digit = CharRange('0', '7');
			nonzero_digit = CharRange('1', '9');
			hex_digit = digit | CharRange('a', 'f') | CharRange('A', 'F');
			sign = CharSet("+-");

			#endregion numbers

			#region whitespace

			tab = CharSeq("\t");
			space = CharSeq(" ");
			simple_ws = tab | space;
			eol = Opt(CharSeq("\r")) + CharSeq("\n");
			ext_line = CharSeq("\\") + Star(simple_ws) + eol;
			multiline_ws = simple_ws | eol;
			until_eol = Star(ext_line | AnythingBut(eol));
			line_comment_content = until_eol;
			line_comment = CharSeq("//") + NoFailSeq(line_comment_content + eol);
			full_comment_content = Until(CharSeq("*/"));
			full_comment = CharSeq("/*") + NoFailSeq(full_comment_content + CharSeq("*/"));
			comment = line_comment | full_comment;
			ws = Eat(multiline_ws);

			#endregion

			dot
				= CharSeq(".");
			dbl_quote
				= CharSeq("\"");
			quote
				= CharSeq("\'");
			simple_escape
				= CharSeq(@"\") + CharSet("abfnrtv'\"?\\");
			octal_escape
				= CharSeq("\\") + octal_digit + Opt(octal_digit + Opt(octal_digit));
			hex_escape
				= CharSeq("\\x") + Star(hex_digit);
			escape_sequence
				= simple_escape
				  | octal_escape
				  | hex_escape;
			c_char
				= escape_sequence | Not(quote) + Anything();
			s_char
				= escape_sequence | Not(dbl_quote) + Anything();
			long_suffix
				= CharSet("Ll");
			unsigned_suffix
				= CharSet("Uu");
			digit_sequence
				= Plus(digit);
			exponent
				= Opt(sign) + digit_sequence;
			exponent_prefix
				= CharSet("Ee");
			exponent_part
				= exponent_prefix + exponent;
			float_suffix
				= CharSet("LlFf");
			simple_float
				= CharSeq(".") + digit_sequence
				  | digit_sequence + dot + Opt(digit_sequence);
			exponential_float
				= digit_sequence + exponent_part
				  | simple_float + exponent_part;
			unsigned_float
				= simple_float
				  | exponential_float;
			hex_prefix = CharSeq("0x") | CharSeq("0X");
			hex_literal
				= hex_prefix + Plus(hex_digit);
			octal_literal
				= CharSeq("0") + Star(octal_digit);
			decimal_literal
				= nonzero_digit + Star(digit);
			unsigned_literal
				= decimal_literal
				  | octal_literal
				  | hex_literal;
			integer_suffix
				= long_suffix
				  | unsigned_suffix
				  | unsigned_suffix + long_suffix
				  | long_suffix + unsigned_suffix;
			int_literal
				= unsigned_literal + Not(dot) + Opt(integer_suffix);

			mul_assign = CharSet("*=");
			div_assign = CharSet("/=");
			mod_assign = CharSet("%=");
			add_assign = CharSet("+=");
			sub_assign = CharSet("-=");
			assignment_operator = CharSet("=") | mul_assign | div_assign | mod_assign | add_assign | sub_assign;


			InitializeRules<AndyBaseGrammar>(false);
		}
	}
}