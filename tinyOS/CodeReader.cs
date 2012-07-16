using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Andy.TinyOS;

namespace tinyOS
{
	public class CodeReader : IEnumerable<Instruction>
	{
		private readonly Stream _codeStream;
		private readonly int _count;
		private readonly ushort _instructions;
		private readonly ushort _instructionTableOffset;

		public CodeReader(byte[] codeData) : this(new MemoryStream(codeData))
		{
		}

	    public CodeReader(Stream codeStream)
	    {
			if (codeStream == null)
				throw new ArgumentNullException("codeStream");

			if (!codeStream.CanSeek)
				throw new ArgumentException("Must supply a seekable stream");

	        _codeStream = codeStream;
			if ( _codeStream.CanSeek )
			{
				_codeStream.Seek(0, SeekOrigin.Begin);
			}

			var reader = new BinaryReader(_codeStream);
			_count = reader.ReadUInt16();
			_instructionTableOffset = reader.ReadUInt16();
	    	_instructions = (ushort) _codeStream.Position;
	    }

		public Instruction this[uint ip]
		{
			get
			{
				if ( ip > Count )
					return null;

				var position = _instructionTableOffset + (ip*2);
				_codeStream.Seek(position, SeekOrigin.Begin);

				var reader = new BinaryReader(_codeStream);
				var offset = reader.ReadUInt16();

				_codeStream.Seek(offset, SeekOrigin.Begin);
				return BuildInstruction(reader);
			}
		}

		private Instruction BuildInstruction(BinaryReader reader)
		{
			var opCodeByte = (OpCode)reader.ReadByte();

			var pLength = reader.ReadByte();
			var parameters = Enumerable
				.Range(0, pLength)
				.Select(x => reader.ReadUInt32())
				.ToArray();
			var comment = reader.ReadString();

			return new Instruction
			{
				OpCode = opCodeByte,
				Parameters = parameters,
				Comment = comment,
			};
		}

		public IEnumerator<Instruction> GetEnumerator()
		{
			var reader = new BinaryReader(_codeStream);
			var pos = _count;
			while ( pos-- > 0 )
			{
				yield return BuildInstruction(reader);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return _count; }
		}
	}
}