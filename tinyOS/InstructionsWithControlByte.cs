using System;
using Andy.TinyOS.OpCodeMeta;

namespace Andy.TinyOS
{
	public static class InstructionsWithControlByte
	{
		private static uint ReadValue(Cpu cpu, byte flag, uint param)
		{
			var opCodeFlag = Source(flag);
			return ReadValue(cpu, opCodeFlag, param);
		}

		private static void WriteValue(Cpu cpu, byte flag, uint param, uint value)
		{
			var opCodeFlag = Dest(flag);
			WriteValue(cpu, opCodeFlag, param, value);
		}

		private static uint ReadValue(Cpu cpu, OpCodeFlag flag, uint param)
		{
			switch (flag)
			{
				case OpCodeFlag.Register:
					return cpu.Registers[param];
				case OpCodeFlag.MemoryAddress:
					return cpu.Read(cpu.Registers[param]);
				case OpCodeFlag.Constant:
					return param;
				default:
					throw new ArgumentOutOfRangeException(nameof(flag));
			}
		}

		private static void WriteValue(Cpu cpu, OpCodeFlag flag, uint param, uint value)
		{
			switch (flag)
			{
				case OpCodeFlag.Register:
					cpu.Registers[param] = value;
					break;
				case OpCodeFlag.MemoryAddress:
					cpu.Write(cpu.Registers[param], value);
					break;					
				default:
					throw new ArgumentOutOfRangeException(nameof(flag));
			}
		}

		private static OpCodeFlag Source(byte flag)
		{
			return (OpCodeFlag) (flag >> 2 & 3);
		}

		private static OpCodeFlag Dest(byte flag)
		{
			return (OpCodeFlag) (flag & 3);
		}

		[OpCode(OpCode.Noop, Comment = "Do nothing")]
		public static void Noop(Cpu cpu, byte flag)
		{
			
		}

		[OpCode(OpCode.Incr, Comment = "Increase the value of a register by 1")]
		public static void Incr(Cpu cpu, byte flag, uint destination)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			WriteValue(cpu, flag, destination, unchecked(value + 1));
		}

		[OpCode(OpCode.Add, Comment = "Add a value to a register")]
		public static void Add(Cpu cpu, byte flag, uint destination, uint source)
		{
			var a = ReadValue(cpu, Dest(flag), destination);
			var b = ReadValue(cpu, Source(flag), source);
			WriteValue(cpu, Dest(flag), destination, unchecked (a+b));
		}

		[OpCode(OpCode.Push, Comment = "Push the value of a register onto the stack")]
		public static void Push(Cpu cpu, byte flag, uint source)
		{
			var value = ReadValue(cpu, flag, source);
			cpu.Push(value);
		}
		
		[OpCode(OpCode.Pop, Comment = "Pop a value off the stack into a register")]
		public static void Pop(Cpu cpu, byte flag, uint destination)
		{
			var value = cpu.Pop();
			WriteValue(cpu, flag, destination, value);
		}

