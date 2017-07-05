using System;
using System.Collections.Generic;
using System.IO;
using Andy.TinyOS;
using Xunit;

namespace ClassLibrary1
{
	public class CpuTests
	{

		[Fact]
		public void RunCpu()
		{
			var cpu = new Cpu(2048, 256);
			IdleProcess.Initialise(cpu, IdleProcess.TerminatingIdle);

			while (cpu.Running)
			{
				cpu.Tick();
			}
		}

		[Fact]
		public void RunRealCpu()
		{
			var cpu = new Cpu(2048, 256);
			IdleProcess.Initialise(cpu, IdleProcess.Instructions);

 			cpu.Tick();
 			Assert.That(cpu.CurrentProcess, Is.EqualTo(cpu.IdleProcess));
			Assert.That(cpu.CurrentProcess.Ip, Is.EqualTo(1U));
			
			cpu.Tick();
			Assert.That(cpu.CurrentProcess.Registers[0], Is.EqualTo(20U));
		}

		[Fact]
		public void RunCpuWithProgram()
		{
			var cpu = new Cpu(2048, 256);
			IdleProcess.Initialise(cpu, IdleProcess.Instructions);

			var prog1 = cpu.Load();

			cpu.Compile(prog1, TestPrograms.ReallySimpleProgram);
			cpu.Run(prog1);

			while ( prog1.IsRunning )
			{
				cpu.Tick();
			}

			Assert.That(prog1.ExitCode, Is.EqualTo(1U));
		}

		[Fact]
		public void ProgramSignals()
		{
			var cpu = new Cpu(2048, 256);
			IdleProcess.Initialise(cpu, IdleProcess.Instructions);
			
			var prog1 = cpu.Load();
			var prog2 = cpu.Load();

			cpu.Compile(prog1, TestPrograms.P1);
			cpu.Compile(prog2, TestPrograms.P2);
			
			cpu.Run(prog1);
			cpu.Run(prog2);

			while (prog1.IsRunning || prog2.IsRunning)
			{
				cpu.Tick();
			}

			Assert.That(prog1.ExitCode, Is.EqualTo(1U));
			Assert.That(prog2.ExitCode, Is.EqualTo(1U));
		}
	}
}