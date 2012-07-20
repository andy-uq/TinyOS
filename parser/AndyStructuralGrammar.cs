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
			term = int_literal | identifier | unary_expression | Recursive(() => CharSet("(") + expression + CharSet(")"));
			factor = term + Star(multiply_operator + term);
			add_expression = factor + Star(addition_operator + factor);
			relational_expression = add_expression + Star(relational_operator + add_expression);
			expression = relational_expression + Star(logical_operator + relational_expression);
			expression_list = CommaList(expression);
			assignment_expression = identifier + Star(assignment_operator + expression);

			InitializeRules<AndyStructuralGrammar>();
		}
	}
}