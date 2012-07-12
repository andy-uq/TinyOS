using System.IO;

namespace Andy.TinyOS.Parser
{
	public static class CppFileParser
	{
		public static ParserState Parse(string file, IAbstractSyntaxTreePrinter printer = null)
		{
			string text = File.ReadAllText(file);
			return CppSourceParser.Parse(text, printer);
		}
	}
}