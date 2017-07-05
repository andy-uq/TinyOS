using System;
using Andy.TinyOS.Utility;

namespace Andy.TinyOS
{
	public class Instruction
	{
		private string _comment;

		public MaskedOpCode MaskedOpCode { get; set; }
		public OpCode OpCode => MaskedOpCode.OpCode;
		public byte OpCodeMask => MaskedOpCode.OpCodeMask;
		public uint[] Parameters { get; set; }
			
		public string Comment
		{
			get { return _comment; }
			set { _comment = string.IsNullOrWhiteSpace(value) ? null : value; }
		}

		public Instruction(OpCode opCode, string commment = null) : this()
		{
			MaskedOpCode = new MaskedOpCode(opCode);
			Comment = commment;
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

	public static class InstuctionExtensions
	{
		public static Instruction Destination(this Instruction instuction, Register value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetDest(OpCodeFlag.Register);
			instuction.Parameters = instuction.Parameters.Length == 0
			                        	? new uint[] {value}
			                        	: new[] {value, instuction.Parameters[0]};

			return instuction;
		}

		public static Instruction Source(this Instruction instuction, Register value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetSource(OpCodeFlag.Register);
			instuction.Parameters = instuction.Parameters.Length == 0
			                        	? new uint[] {value}
			                        	: new[] { instuction.Parameters[0], value };

			return instuction;
		}

		public static Instruction Destination(this Instruction instuction, MemoryAddress value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetDest(OpCodeFlag.MemoryAddress);
			instuction.Parameters = instuction.Parameters.Length == 0
			                        	? new uint[] {value}
			                        	: new[] {value, instuction.Parameters[0]};

			return instuction;
		}

		public static Instruction Source(this Instruction instuction, MemoryAddress value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetSource(OpCodeFlag.MemoryAddress);
			instuction.Parameters = instuction.Parameters.Length == 0
			                        	? new uint[] {value}
			                        	: new[] { instuction.Parameters[0], value };

			return instuction;
		}

		public static Instruction Source(this Instruction instuction, uint value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetSource(OpCodeFlag.Constant);
			instuction.Parameters = instuction.Parameters.Length == 0
			                        	? new[] {value}
			                        	: new[] { instuction.Parameters[0], value };

			return instuction;
		}

		public static Instruction Source(this Instruction instuction, int value)
		{
			instuction.MaskedOpCode = instuction.MaskedOpCode.SetSource(OpCodeFlag.Constant);
			instuction.Parameters = instuction.Parameters.Length == 0
										? new [] { unchecked((uint)value) }
			                        	: new [] { instuction.Parameters[0], unchecked ((uint )value) };

			return instuction;
		}
	}

	public static class OpCodeExtensions
	{
		public static MaskedOpCode SetSource(this MaskedOpCode opCode, OpCodeFlag sourceFlag)
		{
			var code = (ushort)opCode;
			code &= 0xf3ff;
			var result = code | ((ushort)sourceFlag << 10);

			return (MaskedOpCode)result;
		}

		public static MaskedOpCode SetDest(this MaskedOpCode opCode, OpCodeFlag destFlag)
		{
			if (destFlag == OpCodeFlag.Constant)
				throw new InvalidOperationException("Cannot set constant as destination vector");

			var code = (ushort) opCode;
			code &= 0xfcff;
			var result = code | ((ushort)destFlag << 8);

			return (MaskedOpCode )result;
		}

		public static MaskedOpCode AsSigned(this MaskedOpCode opCode)
		{
			var code = (ushort) opCode;
			code |= 0x8000;
			
			return new MaskedOpCode(code);
		}
	}
}