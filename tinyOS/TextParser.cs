using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace tinyOS
{
	public class InstructionFormatter
	{
		private readonly StreamWriter _streamWriter;

		public InstructionFormatter(Stream stream)
		{
			_streamWriter = new StreamWriter(stream);
		}

		public void Write(Instruction instruction)
		{
			var format = "{0}{1}{2}";

			if ( instruction.Comment != null )
			{
				format = "{0}{1}{2};{3}";
			}

			_streamWriter.WriteLine(format, instruction.OpCode, instruction.LValue, instruction.RValue, instruction.Comment);
		}

		public void Close()
		{
			_streamWriter.Close();
		}
	}

	public class TextParser
	{
		private static readonly Regex _legacy = new Regex(@"(?<opcode>(\d+))(\s+(?<lvalue>\S+))?(\s+(?<rvalue>\S+))?(\s*;?<comment>.*)?");
		private static readonly Regex _asm = new Regex(@"(?<opcode>(\w+))(\s+(?<lvalue>\S+))?(\s+(?<rvalue>\S+))?(\s*;?<comment>.*)?");
		private static readonly Regex _register = new Regex(@"r(?<r>\d+)");
		private static readonly Regex _constant = new Regex(@"($(?<int>-?\d+))|(0x(?<hex>[0-9a-f]+))");

		public Instruction Parse(string line)
		{
			Func<Match, Instruction> parse;

			var match = _legacy.Match(line);
			if (match.Success)
			{
				parse = ParseLegacy;
			}
			else
			{
				match = _asm.Match(line);
				parse = Parse;
			}

			if (match.Success)
				return parse(match);

			throw new InvalidOperationException("Cannot parse line");
		}

		private Instruction ParseLegacy(Match match)
		{
			var opCode = match.Groups["opcode"].Value;
			var lValue = match.Groups["lvalue"].Value;
			var rValue = match.Groups["rvalue"].Value;
			var comment = match.Groups["comment"].Value;
			
			return new Instruction
			{
				OpCode = (OpCode)int.Parse(opCode),
				LValue = DecodeValue(lValue),
				RValue = DecodeValue(rValue),
				Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim()
			};
		}

		private Instruction Parse(Match match)
		{
			var opCode = match.Groups["opcode"].Value;
			var lValue = match.Groups["lvalue"].Value;
			var rValue = match.Groups["rvalue"].Value;
			var comment = match.Groups["comment"].Value;
			
			return new Instruction
			{
				OpCode = (OpCode)int.Parse(opCode),
				LValue = DecodeValue(lValue),
				RValue = DecodeValue(rValue),
				Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim()
			};
		}

		private static uint DecodeValue(string lValue)
		{
			if ( string.IsNullOrEmpty(lValue) )
				return default(uint);

			Match match;
			if ((match = _register.Match(lValue)).Success)
			{
				return uint.Parse(match.Value);
			}

			if ( (match = _constant.Match(lValue)).Success )
			{
				return match.Groups["int"].Success
				       	? unchecked((uint) (long.Parse(match.Groups["int"].Value)))
				       	: uint.Parse(match.Groups["hex"].Value, NumberStyles.AllowHexSpecifier);
			}

			return uint.MaxValue;
		}
	}
}