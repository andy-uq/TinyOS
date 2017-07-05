using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.TinyOS.Parser
{
	public class CppFileSetParser
	{
		public static IEnumerable<ParserState> Parse(IAbstractSyntaxTreePrinter printer, string sDir)
		{
			foreach ( var state in Directory.GetFiles("*.c;*.cpp;*.h;*.hpp;").Select(fi => CppFileParser.Parse(fi, printer)) )
			{
				if (state.ParseException != null)
					throw state.ParseException;

				yield return state;
			}
		}
	}
}