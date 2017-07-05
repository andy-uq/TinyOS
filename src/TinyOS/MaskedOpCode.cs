using System;

namespace Andy.TinyOS
{
	public struct MaskedOpCode
	{
		private readonly ushort _value;

		public MaskedOpCode(ushort value) : this()
		{
			_value = value;
		}

		public MaskedOpCode(OpCode opCode)
		{
			_value = (ushort) opCode;
		}

		public OpCodeFlag Destination => (OpCodeFlag) ((_value >> 8) & 0x03);

		public OpCodeFlag Source => (OpCodeFlag) ((_value >> 10) & 0x03);

		public OpCode OpCode => (OpCode) (_value & 0xFF);

		public bool Signed => (_value & 0x8000) != 0;

		public byte OpCodeMask => (byte) (_value >> 8);

		public static explicit operator MaskedOpCode(ushort value)
		{
			return new MaskedOpCode(value);
		}

		public static explicit operator ushort(MaskedOpCode value)
		{
			return value._value;
		}

		public override string ToString()
		{
			if (Source != OpCodeFlag.None && Destination != OpCodeFlag.None)
			{
				return $"{OpCode} {Format(Destination)} {Format(Source)}";
			}

			if (Source == OpCodeFlag.None && Destination == OpCodeFlag.None)
				return OpCode.ToString();

			return $"{OpCode} {Format((OpCodeFlag) ((byte) Source | (byte) Destination))}";
		}

		private string Format(OpCodeFlag opCodeFlag)
		{
			switch (opCodeFlag)
			{
				case OpCodeFlag.Constant:
					return Signed ? "+0" : "0x0";

				case OpCodeFlag.Register:
					return Signed ? "+rX" : "rX";

				case OpCodeFlag.MemoryAddress:
					return Signed ? "+[rX]" : "[rX]";
			}

			throw new ArgumentOutOfRangeException(nameof(opCodeFlag));
		}
	}
}