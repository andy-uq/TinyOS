namespace Andy.TinyOS
{
	public struct VirtualAddress
	{
		private readonly VirtualAddressCalculator _calculator;
		public ushort Offset { get; }
		public uint PageNumber { get; }

		public uint Address => _calculator.Address(this);

		public VirtualAddress(VirtualAddressCalculator calculator, int offset, int pageNumber)
			: this()
		{
			_calculator = calculator;
			Offset = (ushort )offset;
			PageNumber = (uint) pageNumber;
		}

		public static VirtualAddress operator +(VirtualAddress address, uint offset)
		{
			return address._calculator.New(address.Address + offset);
		}

		public static VirtualAddress operator -(VirtualAddress address, uint offset)
		{
			return address._calculator.New(address.Address - offset);
		}

		public override string ToString()
		{
			return $"[0x{Address:x8}] Page: {PageNumber:n0}, Offset: {Offset}";
		}
	}
}