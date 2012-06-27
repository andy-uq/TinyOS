using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tinyOS
{
	public class Cpu
	{
		private const int IdleQuanta = 5;
		private const int UserQuanta = 10;

		private readonly Dictionary<OpCode, Action<Cpu, uint[]>> _operations;
		private readonly Dictionary<uint, ProcessContextBlock> _processes;
		private uint _nextProcessId = 1000;

		public const int PriorityCount = 32;
		public const int RegisterCount = 10;
		public const int LockCount = 10;
		public const int EventCount = 10;
		public const int SpIndex = RegisterCount - 1;

		public ProcessContextBlock CurrentProcess { get; set; }
		public Action<uint> PrintMethod { get; set; }
		public Ram Ram { get; private set; }
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

		public Cpu(Ram ram)
		{
			Ram = ram;
			ReadyQueue = new ReadyQueue(PriorityCount);
			DeviceQueue = new DeviceQueue();
			Locks = Enumerable.Range(1, LockCount).Select(x => (DeviceId)(x + Devices.Locks)).Select(x => new Lock(x)).ToArray();
			Events = Enumerable.Range(1, EventCount).Select(x => (DeviceId)(x + Devices.Events)).Select(x => new Event(x)).ToArray();
			_processes = new Dictionary<uint, ProcessContextBlock>();
			_operations = OpCodeMeta.OpCodeMetaInformationBuilder.GetMetaInformation().ToDictionary(x => x.OpCode, OpCodeMeta.OpCodeMetaInformationBuilder.BuildOperation);
			IdleProcess = new ProcessContextBlock
			{
				Id = 1,
			};
		}

		public Cpu(uint ramSize = (4 << 20), uint frameSize = (1 << 10))
			: this(new Ram(ramSize, frameSize))
		{
		}

		public ProcessContextBlock IdleProcess { get; private set; }

		public bool Running
		{
			get { return (IdleProcess.ExitCode == 0); }
		}

		public ProcessContextBlock Load()
		{
			var pId = NextProcessId();
			var processContextBlock = new ProcessContextBlock
			{
				Id = pId, 
				Priority = 16,
			};

		    processContextBlock.Code.Append(Ram.Allocate(processContextBlock));
			return processContextBlock;
		}

		public void Run(ProcessContextBlock block)
		{
			if (block == null)
				throw new ArgumentNullException("block");

			block.Stack.Append(Ram.Allocate(block));
			block.GlobalData.Append(Ram.Allocate(block));
			block.Registers[7] = block.Id;
			block.Registers[8] = block.GlobalData.Offset;

			_processes.Add(block.Id, block);
			ReadyQueue.Enqueue(block);
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

			if ( CurrentProcess == IdleProcess && CurrentProcess.Code == null )
			{
				Execute(new Instruction { OpCode = OpCode.Noop });
			}
			else
			{
				var codeReader = new CodeReader(GetMemoryStream(CurrentProcess.Code));
				var instruction = codeReader.Instructions.ElementAt((int) Ip);
				Execute(instruction);
			}

			TickCount++;
		}

		private void Execute(Instruction instruction)
		{
			var pId = CurrentProcess.Id;
			CurrentProcess.Ip++;
			CurrentProcess.Quanta--;

			Action<Cpu, uint[]> operation;
			if (_operations.TryGetValue(instruction.OpCode, out operation))
			{
				operation(this, instruction.Parameters);
				Console.WriteLine("{0}) {1}", pId, instruction);
				return;
			}

			throw new InvalidOperationException("Bad instruction: " + instruction);
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
			Jump(value);
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
                throw new InvalidOperationException(string.Format("Bad address: [0x{0:x8}]", vAddr));
            
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

		public void SignalEvent(uint eventNo)
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

		public void WaitEvent(uint eventNo)
		{
			if ( eventNo == 0 || eventNo > EventCount )
				return;

			var ev = Events[eventNo - 1];
			DeviceQueue.Enqueue(ev.Handle, CurrentProcess);
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
	}
}