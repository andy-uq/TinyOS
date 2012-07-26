using System;
using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS.OpCodeMeta;
using Andy.TinyOS.Parser;

namespace Andy.TinyOS.Compiler
{
	public class Assembler
	{
		private ParseNode _root;
		private AsmStructuralGrammar _grammar;

		private Assembler()
		{
			_grammar = new AsmStructuralGrammar();
		}

		public Assembler(string source) : this()
		{
			var parser = new ParserState(source);
			bool match = _grammar.program.Match(parser);
			if (match == false)
				throw new Exception("Cannot parse source file");

			_root = parser.GetRoot();
		}

		public Assembler(ParserState parser) : this()
		{
			_root = parser.GetRoot();
		}

		public IEnumerable<Instruction> Assemble()
		{
			return _root
				.GetHierarchy()
				.Where(x => x.Is(_grammar.line))
				.Select(BuildInstruction);
		}

		private Instruction BuildInstruction(ParseNode line)
		{
			var commentNode = line.GetNamedChild("comment_content");
			var code = line.GetNamedChild("code");
			if (code == null)
				return new Instruction(OpCode.Noop, commentNode == null ? "" : commentNode.Value.Trim());

			var opCodeNode = code.GetNamedChild("opcode");
			var opCode = (OpCode)Enum.Parse(typeof(OpCode), opCodeNode.Value, true);
			var meta = OpCodeMeta.OpCodeMetaInformation.Lookup[opCode];

			var instruction = new Instruction(opCode);
			if (commentNode != null)
				instruction.Comment = commentNode.Value.Trim();

			var operands = code.GetNamedChildren("operand").ToArray();
			switch (operands.Length)
			{
				case 2:
					Destination(instruction, operands[0]);
					Source(instruction, operands[1]);
					break;
				case 1:
					switch (meta.Parameters[0].Type)
					{
						case ParamType.Destination:
							Destination(instruction, operands[0]);
							break;
						case ParamType.Source:
							Source(instruction, operands[0]);
							break;
					}
					break;
				case 0:
					break;
			}

			return instruction;
		}

		private void Destination(Instruction instruction, ParseNode operandNode)
		{
			switch (operandNode[0].RuleName)
			{
				case "constant":
					return;

				case "register":
					instruction.Destination(Register.Parse(operandNode.Value));
					return;

				case "memoryAddress":
					instruction.Destination(MemoryAddress.Parse(operandNode["register"].Value));
					return;
			}
		}

		private void Source(Instruction instruction, ParseNode operandNode)
		{
			switch ( operandNode[0].RuleName )
			{
				case "constant":
					instruction.Source(uint.Parse(operandNode["number"].Value));
					return;

				case "register":
					instruction.Source(Register.Parse(operandNode.Value));
					return;

				case "memoryAddress":
					instruction.Source(MemoryAddress.Parse(operandNode["register"].Value));
					return;
			}
		}
	}
}