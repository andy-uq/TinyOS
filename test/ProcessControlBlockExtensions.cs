using System.IO;
using Andy.TinyOS;
using Andy.TinyOS.Compiler;
using tinyOS;

namespace ClassLibrary1
{
	public static class ProcessControlBlockExtensions
	{
		public static uint Compile(this Cpu cpu, ProcessContextBlock pcb, string source)
		{
			var ms = new MemoryStream();
			using (var writer = new CodeWriter(ms))
			{
				var assembler = new Assembler(source);
				foreach (var i in assembler.Assemble())
					writer.Write(i);

				writer.Close();

				var codeBlock = cpu.AllocateCodeBlock(pcb, (uint)ms.Length);
				ms.WriteTo(codeBlock);

				return (uint )ms.Length;
			}
		}
	}
}