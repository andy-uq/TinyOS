using System;
using System.IO;
using tinyOS;
using tinyOS.OpCodeMeta;

namespace Andy.TinyOS.Utility
{
	public class InstructionTextWriter : IDisposable
	{
		private TextWriter _streamWriter;

		public InstructionTextWriter(TextWriter streamWriter)
		{
			_streamWriter = streamWriter;
		}

		public void Write(Instruction instruction)
		{
			OpCodeMetaInformation meta;
			if ( InstructionFormatter.GetMetaData(instruction, out meta) )
			{
				InstructionFormatter.ValidateMetaParameters(instruction, meta);
				_streamWriter.WriteLine(InstructionFormatter.ToString(instruction));
				return;
			}

			throw new InvalidOperationException("Invalid opcode " + meta.OpCode.ToString() );
		}

		public void Close()
		{
			if ( _streamWriter == null )
				return;

			_streamWriter.Close();
			_streamWriter = null;
		}

		void IDisposable.Dispose()
		{
			Close();
		}
	}
}