using System.Collections.Generic;
using System.IO;

namespace Andy.TinyOS
{
	public class ProcessContextBlock
	{
		public ProcessContextBlock()
		{
			Registers = new uint[10];
			Locks = new HashSet<Lock>();

			Stack = new PageInfo();
			Code = new PageInfo();
			GlobalData = new PageInfo();

			PageTable = new PageTable {Code, GlobalData, Stack};
		}

		public uint Id { get; set; }

		public uint[] Registers { get; }
		public uint Ip { get; set; }
		public bool Sf { get; set; }
		public bool Zf { get; set; }

		public byte Priority { get; set; }

		public HashSet<Lock> Locks { get; }
		public PageTable PageTable { get; }
		public uint ExitCode { get; set; }
		public int Quanta { get; set; }

		public PageInfo Stack { get; }
		public PageInfo Code { get; set; }
		public PageInfo GlobalData { get; }

		public bool IsRunning { get; set; }

		public override string ToString()
		{
			return $"PID:{Id}";
		}
	}

	public static class ProcessControlBlockExtensions
	{
		public static uint Compile(this Cpu cpu, ProcessContextBlock pcb, IEnumerable<Instruction> instructions)
		{
			var stream = new MemoryStream();
			using (var writer = new CodeWriter(stream))
			{
				foreach (var instruction in instructions)
					writer.Write(instruction);

				writer.Close();
				using (var ps = cpu.AllocateCodeBlock(pcb, (uint) stream.Length))
					stream.WriteTo(ps);

				return (uint) stream.Length;
			}
		}
	}
}