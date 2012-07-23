using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using tinyOS;

namespace Andy.TinyOS.Utility
{
	public class InstructionTextReader
	{
		private static readonly Regex _asm = new Regex(@"^\s*(?<opcode>(\w+))(\s+(?!;)(?<value>\S+))*(\s*;\s*(?<comment>.*))?");
		private static readonly Regex _memory = new Regex(@"\[r(?<m>\d+)\]");
		private static readonly Regex _register = new Regex(@"r(?<r>\d+)");
		private static readonly Regex _constant = new Regex(@"(\$(?<int>-?\d+))|(0x(?<hex>[0-9a-f]+))");

		public Instruction Parse(string line)
		{
			Func<Match, Instruction> parse;

			var match = _asm.Match(line);
			if (match.Success)
				return Parse(match);

			throw new InvalidOperationException("Cannot parse line: " + line);
		}

		private Instruction Parse(Match match)
		{
			var opCodeName = match.Groups["opcode"].Value;
			var comment = match.Groups["comment"].Value;

			var opCode = new MaskedOpCode((OpCode) Enum.Parse(typeof (OpCode), opCodeName, true));
			var parameters = match.Groups["value"].Captures
				.Cast<Capture>()
				.Select(x => DecodeValue(x.Value))
				.ToArray();

			switch (parameters.Length)
			{
				case 1:
					opCode.SetSource(parameters[0].Item1);
					if (parameters[0].Item1 != OpCodeFlag.Constant)
						opCode.SetDest(parameters[0].Item1);
					break;
				case 2:
					opCode.SetDest(parameters[0].Item1).SetSource(parameters[1].Item1);
					break;
			}

			return new Instruction
			{
				MaskedOpCode = opCode,
				Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
				Parameters = parameters.Select(x => x.Item2).ToArray(),
			};
		}

		private static Tuple<OpCodeFlag, uint> DecodeValue(string lValue)
		{
			if ( string.IsNullOrEmpty(lValue) )
				return new Tuple<OpCodeFlag, uint>(OpCodeFlag.None, 0);

			Match match;
			if ((match = _register.Match(lValue)).Success)
			{
				var value = uint.Parse(match.Groups["r"].Value) - 1;
				return new Tuple<OpCodeFlag, uint>(OpCodeFlag.Register, value);
			}

			if ((match = _memory.Match(lValue)).Success)
			{
				var value = uint.Parse(match.Groups["r"].Value) - 1;
				return new Tuple<OpCodeFlag, uint>(OpCodeFlag.MemoryAddress, value);
			}

			if ( (match = _constant.Match(lValue)).Success )
			{
				var value = match.Groups["int"].Success
				       	? unchecked((uint) (long.Parse(match.Groups["int"].Value)))
				       	: uint.Parse(match.Groups["hex"].Value, NumberStyles.AllowHexSpecifier);

				return new Tuple<OpCodeFlag, uint>(OpCodeFlag.Constant, value);
			}

			throw new InvalidOperationException("Cannot parse value: " + lValue);
		}
	}
}