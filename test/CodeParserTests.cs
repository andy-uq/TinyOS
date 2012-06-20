using System;
using System.IO;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CodeParserTests
	{
		[Test]
		public void OpenFile()
		{
			var file = @"D:\Users\andy\Documents\GitHub\TinyOS\Sample Programs\scott13.txt";
			var parser = new TextParser();
			var ms = new StringWriter();
			var writer = new tinyOS.InstructionFormatter(ms);

			using (var streamReader = File.OpenText(file))
			{
				string line;
				while ((line = streamReader.ReadLine()) != null)
				{
					if ( string.IsNullOrWhiteSpace(line) )
						continue;

					var i = parser.Parse(line);
					writer.Write(i);
				}
			}

			writer.Close();
			Console.WriteLine(ms);
		}
	}
}