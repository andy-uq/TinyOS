using System;
using System.Linq;
using Andy.TinyOS.OpCodeMeta;

namespace Andy.TinyOS.Utility
{
	public static class InstructionFormatter
	{
		public static string ToString(Instruction instruction)
		{
			OpCodeMetaInformation meta;
			GetMetaData(instruction, out meta);
			return ToString(meta, instruction);
		}

		public static string ToString(OpCodeMetaInformation meta, Instruction instruction)
		{
			var format = "{0}{1};{2}";
			
			return string.Format(format,
			                     instruction.OpCode.ToString().PadRight(10),
			                     string.Join(string.Empty, instruction.Parameters.Select((x, i) => FormatValue(meta.Parameters, i, instruction.OpCodeMask, x))).PadRight(24),
			                     instruction.Comment ?? meta.Comment);
		}

		public static bool GetMetaData(Instruction instruction, out OpCodeMetaInformation result)
		{
			if (OpCodeMetaInformation.Lookup.TryGetValue(instruction.OpCode, out result))
			{
				return true;
			}

			result = new OpCodeMetaInformation
			{
				OpCode = instruction.OpCode,
				Comment = "Unknown instruction",
				Parameters = instruction.Parameters.Select(x => new ParameterInfo {Name = "Unknown", Type = ParamType.None}).ToArray()
			};

			return false;
		}

		public static void ValidateMetaParameters(Instruction instruction, OpCodeMetaInformation meta)
		{
			if (meta.Parameters.Length != instruction.Parameters.Length)
			{
				throw new InvalidOperationException($"Parameter mismatch: {meta.OpCode} ({meta.Parameters.Length}, {instruction.Parameters.Length})");
			}

			for (var i = 0; i < meta.Parameters.Length; i++)
			{
				var metaParameter = meta.Parameters[i];
				if (metaParameter.Type != ParamType.Register)
				{
					continue;
				}

				var parameterValue = instruction.Parameters[i];
				if (parameterValue >= Cpu.RegisterCount)
					throw new ArgumentOutOfRangeException("Unknown register: r" + parameterValue + 1);
			}
		}

		private static object FormatValue(ParameterInfo[] pInfos, int index, byte controlFlag, uint value)
		{
			var pInfo = (index < pInfos.Length) ? pInfos[index] : new ParameterInfo();
			var opType = (OpCodeFlag) (pInfo.Type == ParamType.Destination ? controlFlag & 3 : controlFlag >> 2 & 3);

			switch (opType)
			{
				case OpCodeFlag.Register:
					return string.Concat('r', value + 1).PadRight(12);

				case OpCodeFlag.Constant:
					return string.Concat('$', value).PadRight(12);

				case OpCodeFlag.MemoryAddress:
					return string.Concat("[r", value + 1, ']').PadRight(12);

				case OpCodeFlag.None:
					return string.Empty.PadRight(12);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}