using System;
using System.Collections.Generic;
using System.IO;
using Andy.TinyOS;
using FluentAssertions;
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
 			cpu.CurrentProcess.Should().Be(cpu.IdleProcess);
			cpu.CurrentProcess.Ip.Should().Be(1U);
			
			cpu.Tick();
			cpu.CurrentProcess.Registers[0].Should().Be(20U);
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

			prog1.ExitCode.Should().Be(1U);
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

			prog1.ExitCode.Should().Be(1U);
			prog2.ExitCode.Should().Be(1U);
		}
	}
}