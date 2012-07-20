﻿using System.Collections;
using System.Collections.Generic;
using tinyOS;

namespace Andy.TinyOS
{
	public class CodeStream : IEnumerable<Instruction>
	{
		private readonly List<Instruction> _instructions;

		public CodeStream()
		{
			_instructions = new List<Instruction>();
		}

		public CodeStream(IEnumerable<Instruction> instructions)
		{
			_instructions = new List<Instruction>(instructions);
		}

		public void Add(OpCode opCode, string comment = null, params uint[] opCodeParameters)
		{
			_instructions.Add(new Instruction { OpCode = opCode, Comment = comment, Parameters = opCodeParameters });
		}

		public CodeStream Add(Instruction instruction)
		{
			_instructions.Add(instruction);
			return this;
		}

		public CodeStream Add(params Instruction[] instructions)
		{
			AddRange(instructions);
			return this;
		}

		public CodeStream AddRange(IEnumerable<Instruction> instructions)
		{
			_instructions.AddRange(instructions);
			return this;
		}

		public Instruction[] ToArray()
		{
			return _instructions.ToArray();
		}

		public static CodeStream operator +(CodeStream stream, Instruction instruction)
		{
			return stream.Add(instruction);
		}

		public static CodeStream operator +(CodeStream stream, IEnumerable<Instruction> instructions)
		{
			return stream.AddRange(instructions);
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			return _instructions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}