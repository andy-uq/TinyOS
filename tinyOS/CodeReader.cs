using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS
{
	public class CodeReader
	{
		private readonly Stream _codeStream;

		public CodeReader(byte[] codeData)
		{
			_codeStream = new MemoryStream(codeData);
		}

	    public CodeReader(Stream codeStream)
	    {
	        _codeStream = codeStream;
	    }

	    public IEnumerable<Instruction> Instructions
		{
			get
			{
				if (_codeStream.CanSeek)
				{
					_codeStream.Seek(0, SeekOrigin.Begin);
				}

				var reader = new BinaryReader(_codeStream);
				while (_codeStream.Position < _codeStream.Length )
				{
					var instruction = BuildInstruction(reader);
					if (instruction == null)
						yield break;

					yield return instruction;
				}
			}
		}

		private Instruction BuildInstruction(BinaryReader reader)
		{
			var opCodeByte = (OpCode)reader.ReadByte();
			if ( opCodeByte == (OpCode)255 )
				return null;

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
	}
}