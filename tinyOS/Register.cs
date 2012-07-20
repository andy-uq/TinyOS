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

		public static readonly Register A = new Register(1);
		public static readonly Register B = new Register(2);
		public static readonly Register C = new Register(3);
		public static readonly Register D = new Register(4);
		public static readonly Register E = new Register(5);
		public static readonly Register F = new Register(6);
		public static readonly Register G = new Register(7);
		public static readonly Register H = new Register(8);
		public static readonly Register I = new Register(9);
		public static readonly Register J = new Register(10);
	}
}