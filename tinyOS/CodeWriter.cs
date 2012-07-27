using System;
using System.Collections.Generic;
using System.IO;
using Andy.TinyOS.OpCodeMeta;
using tinyOS;

namespace Andy.TinyOS
{
	public class CodeWriter : IDisposable
	{
		private BinaryWriter _writer;
		private long _position;
		private List<ushort> _instructionTable;

		public CodeWriter(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			if (!stream.CanSeek)
				throw new ArgumentException("Must supply a seekable stream");

			_instructionTable = new List<ushort>();
			_writer = new BinaryWriter(stream);
			_position = stream.Position;
			_writer.Write(0U);
		}

		public void Write(Instruction instruction)
		{
			if (_writer == null)
				throw new ObjectDisposedException("CodeWriter");

			var meta = OpCodeMetaInformation.Lookup[instruction.OpCode];
			if (meta.Parameters.Length != instruction.Parameters.Length)
				throw new InvalidOperationException(string.Format("Bad instruction, not enough parameters: {0}", meta.OpCode));

			_instructionTable.Add((ushort )_writer.BaseStream.Position);
			_writer.Write((byte )instruction.OpCode);
			_writer.Write(instruction.OpCodeMask);
			_writer.Write((byte )instruction.Parameters.Length);
			foreach (var t in instruction.Parameters)
				_writer.Write(t);

			_writer.Write(instruction.Comment ?? string.Empty);
			_writer.Flush();
		}

		public void Close()
		{
			if ( _writer == null )
				return;

			var endOfInstructions = (ushort)_writer.BaseStream.Position;
			foreach (var addr in _instructionTable)
				_writer.Write(addr);

			_writer.BaseStream.Seek(_position, SeekOrigin.Begin);
			_writer.Write((ushort)_instructionTable.Count);
			_writer.Write(endOfInstructions);

			_writer.Flush();
		}

		public void Dispose()
		{
			Close();
			_writer.Close();
			_writer = null;
		}
	}
}