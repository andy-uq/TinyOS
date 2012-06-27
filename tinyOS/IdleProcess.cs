using System;
using System.Collections.Generic;

namespace tinyOS
{
	public static class IdleProcess
	{
		private static readonly Instruction[] _instructions = new[] 
		{ 
			new Instruction {OpCode = OpCode.Movi, Parameters = new[] { 0U, 20U } }, 
			new Instruction {OpCode = OpCode.Movi, Parameters = new[] { 1U, unchecked((uint) (-1)) } }, 
			new Instruction {OpCode = OpCode.Printr, Parameters = new[] { 0U } }, 
			new Instruction {OpCode = OpCode.Jmp, Parameters = new[] { 1U } } 
		};

		private static readonly Instruction[] _terminatingIdle = new[]
		{
			new Instruction { Comment = "Set r2 as destination", OpCode = OpCode.Movi, Parameters = new[] {2U, unchecked((uint) -3)}},
			new Instruction {Comment = "Assign r1 value 1", OpCode = OpCode.Movi, Parameters = new[] {1U, 1U}},
			new Instruction {Comment = "Assign r1 value 1", OpCode = OpCode.Printr, Parameters = new[] {1U}},
			new Instruction {Comment = "Increase r1", OpCode = OpCode.Incr, Parameters = new[] {1U}},
			new Instruction {Comment = "r1 < 10?", OpCode = OpCode.Cmpi, Parameters = new[] {1U, 10U}},
			new Instruction {Comment = "Jump to r2", OpCode = OpCode.Jlt, Parameters = new[] {2U}},
			new Instruction {Comment = "Exit", OpCode = OpCode.Exit, Parameters = new[] {1U}},
		};

		public static Instruction[] TerminatingIdle
		{
			get { return _terminatingIdle; }
		}

		public static Instruction[] Instructions
		{
			get
			{
				return _instructions;
			}
		}

		public static void Initialise(Cpu cpu, Instruction[] code = null)
		{
			cpu.IdleProcess.Code.Append(cpu.Ram.Allocate(cpu.IdleProcess));

			var ms = cpu.GetMemoryStream(cpu.IdleProcess.Code);
			var writer = new CodeWriter(ms);
			Array.ForEach(code ?? Instructions, writer.Write);
		}
	}
}