using System.IO;
using Andy.TinyOS;
using Andy.TinyOS.Compiler;
using tinyOS;

namespace ClassLibrary1
{
	public static class ProcessControlBlockExtensions
	{
		public static Stream Compile(this Cpu cpu, ProcessContextBlock pcb, string source)
		{
			var stream = cpu.GetMemoryStream(pcb.Code);
			using (var writer = new CodeWriter(stream))
			{
				var assembler = new Assembler(source);
				foreach (var i in assembler.Assemble())
					writer.Write(i);
			}

			return stream;
		}
	}
}