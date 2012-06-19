using System.Collections.Generic;

namespace tinyOS
{
	public class ProcessContextBlock
	{
		private Page _stack;
		private Page _code;
		private Page _globalData;
		public HashSet<Lock> Locks { get; set;}

		public uint Id { get; set; }

		public uint[] Registers { get; set; }
		public uint Ip { get; set; }
		public bool Sf { get; set; }
		public bool Zf { get; set; }

		public byte Priority { get; set; }

		public Page Stack
		{
			get { return _stack; }
			set
			{
				_stack = value;
				PageTable.Allocate(value);
			}
		}

		public Page Code
		{
			get { return _code; }
			set
			{
				_code = value;
				PageTable.Allocate(value);
			}
		}

		public Page GlobalData
		{
			get { return _globalData; }
			set
			{
				_globalData = value;
				PageTable.Allocate(value);
			}
		}

		public PageTable PageTable { get; set; }
		public uint ExitCode { get; set; }
		public int Quanta { get; set; }

		public ProcessContextBlock()
		{
			Registers = new uint[10];
			PageTable = new PageTable();
			Locks = new HashSet<Lock>();
		}
	}
}