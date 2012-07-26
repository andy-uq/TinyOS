namespace Andy.TinyOS.Parser
{
	public class AndyStructuralGrammar : AndyBaseGrammar
	{
		public Rule term;
		public Rule factor;
		public Rule add_expression;
		public Rule relational_expression;
		public Rule unary_expression;
		public Rule expression;
		public Rule expression_list;
		public Rule assignment_expression;
		public Rule statement;
		public Rule block;
		public Rule function_call;
		private Rule control_statement;
		public Rule if_statement;
		public Rule if_condition;
		public Rule else_block;

		/// <summary>
		/// Creates a rule that matches the rule R multiple times, delimited by commas. 
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public Rule CommaList(Rule r)
		{
			return r + Star(CharSeq(",") + r);
		}

		public AndyStructuralGrammar()
		{
			unary_expression = unary_operator + Recursive(() => term);
			function_call = identifier + CharSeq("(") + Opt(Recursive(() => expression_list)) + CharSeq(")");

			term = 
				int_literal 
				| identifier 
				| function_call
				| unary_expression 
				| Recursive(() => CharSet("(") + expression + CharSet(")"));
			
			term = term + ws;
			factor = term + Star(multiply_operator + ws + term) + ws;
			add_expression = factor + Star(addition_operator + ws + factor);
			relational_expression = add_expression + Star(relational_operator + ws + add_expression);
			expression = relational_expression + Star(logical_operator + ws + relational_expression);
			assignment_expression = identifier + ws + Plus(assignment_operator + ws + expression);
			expression_list = CommaList(expression);

			if_condition = if_keyword + Delimiter("(") + relational_expression + Delimiter(")");
			else_block = else_keyword + ws + Recursive(() => block);
			if_statement = if_condition + Recursive(() => block) + Opt(else_block);
			control_statement = if_statement;

			statement = 
				Opt(assignment_expression) + ws + statement_delimiter 
				| control_statement;

			block = Delimiter("{") + Star(statement + ws) + NoFail(Delimiter("}"));

			InitializeRules<AndyStructuralGrammar>();
		}

		public Rule Delimiter(string delimiter)
		{
			return Skip(CharSeq(delimiter)) + ws;
		}
	}
}