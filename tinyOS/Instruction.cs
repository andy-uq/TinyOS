namespace tinyOS
{
	public class Instruction
	{
		public OpCode OpCode { get; set; }
		public uint[] Parameters { get; set; }
		public string Comment { get; set; }

		public override string ToString()
		{
			return InstructionFormatter.ToString(this);
		}
	}
}