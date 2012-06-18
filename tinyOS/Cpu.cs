using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class Cpu
	{
		public const int RegisterCount = 10;
		public const int SpIndex = RegisterCount - 1;

		public ProcessContextBlock CurrentProcess { get; set; }
		
		public Action<uint> PrintMethod { get; set; }
		
		public MemoryManager MemoryManager { get; set; }
		public byte[] Ram { get; private set; }

		public uint[] Registers { get { return CurrentProcess.Registers; } }

		public bool Sf
		{
			get { return CurrentProcess.Sf; }
			set { CurrentProcess.Sf = value; }
		}

		public bool Zf { get { return CurrentProcess.Zf; } set { CurrentProcess.Zf = value; } }
		public uint Ip { get { return CurrentProcess.Ip; } }

		public uint Sp
		{
			get { return Registers[SpIndex]; }
			set { Registers[SpIndex] = value; }
		}

		public Cpu(uint ramSize)
		{
			Ram = new byte[ramSize];
			MemoryManager = new MemoryManager(ramSize);
		}

		public void Run(ProcessContextBlock block)
		{
			var globalData = MemoryManager.Allocate(block.Id, 4096);
			if (globalData != null)
			{
				block.Stack = MemoryManager.Allocate(block.Id, 4096);
				block.GlobalData = MemoryManager.Allocate(block.Id, 4096);
				block.Registers[7] = (uint )block.Id;
				block.Registers[8] = block.GlobalData.Offset;
			}
		}

		public void Exit(uint exitCode)
		{
			CurrentProcess.ExitCode = exitCode;

			foreach (var page in CurrentProcess.PageTable)
				MemoryManager.Free(page);

			MemoryManager.Compact();
		}

		public int Translate(uint vAddr)
		{
			return MemoryManager.Translate(CurrentProcess.PageTable, vAddr);
		}
		
		public void Print(uint value)
		{
			(PrintMethod ?? Console.WriteLine)(value);
		}

		public void Jump(uint uOffset)
		{
			unchecked
			{
				var offset = (int)uOffset;
				CurrentProcess.Ip = (uint)(CurrentProcess.Ip + offset);
			}
		}

		public void Return()
		{
			CurrentProcess.Ip = Pop();
		}

		public void Call(uint value)
		{
			Push(Ip);
			Jump(value);
		}

		public uint Allocate(uint size)
		{
			var page = MemoryManager.Allocate(CurrentProcess.Id, size);

			if ( page == null )
				return 0;

			CurrentProcess.PageTable.Allocate(page);
			return page.Offset;
		}

		public void Free(uint offset)
		{
			var page = CurrentProcess.PageTable.SingleOrDefault(x => x.Offset == (offset - 1024U));
			if (page != null)
			{
				CurrentProcess.PageTable.Free(page);
				MemoryManager.Free(page);
			}
		}

		public void Write(uint vAddr, uint value)
		{
			var addr = Translate(vAddr);
			var data = BitConverter.GetBytes(value);
			Array.Copy(data, 0, Ram, addr, 4);
		}

		public uint Read(uint vAddr)
		{
			var addr = Translate(vAddr);
			return BitConverter.ToUInt32(Ram, addr);
		}

		public void Push(uint value)
		{
			Write(CurrentProcess.Stack.Offset + Sp, value);
			Sp += 4;
		}

		public uint Pop()
		{
			Sp -= 4;
			var value = Read(CurrentProcess.Stack.Offset + Sp);

			return value;
		}

		public uint Peek()
		{
			return Read(CurrentProcess.Stack.Offset + (Sp - 4));
		}
	}

	public class ProcessContextBlock
	{
		private Page _stack;
		private Page _code;
		private Page _globalData;

		public int Id { get; set; }

		public uint[] Registers { get; set; }
		public uint Ip { get; set; }
		public bool Sf { get; set; }
		public bool Zf { get; set; }

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

		public ProcessContextBlock()
		{
			Registers = new uint[10];
			PageTable = new PageTable();
		}
	}
	
	public class PageTable : IEnumerable<Page>
	{
		private readonly SortedSet<Page> _pages;

		public PageTable()
		{
			_pages = new SortedSet<Page>(new PageOffsetComparer { Forward = true });
		}

		public void Allocate(Page page)
		{
			_pages.Add(page);
		}

		public void Free(Page page)
		{
			_pages.Remove(page);
		}

		public IEnumerator<Page> GetEnumerator()
		{
			return _pages.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public class Page
	{
		public int Owner { get; set; }
		public uint Offset { get; set; }
		public uint Size { get; set; }
	}
}