using System;
using tinyOS;

namespace Andy.TinyOS
{
	public static class IdleProcess
	{
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