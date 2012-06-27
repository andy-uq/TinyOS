using System;
using System.IO;

namespace tinyOS
{
	public class CodeWriter : IDisposable
	{
		private BinaryWriter _writer;

		public CodeWriter(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			_writer = new BinaryWriter(stream);
		}

		public void Write(Instruction instruction)
		{
			if (_writer == null)
				throw new ObjectDisposedException("CodeWriter");

			var meta = OpCodeMeta.OpCodeMetaInformation.Lookup[instruction.OpCode];
			if (meta.Parameters.Length != instruction.Parameters.Length)
				throw new InvalidOperationException(string.Format("Bad instruction, not enough parameters: {0}", meta.OpCode));

			_writer.Write((byte )instruction.OpCode);
			_writer.Write((byte )instruction.Parameters.Length);
			foreach (var t in instruction.Parameters)
				_writer.Write(t);

			_writer.Write(instruction.Comment ?? string.Empty);
		}

		public void Close()
		{
			if ( _writer == null )
				return;

			_writer.Write((byte) 255);
			_writer.Close();
			_writer = null;
		}

		public void Dispose()
		{
			Close();
		}
	}
}