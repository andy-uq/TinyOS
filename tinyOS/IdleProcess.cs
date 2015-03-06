using System;
using System.IO;
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
					.Mov.RI(Register.E, (int )DeviceId.Terminal)
					.Mov.RI(Register.B, -3)
					.Mov.RI(Register.A, 1)
					.Output.RR(Register.A, Register.E)
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
					.Mov.RI(Register.E, (int) DeviceId.Terminal)
					.Mov.RI(Register.A, 20)
					.Mov.RI(Register.B, -1)
					.Output.RR(Register.A, Register.E)
					.Jmp.R(Register.B);
				
				return code.ToArray();
			}
		}

		public static void Initialise(Cpu cpu, Instruction[] code = null)
		{			
			var ms = new MemoryStream();
			using (var writer = new CodeWriter(ms))
			{
				Array.ForEach(code ?? Instructions, writer.Write);
				writer.Close();

				var codeBlock = cpu.AllocateCodeBlock(cpu.IdleProcess, (uint )ms.Length);
				ms.WriteTo(codeBlock);
			}
		}
	}
}