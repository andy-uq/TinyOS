namespace Andy.TinyOS
{
	public struct VirtualAddress
	{
		private readonly VirtualAddressCalculator _calculator;
		public ushort Offset { get; private set; }
		public uint PageNumber { get; private set; }

		public uint Address
		{
			get { return _calculator.Address(this); }
		}

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
			return string.Format("[0x{0:x8}] Page: {1:n0}, Offset: {2}", Address, PageNumber, Offset);
		}
	}
}