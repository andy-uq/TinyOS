using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS
{
	public class CodeReader
	{
		private readonly MemoryStream _codeStream;

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
					var pLength = reader.ReadByte();

					yield return new Instruction
					{
						OpCode = opCodeByte, 
						Parameters = Enumerable
							.Range(0, pLength)
							.Select(x => reader.ReadUInt32())
							.ToArray(),
						Comment = reader.ReadString(),
					};
				}
			}
		}
	}
}