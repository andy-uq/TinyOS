using System.IO;

namespace tinyOS
{
	public class CodeWriter
	{
		private readonly BinaryWriter _writer;

		public CodeWriter(Stream stream)
		{
			_writer = new BinaryWriter(stream);
		}

		public void Write(Instruction instruction)
		{
			_writer.Write((byte )instruction.OpCode);
			_writer.Write((byte )instruction.Parameters.Length);
			foreach (var t in instruction.Parameters)
				_writer.Write(t);

			_writer.Write(instruction.Comment ?? string.Empty);
		}

		public void Close()
		{
			_writer.Close();
		}
	}
}