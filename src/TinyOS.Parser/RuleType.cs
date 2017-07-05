namespace Andy.TinyOS.Parser
{
	public enum RuleType
	{
		Root,
		Choice,
		Sequence,
		Opt,
		Anything,
		Nothing,
		Star,
		Plus,
		Not,
		Recursive,
		CharSequence,
		CharSet,
		CharRange,
		NoFail,
		Skip,
		Leaf,
		Eof
	}
}