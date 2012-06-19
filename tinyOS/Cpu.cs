using System;
using System.Collections.Generic;
using System.Linq;

namespace tinyOS
{
	public class Cpu
	{
		private const int IdleQuanta = 5;
		private const int UserQuanta = 10;

		private readonly Dictionary<uint, ProcessContextBlock> _processes;
		private uint _nextProcessId = 1;

		public const int PriorityCount = 32;
		public const int RegisterCount = 10;
		public const int LockCount = 10;
		public const int EventCount = 10;
		public const int SpIndex = RegisterCount - 1;

		public ProcessContextBlock CurrentProcess { get; set; }
		public Action<uint> PrintMethod { get; set; }
		public MemoryManager MemoryManager { get; set; }
		public byte[] Ram { get; private set; }
		public ReadyQueue ReadyQueue { get; private set; }
		public DeviceQueue DeviceQueue { get; private set; }
		public Lock[] Locks { get; set; }
		public Event[] Events { get; set; }
		public ulong TickCount { get; private set; }

		public uint[] Registers
		{
			get { return CurrentProcess.Registers; }
		}

		public bool Sf
		{
			get { return CurrentProcess.Sf; }
			set { CurrentProcess.Sf = value; }
		}

		public bool Zf
		{
			get { return CurrentProcess.Zf; }
			set { CurrentProcess.Zf = value; }
		}

		public uint Ip
		{
			get { return CurrentProcess.Ip; }
		}

		public uint Sp
		{
			get { return Registers[SpIndex]; }
			set { Registers[SpIndex] = value; }
		}

		public Cpu(uint ramSize)
		{
			Ram = new byte[ramSize];
			MemoryManager = new MemoryManager(ramSize);
			ReadyQueue = new ReadyQueue(PriorityCount);
			DeviceQueue = new DeviceQueue();
			Locks = Enumerable.Range(1, LockCount).Select(x => (DeviceId) (x + Devices.Locks)).Select(x => new Lock(x)).ToArray();
			Events = Enumerable.Range(1, EventCount).Select(x => (DeviceId) (x + Devices.Events)).Select(x => new Event(x)).ToArray();
			_processes = new Dictionary<uint, ProcessContextBlock>();
			IdleProcess = new ProcessContextBlock
			{
				Id = 0,
			};
		}

		protected ProcessContextBlock IdleProcess { get; set; }

		public void Run(ProcessContextBlock block)
		{
			var globalData = MemoryManager.Allocate(block.Id, 4096);
			if (globalData != null)
			{
				block.Id = NextProcessId();
				block.Stack = MemoryManager.Allocate(block.Id, 4096);
				block.GlobalData = MemoryManager.Allocate(block.Id, 4096);
				block.Registers[7] = (uint) block.Id;
				block.Registers[8] = block.GlobalData.PhysicalOffset;

				_processes.Add(block.Id, block);
				ReadyQueue.Enqueue(block);
			}
		}

		private uint NextProcessId()
		{
			return _nextProcessId++;
		}

		public void Tick()
		{
			if (CurrentProcess == null || CurrentProcess.Quanta == 0)
			{
				CurrentProcess = SwitchToNextProcess();
			}

			CurrentProcess.Quanta--;
			TickCount++;
		}

		private ProcessContextBlock SwitchToNextProcess()
		{
			var process = ReadyQueue.Dequeue();

			if (process == null)
			{
				process = IdleProcess;
				process.Quanta = IdleQuanta;
			}
			else
			{
				process.Quanta = UserQuanta;
			}

			return process;
		}

		public void Exit(uint exitCode)
		{
			CurrentProcess.ExitCode = exitCode;

			foreach (var page in CurrentProcess.PageTable)
				MemoryManager.Free(page);

			MemoryManager.Compact();
			CurrentProcess = null;
		}

		public void AdjustPriority(byte newPriority)
		{
			CurrentProcess.Priority = newPriority;
		}

		public void TerminateProcess(uint pId, uint exitCode)
		{
			var process = _processes[pId];

			foreach (var page in process.PageTable)
				MemoryManager.Free(page);

			process.ExitCode = exitCode;
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
				var offset = (int) uOffset;
				CurrentProcess.Ip = (uint) (CurrentProcess.Ip + offset);
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

			if (page == null)
				return 0;

			CurrentProcess.PageTable.Allocate(page);
			return page.PhysicalOffset;
		}

		public void Free(uint offset)
		{
			var page = CurrentProcess.PageTable.SingleOrDefault(x => x.PhysicalOffset == (offset - 1024U));
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
			Write(CurrentProcess.Stack.PhysicalOffset + Sp, value);
			Sp += 4;
		}

		public uint Pop()
		{
			Sp -= 4;
			var value = Read(CurrentProcess.Stack.PhysicalOffset + Sp);

			return value;
		}

		public uint Peek()
		{
			return Read(CurrentProcess.Stack.PhysicalOffset + (Sp - 4));
		}

		public void Sleep(uint sleep)
		{
			CurrentProcess = null;
		}

		public void ReleaseLock(uint lockNo)
		{
			if (lockNo == 0 || lockNo > LockCount)
				return;

			var @lock = Locks[lockNo - 1];
			if (@lock.Owner != CurrentProcess.Id)
				return;

			@lock.RefCount--;
			if (@lock.RefCount != 0) 
				return;

			@lock.Owner = 0;
			@lock.RefCount = 0;

			var process = DeviceQueue.Dequeue(@lock.Handle);
			if (process == null)
				return;

			AcquireLock(process, @lock);
			ReadyQueue.Enqueue(process);
		}

		public void AcquireLock(uint lockNo)
		{
			if (lockNo == 0 || lockNo > LockCount)
				return;

			var @lock = Locks[lockNo - 1];
			if (@lock.Owner == CurrentProcess.Id || @lock.Owner == 0)
			{
				AcquireLock(CurrentProcess, @lock);
				return;
			}

			DeviceQueue.Enqueue(@lock.Handle, CurrentProcess);
			CurrentProcess = null;
		}

		private void AcquireLock(ProcessContextBlock process, Lock @lock)
		{
			@lock.Owner = process.Id;
			@lock.RefCount++;
			process.Locks.Add(@lock);
		}

		public void SignalEvent(uint eventNo, uint eventData)
		{
			if ( eventNo == 0 || eventNo > EventCount )
				return;

			var ev = Events[eventNo - 1];

			ProcessContextBlock process;
			while ((process = DeviceQueue.Dequeue(ev.Handle)) != null)
			{
				ReadyQueue.Enqueue(process);
			}
		}

		public void WaitEvent(uint eventNo, uint rY)
		{
			if ( eventNo == 0 || eventNo > EventCount )
				return;

			var ev = Events[eventNo - 1];
			DeviceQueue.Enqueue(ev.Handle, CurrentProcess);
			CurrentProcess = null;
		}
	}
}