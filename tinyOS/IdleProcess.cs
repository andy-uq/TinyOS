using System;
using System.Collections.Generic;

namespace tinyOS
{
	public static class IdleProcess
	{
		private static readonly Instruction[] _instructions = new[] 
		{ 
			new Instruction {OpCode = OpCode.Printr, Parameters = new[] { 20U } }, 
			new Instruction {OpCode = OpCode.Jmp, Parameters = new[] { unchecked((uint) (-1)) } } 
		};

		public static Instruction[] Instructions
		{
			get
			{
				return _instructions;
			}
		}
	}
}