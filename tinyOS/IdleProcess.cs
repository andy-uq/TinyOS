using System;
using tinyOS;

namespace Andy.TinyOS
{
	public static class IdleProcess
	{
		private static readonly Instruction[] _terminatingIdle = new[]
		{
			new Instruction { Comment = "Set r2 as destination", OpCode = OpCode.Movi, Parameters = new[] {2U, unchecked((uint) -3)}},
			new Instruction {Comment = "Assign r1 value 1", OpCode = OpCode.Movi, Parameters = new[] {1U, 1U}},
			new Instruction {Comment = "Print r1", OpCode = OpCode.Printr, Parameters = new[] {1U}},
			new Instruction {Comment = "Increase r1", OpCode = OpCode.Incr, Parameters = new[] {1U}},
			new Instruction {Comment = "r1 < 10?", OpCode = OpCode.Cmpi, Parameters = new[] {1U, 10U}},
			new Instruction {Comment = "Jump to r2", OpCode = OpCode.Jlt, Parameters = new[] {2U}},
			new Instruction {Comment = "Exit", OpCode = OpCode.Exit, Parameters = new[] {1U}},
		};

		public static Instruction[] TerminatingIdle
		{
			get
			{
				var code = new CodeStream();

				code.AsFluent()
					.Movi(Register.B, -3)
					.Movi(Register.A, 1)
					.Printr(Register.A)
					.Incr(Register.A)
					.Cmpi(Register.A, 10)
					.Jlt(Register.B)
					.Exit(Register.A)
				;

				return code.ToArray();
			}
		}

		public static Instruction[] Instructions
		{
			get
			{
				var code = new CodeStream();

				code.AsFluent()
					.Movi(Register.A, 20)
					.Movi(Register.B, -1)
					.Printr(Register.A)
					.Jmp(Register.B);
				
				return code.ToArray();
			}
		}

		public static void Initialise(Cpu cpu, Instruction[] code = null)
		{
			cpu.IdleProcess.Code.Append(cpu.Ram.Allocate(cpu.IdleProcess));

			var ms = cpu.GetMemoryStream(cpu.IdleProcess.Code);
			using (var writer = new CodeWriter(ms))
			{
				Array.ForEach(code ?? Instructions, writer.Write);
			}
		}
	}
}