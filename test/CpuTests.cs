using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CpuTests
	{
		const string _reallySimpleProgram = @"
			movi r1 $1
			exit r1
		";
		const string _p1 = @"
			movi r1 $1
			acquire r1
			wait r1
			release r1
			exit r1
		";
		const string _p2 = @"
			movi r1 $1
			signal r1
			acquire r1
			release r1
			exit r1
		";

		[Test]
		public void RunCpu()
		{
			var cpu = new Cpu(512);
			cpu.IdleProcess.Code = cpu.MemoryManager.Allocate(0, 512);
			var ms = new MemoryStream(cpu.Ram, (int )cpu.IdleProcess.Code.PhysicalOffset, (int )cpu.IdleProcess.Code.Size);
			var writer = new CodeWriter(ms);
			Array.ForEach(IdleProcess.TerminatingIdle, writer.Write);

			while (cpu.Running)
			{
				cpu.Tick();
			}
		}

		[Test]
		public void RunCpuWithProgram()
		{
			var cpu = new Cpu(2048) { DefaultCodeSize = 512, GlobalDataSize = 32, StackSize = 32 };
			var prog1 = cpu.Load();
			
			prog1.Compile(_reallySimpleProgram);
			
			cpu.Run(prog1);

			while (prog1.ExitCode == 0)
			{
				cpu.Tick();
			}

			Assert.That(prog1.ExitCode, Is.EqualTo(1));
		}

		[Test]
		public void ProgramSignals()
		{
			var cpu = new Cpu(2048) { DefaultCodeSize = 512, GlobalDataSize = 32, StackSize = 32 };
			var prog1 = cpu.Load();
			var prog2 = cpu.Load();
			
			prog1.Compile(_p1);
			prog2.Compile(_p2);
			
			cpu.Run(prog1);
			cpu.Run(prog2);

			while (prog1.ExitCode == 0 || prog2.ExitCode == 0)
			{
				cpu.Tick();
			}

			Assert.That(prog1.ExitCode, Is.EqualTo(1));
			Assert.That(prog2.ExitCode, Is.EqualTo(1));
		}
	}
}