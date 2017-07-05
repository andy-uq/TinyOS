namespace Andy.TinyOS.Parser
{
	public class AsmStructuralGrammar : AsmBaseGrammar
	{
		public Rule line;
		public Rule line_list;
		public Rule program;
		public Rule code;

		public AsmStructuralGrammar()
		{
			code = opcode + ws + Opt(operand) + ws + Opt(operand);
			line = code + ws + Opt(comment) + ws
			       | comment + ws;
			
			line_list = line + Star(line);

			program = ws + line_list;
			InitializeRules<AsmStructuralGrammar>();
		}
	}
}