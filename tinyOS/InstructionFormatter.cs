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
			_streamWriter.WriteLine(ToString(instruction));
		}

		public static string ToString(Instruction instruction)
		{
			var format = "{0}{1}";

			if ( instruction.Comment != null )
			{
				format = "{0}{1};{2}";
			}

			OpCodeMetaInformation meta;
			if ( !OpCodeMetaInformation.Lookup.TryGetValue(instruction.OpCode, out meta) )
				meta = new OpCodeMetaInformation
				{
					OpCode = instruction.OpCode, 
					Comment = "Unknown instruction",
					Parameters = instruction.Parameters.Select(x => new ParameterInfo { Name = "Unknown", Type = ParamType.None }).ToArray()
				};

			if (meta.Parameters.Length < instruction.Parameters.Length)
			{
				throw new InvalidOperationException(string.Format("Parameter mismatch: {0} ({1}, {2})", meta.OpCode, meta.Parameters.Length, instruction.Parameters.Length));
			}

			return string.Format(format,
			                     instruction.OpCode.ToString().PadRight(10),
			                     string.Join(string.Empty, instruction.Parameters.Select((x, i) => FormatValue(meta.Parameters[i], x))).PadRight(20),
			                     instruction.Comment);
		}

		private static object FormatValue(ParameterInfo pInfo, uint value)
		{
			switch (pInfo.Type)
			{
				case ParamType.Register:
					return string.Concat('r', value).PadRight(12);

				case ParamType.Constant:
					return string.Concat('$', value).PadRight(12);

				case ParamType.None:
					return string.Format("Unknown ({0})", value);

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