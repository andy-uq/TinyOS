using System;
using System.IO;
using tinyOS.OpCodeMeta;

namespace tinyOS
{
	public class InstructionFormatter
	{
		private readonly TextWriter _streamWriter;

		public InstructionFormatter(TextWriter streamWriter)
		{
			_streamWriter = streamWriter;
		}

		public void Write(Instruction instruction)
		{
			var format = "{0}{1}{2}";

			if ( instruction.Comment != null )
			{
				format = "{0}{1}{2};{3}";
			}

			_streamWriter.WriteLine(format, instruction.OpCode.ToString().PadRight(10), FormatValue(instruction.OpCode, instruction.LValue, 0), FormatValue(instruction.OpCode, instruction.RValue, 1), instruction.Comment);
		}

		private object FormatValue(OpCode opCode, uint u, int i)
		{
			var meta = OpCodeMetaInformation.Lookup[opCode];
			if (i < meta.Parameters.Length)
			{
				var p = meta.Parameters[i];
				switch (p.Type)
				{
					case ParamType.Register:
						return string.Concat('r', u).PadRight(10);

					case ParamType.Constant:
						return string.Concat('$', u).PadRight(10);

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return new string(' ', 10);
		}

		public void Close()
		{
			_streamWriter.Close();
		}
	}
}