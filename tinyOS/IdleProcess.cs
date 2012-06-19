using System;
using System.Collections.Generic;

namespace tinyOS
{
	public static class IdleProcess
	{
		private static readonly Instruction[] _instructions = new[] { new Instruction {OpCode = OpCode.Printr, LValue = 20}, new Instruction {OpCode = OpCode.Jmp, LValue = unchecked((uint) (-1))}};

		public static Instruction[] Instructions
		{
			get
			{
				return _instructions;
			}
		}
	}
}