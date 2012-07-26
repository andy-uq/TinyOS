using System;
using System.Linq;

namespace Andy.TinyOS
{
	public class Register
	{
		private int _index;

		private Register(int i)
		{
			_index = i - 1;
		}

		public int Index
		{
			get { return _index; }
		}

		public static implicit operator uint(Register r)
		{
			return (uint) r._index;
		}

		/// <summary>r1</summary>
		public static readonly Register A = new Register(1);

		/// <summary>r2</summary>
		public static readonly Register B = new Register(2);

		/// <summary>r3</summary>
		public static readonly Register C = new Register(3);

		/// <summary>r4</summary>
		public static readonly Register D = new Register(4);
		
		/// <summary>r5</summary>
		public static readonly Register E = new Register(5);

		/// <summary>r6</summary>
		public static readonly Register F = new Register(6);

		/// <summary>r7</summary>
		public static readonly Register G = new Register(7);

		/// <summary>r8</summary>
		public static readonly Register H = new Register(8);

		/// <summary>r9</summary>
		public static readonly Register I = new Register(9);

		/// <summary>r10</summary>
		public static readonly Register J = new Register(10);

		private static Register[] _registers;

		static Register()
		{
			_registers = typeof (Register)
				.GetFields()
				.Where(x => x.FieldType == typeof (Register))
				.Select(x => x.GetValue(null))
				.Cast<Register>()
				.ToArray();
		}

		public static Register Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("Register name must not be null or empty", "value");

			if (value[0] != 'r')
				throw new ArgumentException("Register name must start with an R", "value");

			int num = int.Parse(value.Substring(1)) - 1;
			return _registers[num];
		}
	}
}