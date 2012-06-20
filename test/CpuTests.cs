using System;
using System.IO;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CpuTests
	{
		[Test]
		public void RunCpu()
		{
			var cpu = new Cpu(1024);
			cpu.IdleProcess.Code = cpu.MemoryManager.Allocate(0, 512);
			var ms = new MemoryStream(cpu.Ram, (int )cpu.IdleProcess.Code.PhysicalOffset, (int )cpu.IdleProcess.Code.Size);
			var writer = new CodeWriter(ms);
			Array.ForEach(IdleProcess.TerminatingIdle, writer.Write);

			while (cpu.Running)
			{
				cpu.Tick();
			}
		}
	}
}