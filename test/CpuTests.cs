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
			var cpu = new Cpu(2048, 256);
			IdleProcess.Initialise(cpu, IdleProcess.TerminatingIdle);

			while (cpu.Running)
			{
				cpu.Tick();
			}
		}

		[Test]
		public void RunRealCpu()
		{
			var cpu = new Cpu(2048, 256);
			cpu.IdleProcess.Code.Append(cpu.Ram.Allocate(cpu.IdleProcess));
			
			var ms = cpu.GetMemoryStream(cpu.IdleProcess.Code);
			var writer = new CodeWriter(ms);
			Array.ForEach(IdleProcess.Instructions, writer.Write);

			cpu.Tick();
			Assert.That(cpu.CurrentProcess, Is.EqualTo(cpu.IdleProcess));
			Assert.That(cpu.CurrentProcess.Ip, Is.EqualTo(1));
			Assert.That(cpu.CurrentProcess.Registers[0], Is.EqualTo(20));


		}

		[Test]
		public void RunCpuWithProgram()
		{
			var cpu = new Cpu(2048, 256);
			var prog1 = cpu.Load();

			cpu.Compile(prog1, _reallySimpleProgram);
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
			var cpu = new Cpu(2048, 256);
			var prog1 = cpu.Load();
			var prog2 = cpu.Load();

			cpu.Compile(prog1, _p1);
			cpu.Compile(prog2, _p2);
			
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