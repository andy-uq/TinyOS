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
					.Mov.RI(Register.B, -3)
					.Mov.RI(Register.A, 1)
					.Print.R(Register.A)
					.Incr.R(Register.A)
					.Cmpi.RI(Register.A, 10)
					.Jlt.R(Register.B)
					.Exit.R(Register.A)
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
					.Mov.RI(Register.A, 20)
					.Mov.RI(Register.B, -1)
					.Print.R(Register.A)
					.Jmp.R(Register.B);
				
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