using System.Linq;

namespace Andy.TinyOS
{
	public class MemoryAddress
	{
		private int _index;

		private MemoryAddress(int i)
		{
			_index = i - 1;
		}

		public int Index
		{
			get { return _index; }
		}

		public static implicit operator uint(MemoryAddress r)
		{
			return (uint) r._index;
		}

		/// <summary>[r1]</summary>
		public static readonly MemoryAddress A = new MemoryAddress(1);

		/// <summary>[r2]</summary>
		public static readonly MemoryAddress B = new MemoryAddress(2);

		/// <summary>[r3]</summary>
		public static readonly MemoryAddress C = new MemoryAddress(3);

		/// <summary>[r4]</summary>
		public static readonly MemoryAddress D = new MemoryAddress(4);
		
		/// <summary>[r5]</summary>
		public static readonly MemoryAddress E = new MemoryAddress(5);

		/// <summary>[r6]</summary>
		public static readonly MemoryAddress F = new MemoryAddress(6);

		/// <summary>[r7]</summary>
		public static readonly MemoryAddress G = new MemoryAddress(7);

		/// <summary>[r8]</summary>
		public static readonly MemoryAddress H = new MemoryAddress(8);

		/// <summary>[r9]</summary>
		public static readonly MemoryAddress I = new MemoryAddress(9);

		/// <summary>[r10]</summary>
		public static readonly MemoryAddress J = new MemoryAddress(10);
		
		private static MemoryAddress[] _memoryAddresses;

		static MemoryAddress()
		{
			_memoryAddresses = typeof(MemoryAddress)
				.GetFields()
				.Where(x => x.FieldType == typeof(MemoryAddress))
				.Select(x => x.GetValue(null))
				.Cast<MemoryAddress>()
				.ToArray();
		}

		public static MemoryAddress Parse(string value)
		{
			var register = Register.Parse(value);
			return _memoryAddresses[register.Index];
		}
	}
}