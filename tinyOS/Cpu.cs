﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andy.TinyOS
{
	public class Cpu
	{
		private const int IdleQuanta = 5;
		private const int UserQuanta = 10;

		private readonly Dictionary<OpCode, Action<Cpu, byte, uint[]>> _operations;
		private readonly Dictionary<uint, ProcessContextBlock> _processes;
		private uint _nextProcessId = 1000;

		public const int PriorityCount = 32;
		public const int RegisterCount = 10;
		public const int LockCount = 10;
		public const int EventCount = 10;
		public const int SpIndex = RegisterCount - 1;

		public ProcessContextBlock CurrentProcess { get; set; }
		public Action<uint> OutputMethod { get; set; }
		public InputDevice InputDevice { get; set; }
		public Ram Ram { get; }
		public ReadyQueue ReadyQueue { get; }
		public DeviceQueue DeviceReadQueue { get; }
		public DeviceQueue DeviceWriteQueue { get; }
		public CpuSleepTimer SleepTimer { get; }
		public Lock[] Locks { get; set; }
		public Event[] Events { get; set; }
		public ulong TickCount { get; private set; }

		public uint[] Registers => CurrentProcess.Registers;

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

		public uint Ip => CurrentProcess.Ip;

		public uint Sp
		{
			get { return Registers[SpIndex]; }
			set { Registers[SpIndex] = value; }
		}

		public Cpu(Ram ram)
		{
			Ram = ram;
			ReadyQueue = new ReadyQueue(PriorityCount);
			DeviceReadQueue = new DeviceQueue();
			DeviceWriteQueue = new DeviceQueue();
			Locks = Enumerable.Range(1, LockCount).Select(x => (DeviceId)(x + Devices.Locks)).Select(x => new Lock(x)).ToArray();
			Events = Enumerable.Range(1, EventCount).Select(x => (DeviceId)(x + Devices.Events)).Select(x => new Event(x)).ToArray();
			SleepTimer = new CpuSleepTimer();
			_processes = new Dictionary<uint, ProcessContextBlock>();
			_operations = OpCodeMetaInformationBuilder.GetMetaInformation().ToDictionary(x => x.OpCode, OpCodeMetaInformationBuilder.BuildOperation);
			IdleProcess = new ProcessContextBlock { Id = 1, };
			InputDevice = new TerminalInputDevice();
		}

		public Cpu(uint ramSize = (4 << 20), uint frameSize = (1 << 10))
			: this(new Ram(ramSize, frameSize))
		{
		}

		public ProcessContextBlock IdleProcess { get; }

		public bool Running => (IdleProcess.ExitCode == 0);

		public ProcessContextBlock Load()
		{
			var pId = NextProcessId();
			var processContextBlock = new ProcessContextBlock
			{
				Id = pId, 
				Priority = 16,
			};

			return processContextBlock;
		}
		
		public Stream AllocateCodeBlock(ProcessContextBlock pcb, uint length)
		{
			var page = new PageInfo();

			while ( length > Ram.FrameSize )
			{
				length -= Ram.FrameSize;
				page.Append(Ram.Allocate(pcb));
			}

			page.Append(Ram.Allocate(pcb));
			pcb.Code = page;

			return GetMemoryStream(page);
		}

		public void Run(ProcessContextBlock block)
		{
			if (block == null)
				throw new ArgumentNullException(nameof(block));

			block.Stack.Append(Ram.Allocate(block));
			block.GlobalData.Append(Ram.Allocate(block));
			block.Registers[Register.G] = block.Id;
			block.Registers[Register.H] = block.GlobalData.Offset;

			_processes.Add(block.Id, block);
			ReadyQueue.Enqueue(block);
			block.IsRunning = true;
		}

		private uint NextProcessId()
		{
			return _nextProcessId++;
		}

		public void Tick()
		{
			if (CurrentProcess == null || CurrentProcess.Quanta == 0 || CurrentProcess == IdleProcess)
			{
				CurrentProcess = SwitchToNextProcess();
			}

			Stream codeStream = GetMemoryStream(CurrentProcess.Code);
			var codeReader = new CodeReader(codeStream);
			var instruction = codeReader[Ip];
			if (instruction != null)
			{
				Execute(instruction);
			}
			else
			{
				foreach (var page in CurrentProcess.PageTable)
					Ram.Free(page);

				CurrentProcess.IsRunning = false;
				CurrentProcess = null;
			}

			TickSleepTimer();
			ReadTerminal();
			WriteTerminal();

			TickCount++;
		}

		private void TickSleepTimer()
		{
			DeviceId sleepTimer;
			if (!SleepTimer.Tick(out sleepTimer)) 
				return;

			BlockingProcess wakingProcess;
			while ((wakingProcess = DeviceReadQueue.Dequeue(sleepTimer)) != null)
				ReadyQueue.Enqueue(wakingProcess.Process);
		}

		private void WriteTerminal()
		{
			BlockingProcess wakingProcess;
			while ((wakingProcess = DeviceWriteQueue.Dequeue(DeviceId.Terminal)) != null)
			{
				var process = wakingProcess.Process;
				(OutputMethod ?? Console.WriteLine)(wakingProcess.Argument);
				ReadyQueue.Enqueue(process);
			}
		}

		private void ReadTerminal()
		{
			uint value;
			if (!InputDevice.Get(out value)) 
				return;
			
			BlockingProcess wakingProcess;
			while ((wakingProcess = DeviceReadQueue.Dequeue(DeviceId.Terminal)) != null)
			{
				var process = wakingProcess.Process;
				process.Registers[wakingProcess.Argument] = value;

				ReadyQueue.Enqueue(process);
			}
		}

		public void Execute(Instruction instruction)
		{
			var pId = CurrentProcess.Id;
			CurrentProcess.Ip++;
			CurrentProcess.Quanta--;

			Action<Cpu, byte, uint[]> operation;
			if (_operations.TryGetValue(instruction.OpCode, out operation))
			{
				operation(this, instruction.OpCodeMask, instruction.Parameters);
				Console.WriteLine("{0}) {1}", pId, instruction);
				return;
			}

			throw new InvalidOperationException("Bad instruction: " + instruction);
		}

		private ProcessContextBlock SwitchToNextProcess()
		{
			if ( CurrentProcess != null )
			{
				ReadyQueue.Enqueue(CurrentProcess);
			}

			var process = ReadyQueue.Dequeue() ?? IdleProcess;
			process.Quanta = UserQuanta;

			return process;
		}

		public void Exit(uint exitCode)
		{
			CurrentProcess.ExitCode = exitCode;
			CurrentProcess.IsRunning = false;

			foreach (var page in CurrentProcess.PageTable)
				Ram.Free(page);

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
				Ram.Free(page);

			process.ExitCode = exitCode;
			process.IsRunning = false;
		}

		public void Jump(uint uOffset)
		{
			unchecked
			{
				var offset = (int) uOffset;
				CurrentProcess.Ip = (uint) (CurrentProcess.Ip + offset) - 1;
			}
		}

		public void Return()
		{
			CurrentProcess.Ip = Pop();
		}

		public void Call(uint value)
		{
			Push(Ip);
			CurrentProcess.Ip = value;
		}

		public uint AllocateShared(uint size)
		{
			var page = new PageInfo();
			var pageSet = Ram.AllocateFromShared(CurrentProcess, size).ToList();
			pageSet.ForEach(page.Append);
			
			CurrentProcess.PageTable.Add(page);
			return page.Offset;
		}

		public uint Allocate(uint size)
		{
			var page = new PageInfo();
			
			while (size > Ram.FrameSize)
			{
				size -= Ram.FrameSize;
				page.Append(Ram.Allocate(CurrentProcess));
			}

			page.Append(Ram.Allocate(CurrentProcess));
			CurrentProcess.PageTable.Add(page);

			return page.Offset;
		}

		public void Free(uint offset)
		{
			var page = CurrentProcess.PageTable.Find(x => x.Offset == offset);
			if (page != null)
			{
				foreach (var p in page.Pages)
					Ram.Free(p);

				CurrentProcess.PageTable.Free(page);
			}
		}

		public bool Write(uint vAddr, uint value)
		{
			var page = CurrentProcess.PageTable.Find(vAddr);
			if (page == null)
				return false;

			using (var stream = GetMemoryStream(page))
			{
				stream.Position = vAddr - page.Offset;
				using (var writer = new BinaryWriter(stream))
					writer.Write(value);
			}

			return true;
		}

		public uint Read(uint vAddr)
		{
			var page = CurrentProcess.PageTable.Find(vAddr);
			if (page == null)
				throw new InvalidOperationException($"Bad address: [0x{vAddr:x8}]");
			
			using (var stream = GetMemoryStream(page))
			{
				stream.Position = vAddr - page.Offset;
				using (var writer = new BinaryReader(stream))
					return writer.ReadUInt32();
			}
		}

		public void Push(uint value)
		{
			if (Write(CurrentProcess.Stack.Offset + Sp, value))
			{
				Sp += 4;
			}
		}

		public uint Pop()
		{
			if (Sp == 0)
				return 0;

			Sp -= 4;
			var value = Read(CurrentProcess.Stack.Offset + Sp);

			return value;
		}

		public uint Peek()
		{
			if (Sp == 0)
				return 0;
			
			return Read(CurrentProcess.Stack.Offset + (Sp - 4));
		}

		public void Sleep(uint sleep)
		{
			var handle = SleepTimer.Register(sleep);
			DeviceReadQueue.Enqueue(handle, CurrentProcess);
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

			var blockingProcess = DeviceReadQueue.Dequeue(@lock.Handle);
			if (blockingProcess == null)
				return;

			var pcb = blockingProcess.Process;

			AcquireLock(pcb, @lock);
			ReadyQueue.Enqueue(pcb);
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

			DeviceReadQueue.Enqueue(@lock.Handle, CurrentProcess);
			CurrentProcess = null;
		}

		private void AcquireLock(ProcessContextBlock process, Lock @lock)
		{
			@lock.Owner = process.Id;
			@lock.RefCount++;
			process.Locks.Add(@lock);
		}

		public void SignalEvent(uint eventNo)
		{
			if ( eventNo == 0 || eventNo > EventCount )
				return;

			var ev = Events[eventNo - 1];

			BlockingProcess blockingProcess;
			while ((blockingProcess = DeviceReadQueue.Dequeue(ev.Handle)) != null)
			{
				ReadyQueue.Enqueue(blockingProcess.Process);
			}
		}

		public void WaitEvent(uint eventNo)
		{
			if ( eventNo == 0 || eventNo > EventCount )
				return;

			var ev = Events[eventNo - 1];
			DeviceReadQueue.Enqueue(ev.Handle, CurrentProcess);
			CurrentProcess = null;
		}

		public Stream GetMemoryStream(PageInfo page)
		{
			return new PageStream(Ram, page);
		}

		public void MemoryClear(uint vAddr, uint count)
		{
			var page = CurrentProcess.PageTable.Find(vAddr);
			if ( page == null )
				return;

			using ( var stream = GetMemoryStream(page) )
			{
				stream.Position = vAddr - page.Offset;
				while (count-- > 0)
					stream.WriteByte(0);
			}
		}

		public void Input(DeviceId deviceId, OpCodeFlag flag, uint rX)
		{
			if (Enum.IsDefined(typeof (DeviceId), deviceId))
			{
				DeviceReadQueue.Enqueue(deviceId, CurrentProcess, flag, rX);
				CurrentProcess = null;
			}
			else
			{
				CurrentProcess.Zf = true;
			}
		}

		public void Output(DeviceId deviceId, uint value)
		{
			if (Enum.IsDefined(typeof (DeviceId), deviceId))
			{
				DeviceWriteQueue.Enqueue(deviceId, CurrentProcess, OpCodeFlag.None, value);
				CurrentProcess.Zf = false;
				CurrentProcess = null;
			}
			else
			{
				CurrentProcess.Zf = true;
			}
		}
	}

	public class InputDevice
	{
		private readonly Stack<uint> _stack = new Stack<uint>();

		public bool Get(out uint value)
		{
			if ( _stack.Count == 0 )
			{
				value = 0;
				return false;
			}

			value = _stack.Pop();
			return true;
		}

		public void Push(uint value)
		{
			_stack.Push(value);
		}
	}

	public class TerminalInputDevice : InputDevice
	{
	}
}