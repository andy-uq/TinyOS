using System;
using System.IO;
using System.Linq;
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
			var format = "{0}{1}";

			if ( instruction.Comment != null )
			{
				format = "{0}{1};{2}";
			}

			var meta = OpCodeMetaInformation.Lookup[instruction.OpCode];
			if (meta.Parameters.Length < instruction.Parameters.Length)
			{
				throw new InvalidOperationException(string.Format("Parameter mismatch: {0} ({1}, {2})", meta.OpCode, meta.Parameters.Length, instruction.Parameters.Length));
			}

			_streamWriter.WriteLine(format, 
				instruction.OpCode.ToString().PadRight(10), 
				string.Join(string.Empty, instruction.Parameters.Select((x, i) => FormatValue(meta.Parameters[i], x))).PadRight(20), 
				instruction.Comment
			);
		}

		private object FormatValue(ParameterInfo pInfo, uint value)
		{
			switch (pInfo.Type)
			{
				case ParamType.Register:
					return string.Concat('r', value).PadRight(10);

				case ParamType.Constant:
					return string.Concat('$', value).PadRight(10);

				case ParamType.None:
					return new string(' ', 10);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Close()
		{
			_streamWriter.Close();
		}
	}
}