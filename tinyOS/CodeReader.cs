using System.Collections.Generic;
using System.IO;

namespace tinyOS
{
	public class CodeReader
	{
		private MemoryStream _codeStream;

		public CodeReader(byte[] codeData)
		{
			_codeStream = new MemoryStream(codeData);
		}

		public IEnumerable<Instruction> Instructions
		{
			get
			{
				_codeStream.Position = 0;
				var reader = new BinaryReader(_codeStream);
				while (_codeStream.Position < _codeStream.Length )
				{
					var opCodeByte = (OpCode )reader.ReadByte();
					var lValue = reader.ReadUInt32();
					var rValue = reader.ReadUInt32();

					yield return new Instruction {OpCode = opCodeByte, LValue = lValue, RValue = rValue};
				}
			}
		}
	}
}