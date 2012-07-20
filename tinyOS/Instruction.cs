using Andy.TinyOS;
using Andy.TinyOS.Utility;

namespace tinyOS
{
	public class Instruction
	{
		private string _comment;
		
		public OpCode OpCode { get; set; }
		public uint[] Parameters { get; set; }
			
		public string Comment
		{
			get { return _comment; }
			set { _comment = string.IsNullOrWhiteSpace(value) ? null : value; }
		}

		public Instruction()
		{
			Parameters = new uint[0];
		}

		public override string ToString()
		{
			return InstructionFormatter.ToString(this).TrimEnd();
		}
	}
}