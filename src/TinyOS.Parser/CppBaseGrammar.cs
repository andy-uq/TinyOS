namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// A grammar for some of the basic syntactic elements for the C++ language
	/// Inspired by:
	/// * http://www.lysator.liu.se/c/ANSI-C-grammar-y.html
	/// * http://cpp.comsci.us/etymology/literals.html   
	/// </summary>
	public class CppBaseGrammar
		: BaseGrammar
	{
		// ReSharper disable NotAccessedField.Global
		// ReSharper disable InconsistentNaming
		// ReSharper disable MemberCanBePrivate.Global
		// ReSharper disable MemberCanBeProtected.Global
		// ReSharper disable FieldCanBeMadeReadOnly.Global

		#region keyword rules

		public Rule ADD_ASSIGN;
		public Rule AMP;
		public Rule AND_ASSIGN;
		public Rule AND_OP;
		public Rule AUTO;
		public Rule BREAK;
		public Rule CARET;
		public Rule CASE;
		public Rule CHAR;
		public Rule CLASS;
		public Rule COLON;
		public Rule COMMA;
		public Rule COND;
		public Rule CONST;
		public Rule CONTINUE;
		public Rule DEC_OP;
		public Rule DEFAULT;
		public Rule DIV_ASSIGN;
		public Rule DO;
		public Rule DOT;
		public Rule DOUBLE;
		public Rule ELLIPSIS;
		public Rule ELSE;
		public Rule ENUM;
		public Rule EQ;
		public Rule EQ_OP;
		public Rule EXTERN;
		public Rule FLOAT;
		public Rule FOR;
		public Rule GE_OP;
		public Rule GOTO;
		public Rule GT_OP;
		public Rule IF;
		public Rule INC_OP;
		public Rule INT;
		public Rule LEFT_ASSIGN;
		public Rule LEFT_OP;
		public Rule LE_OP;
		public Rule LONG;
		public Rule LT_OP;
		public Rule MINUS;
		public Rule MOD;
		public Rule MOD_ASSIGN;
		public Rule MUL_ASSIGN;
		public Rule NE_OP;
		public Rule NOT;
		public Rule OPERATOR;
		public Rule OR_ASSIGN;
		public Rule OR_OP;
		public Rule PIPE;
		public Rule PLUS;
		public Rule PRIVATE;
		public Rule PROTECTED;
		public Rule PTR_OP;
		public Rule PUBLIC;
		public Rule REGISTER;
		public Rule RETURN;
		public Rule RIGHT_ASSIGN;
		public Rule RIGHT_OP;
		public Rule SHORT;
		public Rule SIGNED;
		public Rule SIZEOF;
		public Rule SLASH;
		public Rule STAR;
		public Rule STATIC;
		public Rule STRUCT;
		public Rule SUB_ASSIGN;
		public Rule SWITCH;
		public Rule TEMPLATE;
		public Rule TILDE;
		public Rule TYPEDEF;
		public Rule TYPEID;
		public Rule TYPENAME;
		public Rule UNION;
		public Rule UNSIGNED;
		public Rule USING;
		public Rule VIRTUAL;
		public Rule VOID;
		public Rule VOLATILE;
		public Rule WHILE;
		public Rule XOR_ASSIGN;

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

		#endregion

		#region pre-processor directives

		public Rule elif_macro;
		public Rule else_macro;
		public Rule endif_macro;
		public Rule ifdef_macro;
		public Rule include;
		public Rule included_file;
		public Rule pragma;

		#endregion

		// ReSharper enable NotAccessedField.Global
		// ReSharper enable InconsistentNaming
		// ReSharper enable MemberCanBePrivate.Global
		// ReSharper enable MemberCanBeProtected.Global
		// ReSharper enable FieldCanBeMadeReadOnly.Global

		#region rule generating functions

		/// <summary>
		/// Creates a rule that matches the rule R multiple times, delimited by commas. 
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public Rule CommaList(Rule r)
		{
			return r + Star(CharSeq(",") + r);
		}

		/// <summary>
		/// Creates a rule that matches a pair of rules, consuming all nested pairs within
		/// as well. 
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Rule Nested(Rule begin, Rule end)
		{
			var recursive = new RecursiveRule(() => Nested(begin, end));
			return begin + NoFailSeq(Star(recursive | Not(end) + Not(begin) + Anything()) + end);
		}

		/// <summary>
		/// Creates a rule that matches a pair of character sequences, consuming all nested pairs 
		/// within as well. For example: "Nested("{", "}"));
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Rule Nested(string begin, string end)
		{
			return Nested(CharSeq(begin), CharSeq(end));
		}

		/// <summary>
		/// Creates a SkipRule that matches a sequence of characters, like CharSeq. It also assures that no other letters follows 
		/// and will also eats whitespace. No node is created.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public Rule Word(string s)
		{
			return CharSeq(s) + Not(ident_next_char) + ws;
		}

		#endregion

		/// <summary>
		/// Constructor: initializes the public rule fields.
		/// </summary>
		public CppBaseGrammar()
		{
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

			#region keyword rules

			COND = Word("?");
			DOT = Word(".");
			COLON = Word(":");
			AMP = Word("&");
			PLUS = Word("+");
			MINUS = Word("-");
			STAR = Word("*");
			SLASH = Word("/");
			MOD = Word("%");
			NOT = Word("!");
			TILDE = Word("~");
			CARET = Word("^");
			PIPE = Word("|");
			EQ = Word("=");
			COMMA = Word(",");
			SIZEOF = Word("sizeof");
			PTR_OP = Word("->");
			INC_OP = Word("++");
			DEC_OP = Word("--");
			LEFT_OP = Word("<<");
			RIGHT_OP = Word(">>");
			LT_OP = Word("<");
			GT_OP = Word(">");
			LE_OP = Word("<=");
			GE_OP = Word(">=");
			EQ_OP = Word("==");
			NE_OP = Word("!=");
			AND_OP = Word("&&");
			OR_OP = Word("||");
			MUL_ASSIGN = Word("*=");
			DIV_ASSIGN = Word("/=");
			MOD_ASSIGN = Word("%=");
			ADD_ASSIGN = Word("+=");
			SUB_ASSIGN = Word("-=");
			LEFT_ASSIGN = Word("<<=");
			RIGHT_ASSIGN = Word(">>=");
			AND_ASSIGN = Word("&=");
			XOR_ASSIGN = Word("^=");
			OR_ASSIGN = Word("|=");
			TYPEDEF = Word("typedef");
			EXTERN = Word("extern");
			STATIC = Word("static");
			AUTO = Word("auto");
			REGISTER = Word("register");
			CHAR = Word("char");
			SHORT = Word("short");
			INT = Word("int");
			LONG = Word("long");
			SIGNED = Word("signed");
			UNSIGNED = Word("unsigned");
			FLOAT = Word("float");
			DOUBLE = Word("double");
			CONST = Word("const");
			VOLATILE = Word("volatile");
			VOID = Word("void");
			STRUCT = Word("struct");
			UNION = Word("union");
			ENUM = Word("enum");
			ELLIPSIS = Word("...");
			CASE = Word("case");
			DEFAULT = Word("default");
			IF = Word("if");
			ELSE = Word("else");
			SWITCH = Word("switch");
			WHILE = Word("while");
			DO = Word("do");
			FOR = Word("for");
			GOTO = Word("goto");
			CONTINUE = Word("continue");
			BREAK = Word("break");
			RETURN = Word("return");
			CLASS = Word("class");
			TYPENAME = Word("typename");
			TYPEID = Word("typeid");
			TEMPLATE = Word("template");
			PUBLIC = Word("public");
			PROTECTED = Word("protected");
			PRIVATE = Word("private");
			VIRTUAL = Word("virtual");
			OPERATOR = Word("operator");
			USING = Word("using");

			#endregion

			#region literals

			dot
				= CharSeq(".");
			dbl_quote
				= CharSeq("\"");
			quote
				= CharSeq("\'");
			simple_escape
				= CharSeq("\\") + CharSet("abfnrtv'\"?\\");
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
			hex_prefix
				= CharSeq("0X") | CharSeq("0x");
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
			float_literal
				= unsigned_float + Opt(float_suffix);
			char_literal
				= Opt(CharSeq("L")) + quote + Star(c_char) + quote;
			string_literal
				= Opt(CharSeq("L")) + dbl_quote + Star(s_char) + dbl_quote;
			boolean_literal
				= Word("true") | Word("false");
			literal
				= (int_literal
				   | char_literal
				   | float_literal
				   | string_literal
				   | boolean_literal)
				  + NoFail(Not(ident_next_char)) + ws;

			#endregion

			#region pre-processor directives

			pragma = Word("#") + Word("pragma") + until_eol;
			included_file = string_literal | CharSeq("<") + Star(Not(CharSeq(">")) + Anything()) + CharSeq(">");
			include = Word("#") + Word("include") + included_file;
			ifdef_macro = Word("#") + Word("if") + until_eol + eol;
			endif_macro = Word("#") + Word("endif") + until_eol + eol;
			elif_macro = Word("#") + Word("elif") + until_eol + eol;
			else_macro = Word("#") + Word("else") + until_eol + eol;

			#endregion

			#region symbols

			semicolon = CharSeq(";");
			eos = Word(";");

			#endregion

			InitializeRules<CppBaseGrammar>();
		}
	}
}