		[OpCode(OpCode.Mov, Comment = "Assign a register a value")]
		public static void Mov(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value);
		}

		[OpCode(OpCode.Jmp, Comment = "Jump to an instruction relative to the current instruction. Value may be negative.")]
		public static void Jmp(Cpu cpu, byte flag, uint source)
		{
			var uOffset = ReadValue(cpu, flag, source);
			cpu.Jump(uOffset);
		}

		[OpCode(OpCode.Jlt, Comment = "Jump to an instruction relative to the current instruction when Sf is set. Value may be negative.")]
		public static void Jlt(Cpu cpu, byte flag, uint source)
		{
			if (cpu.Sf)
			{
				var uOffset = ReadValue(cpu, flag, source);
				cpu.Jump(uOffset);
			}
		}

		[OpCode(OpCode.Jgt, Comment = "Jump to an instruction relative to the current instruction when SF is unset. Value may be negative.")]
		public static void Jgt(Cpu cpu, byte flag, uint source)
		{
			if (!cpu.Sf)
			{
				var uOffset = ReadValue(cpu, flag, source);
				cpu.Jump(uOffset);
			}
		}

		[OpCode(OpCode.Je, Comment = "Jump to an instruction relative to the current instruction when ZF is set. Value may be negative.")]
		public static void Je(Cpu cpu, byte flag, uint source)
		{
			if (cpu.Zf)
			{
				var uOffset = ReadValue(cpu, flag, source);
				cpu.Jump(uOffset);
			}
		}

		[OpCode(OpCode.Jne, Comment = "Jump to an instruction relative to the current instruction when ZF is unset. Value may be negative.")]
		public static void Jne(Cpu cpu, byte flag, uint source)
		{
			if (!cpu.Zf)
			{
				var uOffset = ReadValue(cpu, flag, source);
				cpu.Jump(uOffset);
			}
		}

		[OpCode(OpCode.Cmp, Comment = "Compare a register and a constant value. Set ZF if values are equal, SF if destination < source")]
		public static void Cmp(Cpu cpu, byte flag, uint destination, uint source)
		{
			var lValue = ReadValue(cpu, flag, source);
			var rValue = ReadValue(cpu, Dest(flag), destination);

			cpu.Sf = (rValue < lValue);
			cpu.Zf = (lValue == rValue);

			Console.WriteLine($"{rValue} < {lValue} : Sf: {cpu.Sf} Zf: {cpu.Zf}");
		}

		[OpCode(OpCode.Call, Comment = "Call the function absolute from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.")]
		public static void Call(Cpu cpu, byte flag, uint source)
		{
			var uOffset = ReadValue(cpu, flag, source);
			cpu.Call(uOffset);
		}

		[OpCode(OpCode.Ret, Comment = "Returns control to the next instruction after the last call")]
		public static void Return(Cpu cpu, byte flag)
		{
			cpu.Return();
		}

		[OpCode(OpCode.Alloc, Comment = "CreatePage memory")]
		public static void Alloc(Cpu cpu, byte flag, uint destination, uint source)
		{
			uint size = ReadValue(cpu, flag, source);
			var offset = cpu.Allocate(size);
			WriteValue(cpu, flag, destination, offset);
		}

		[OpCode(OpCode.Acquire, Comment = "Acquire the operating system lock whose number is provided in the register")]
		public static void AcquireLock(Cpu cpu, byte flag, uint source)
		{
			var lockNo = ReadValue(cpu, flag, source);
			cpu.AcquireLock(lockNo);
		}

		[OpCode(OpCode.Release, Comment = "Release the lock whose number is provided in the register; If the lock is not held by the current process, the instruction is a no-op")]
		public static void ReleaseLock(Cpu cpu, byte flag, uint source)
		{
			var lockNo = ReadValue(cpu, flag, source);
			cpu.ReleaseLock(lockNo);
		}

		[OpCode(OpCode.Signal, Comment = "Signal the operating system event whose number is provided in the register")]
		public static void SignalEvent(Cpu cpu, byte flag, uint source)
		{
			var eventNo = ReadValue(cpu, flag, source);
			cpu.SignalEvent(eventNo);
		}

		[OpCode(OpCode.Wait, Comment = "Wait for an operating system event to become signalled")]
		public static void WaitEvent(Cpu cpu, byte flag, uint source)
		{
			var eventNo = ReadValue(cpu, flag, source);
			cpu.WaitEvent(eventNo);
		}

		[OpCode(OpCode.Sleep, Comment = "Sleep the number of clock cycles as indicated in r1. If the time to sleep is 0, the process sleeps infinitely")]
		public static void Sleep(Cpu cpu, byte flag, uint source)
		{
			var sleep = ReadValue(cpu, flag, source);
			cpu.Sleep(sleep);
		}

		[OpCode(OpCode.SetP, Comment = "Set the priority of the current process to the value in register")]
		public static void SetPriority(Cpu cpu, byte flag, uint source)
		{
			var priority = ReadValue(cpu, flag, source);
			cpu.AdjustPriority((byte) (priority & 0xFF));
		}

		[OpCode(OpCode.Exit, Comment = "Terminates the current process")]
		public static void Exit(Cpu cpu, byte flag, uint source)
		{
			var exitCode = ReadValue(cpu, flag, source);
			cpu.Exit(exitCode);
		}

		[OpCode(OpCode.TermP, Comment = "Terminates another process")]
		public static void TerminateProcess(Cpu cpu, byte flag, uint destination, uint source)
		{
			var pId = ReadValue(cpu, flag, destination);
			var exitCode = ReadValue(cpu, flag, source);
			cpu.TerminateProcess(pId, exitCode);
		}

		[OpCode(OpCode.Free, Comment = "Free memory previously allocated, pointed to by a register ")]
		public static void FreeMemory(Cpu cpu, byte flag, uint destination)
		{
			var offset = ReadValue(cpu, Dest(flag), destination);
			cpu.Free(offset);
		}

		[OpCode(OpCode.Clear, Comment = "Zero-out memory pointed to by a register for X bytes")]
		public static void MemoryClear(Cpu cpu, byte flag, uint destination, uint source)
		{
			var vAddr = ReadValue(cpu, Dest(flag), destination);
			var count = ReadValue(cpu, flag, source);

			cpu.MemoryClear(vAddr, count);
		}

		[OpCode(OpCode.Map, Comment = "Map a shared memory portion into the address space")]
		public static void Map(Cpu cpu, byte flag, uint destination, uint source)
		{
			uint size = ReadValue(cpu, flag, source);
			var offset = cpu.AllocateShared(size);
			WriteValue(cpu, flag, destination, offset);
		}

		[OpCode(OpCode.Input, Comment = "Read the next 32-bit value into a register")]
		public static void Input(Cpu cpu, byte flag, uint destination, uint source)
		{
			cpu.Input((DeviceId )source, Dest(flag), destination);
		}

		[OpCode(OpCode.Output, Comment = "Output a value to the device pointed to by the register")]
		public static void Output(Cpu cpu, byte flag, uint destination, uint source)
		{
			var deviceId = (DeviceId)ReadValue(cpu, Dest(flag), destination);
			var value = ReadValue(cpu, Source(flag), source);
			cpu.Output(deviceId, value);
		}

		[OpCode(OpCode.Not, Comment = "Return the ones complement of a value")]
		public static void Not(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, ~value);
		}

		[OpCode(OpCode.Neg, Comment = "Return the twos complement of a value")]
		public static void Neg(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, ~value+1);
		}

		[OpCode(OpCode.Mul, Comment = "Return the result of multiplying one register with another")]
		public static void Mul(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			var factor = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value * factor);
		}

		[OpCode(OpCode.Div, Comment = "Return the result of dividing one register with another")]
		public static void Div(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			var denominator = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value / denominator);
		}

		[OpCode(OpCode.And, Comment = "Return the result of AND one register with another")]
		public static void And(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			var rValue = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value & rValue);
		}

		[OpCode(OpCode.Or, Comment = "Return the result of OR one register with another")]
		public static void Or(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			var rValue = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value | rValue);
		}

		[OpCode(OpCode.Xor, Comment = "Return the result of XOR one register with another")]
		public static void Xor(Cpu cpu, byte flag, uint destination, uint source)
		{
			var value = ReadValue(cpu, Dest(flag), destination);
			var rValue = ReadValue(cpu, flag, source);
			WriteValue(cpu, flag, destination, value ^ rValue);
		}
	}
}