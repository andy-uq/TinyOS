namespace Andy.TinyOS.Parser
{
	public static class CppSourceParser
	{
		private static readonly CppStructuralGrammar _grammar = new CppStructuralGrammar();

		public static ParserState Parse(string source, IAbstractSyntaxTreePrinter printer = null, Rule rule = null)
		{
			if (rule == null)
				rule = _grammar.file;

			var state = new ParserState(source);
			try
			{
				if (rule.Match(state))
				{
					if (state.AtEndOfInput())
					{
						if (printer != null)
							printer.Print(state.GetRoot());

						return state;
					}

					throw new ParsingException(state, rule, message: "Failed to read end of input");
				}

				throw new ParsingException(state, rule, message: "Failed to parse source");
			}
			catch (ParsingException e)
			{
				return state.Exception(e);
			}
		}
	}
}