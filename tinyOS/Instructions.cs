using System;
using tinyOS.OpCodeMeta;

namespace tinyOS
{
	public static class Instructions
	{
		[OpCode(OpCode.Noop, Comment = "Do nothing")]
		public static void Noop(Cpu cpu)
		{}

		[OpCode(OpCode.Incr, Comment = "Increase the value of a register by 1")]
		[Parameter("rX", Type = ParamType.Register, Comment="Register to be increased")]
		public static void Incr(Cpu cpu, uint rX)
		{
			unchecked
			{
				cpu.Registers[rX]++;
			}
		}

		[OpCode(OpCode.Addi, Comment = "Add a constant value to a register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register to be increased")]
		[Parameter("iValue", Type = ParamType.Constant, Comment = "Amount to increase")]
		public static void Addi(Cpu cpu, uint rX, uint iValue)
		{
			unchecked
			{
				cpu.Registers[rX] += iValue;
			}
		}

		[OpCode(OpCode.Addr, Comment = "Add the value of one register to another")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register to be increased")]
		[Parameter("rY", Type = ParamType.Constant, Comment = "Register containing amount to increase by")]
		public static void Addr(Cpu cpu, uint rX, uint rY)
		{
			unchecked
			{
				cpu.Registers[rX] += cpu.Registers[rY];
			}
		}

		[OpCode(OpCode.Pushr, Comment = "Push the value of a register onto the stack")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register to be pushed")]
		public static void Pushr(Cpu cpu, uint rX)
		{
			cpu.Push(cpu.Registers[rX]);
		}

		[OpCode(OpCode.Pushi, Comment = "Push the value of a constant onto the stack")]
		[Parameter("iValue", Type = ParamType.Constant, Comment = "Constant to be pushed")]
		public static void Pushi(Cpu cpu, uint iValue)
		{
			cpu.Push(iValue);
		}

		[OpCode(OpCode.Popr, Comment = "Pop a value off the stack into a register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register to receive the value")]
		public static void Popr(Cpu cpu, uint rX)
		{
			cpu.Registers[rX] = cpu.Pop();
		}

		[OpCode(OpCode.Popm, Comment = "Pop a value off the stack and into a memory location pointed to by a register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register pointing to memory address")]
		public static void Popm(Cpu cpu, uint rX)
		{
			var value = cpu.Pop();

			var vAddr = cpu.Registers[rX];
			cpu.Write(vAddr, value);
		}

		[OpCode(OpCode.Movi, Comment = "Assign a register to a constant value")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register being assigned")]
		[Parameter("iValue", Type = ParamType.Constant, Comment = "Assignment value")]
		public static void Movi(Cpu cpu, uint rX, uint iValue)
		{
			cpu.Registers[rX] = iValue;
		}

		[OpCode(OpCode.Movr, Comment = "Assign a register the same value as another register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register being assigned")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing the assignment value")]
		public static void Movr(Cpu cpu, uint rX, uint rY)
		{
			cpu.Registers[rX] = cpu.Registers[rY];
		}

		[OpCode(OpCode.Movmr, Comment = "Assign a register a value from memory")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register being assigned")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing source address")]
		public static void Movmr(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];
			cpu.Registers[rX] = cpu.Read(vAddr);
		}

		[OpCode(OpCode.Movmm, Comment = "Assign a value from memory to another memory location")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing destination address")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing source address")]
		public static void Movmm(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];
			var value = cpu.Read(vAddr);

			vAddr = cpu.Registers[rX];
			cpu.Write(vAddr, value);
		}

		[OpCode(OpCode.Movrm, Comment = "Assign a value from a register to a memory location")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing destination address")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register to be written")]
		public static void Movrm(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rX];
			cpu.Write(vAddr, cpu.Registers[rY]);
		}

		[OpCode(OpCode.Printr, Comment = "Print the value in a register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register to print")]
		public static void Printr(Cpu cpu, uint rX)
		{
			cpu.Print(cpu.Registers[rX]);
		}

		[OpCode(OpCode.Printm, Comment = "Print a value in memory")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing memory address")]
		public static void Printm(Cpu cpu, uint rX)
		{
			var vAddr = cpu.Registers[rX];
			var value = cpu.Read(vAddr);
			cpu.Print(value);
		}

		[OpCode(OpCode.Jmp, Comment = "Jump to an instruction relative to the current instruction. Value may be negative.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing number of instructions to jump")]
		public static void Jmp(Cpu cpu, uint rX)
		{
			cpu.Jump(cpu.Registers[rX]);
		}

		[OpCode(OpCode.Jlt, Comment = "Jump to an instruction relative to the current instruction when Sf is set. Value may be negative.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing number of instructions to jump")]
		public static void Jlt(Cpu cpu, uint rX)
		{
			if (cpu.Sf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		[OpCode(OpCode.Jgt, Comment = "Jump to an instruction relative to the current instruction when SF is unset. Value may be negative.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing number of instructions to jump")]
		public static void Jgt(Cpu cpu, uint rX)
		{
			if (!cpu.Sf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		[OpCode(OpCode.Je, Comment = "Jump to an instruction relative to the current instruction when ZF is set. Value may be negative.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing number of instructions to jump")]
		public static void Je(Cpu cpu, uint rX)
		{
			if (cpu.Zf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		[OpCode(OpCode.Jne, Comment = "Jump to an instruction relative to the current instruction when ZF is unset. Value may be negative.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing number of instructions to jump")]
		public static void Jne(Cpu cpu, uint rX)
		{
			if (!cpu.Zf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		[OpCode(OpCode.Cmpi, Comment = "Compare a register and a constant value. Set ZF if values are equal, SF if rX < iValue")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing value to compare")]
		[Parameter("iValue", Type = ParamType.Constant, Comment = "Value to compare against")]
		public static void Cmpi(Cpu cpu, uint rX, uint iValue)
		{
			var lValue = cpu.Registers[rX];

			cpu.Sf = (lValue < iValue);
			cpu.Zf = (lValue == iValue);
		}

		[OpCode(OpCode.Cmpr, Comment = "Compare two registers. Set ZF if values are equal, SF if rX < rY")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing value to compare")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register to compare against")]
		public static void Cmpr(Cpu cpu, uint rX, uint rY)
		{
			var lValue = cpu.Registers[rX];
			var rValue = cpu.Registers[rY];

			cpu.Sf = (lValue < rValue);
			cpu.Zf = (lValue == rValue);
		}

		[OpCode(OpCode.Cmprm, Comment = "Compare two registers. Set ZF if values are equal, SF if rX < rY")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing value to compare")]
		[Parameter("rY", Type = ParamType.Constant, Comment = "Register pointing to memory address to compare against")]
		public static void Cmprm(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];

			var lValue = cpu.Registers[rX];
			var rValue = cpu.Read(vAddr);

			cpu.Sf = (lValue < rValue);
			cpu.Zf = (lValue == rValue);
		}

		[OpCode(OpCode.Call, Comment = "Call the function offset from the current instruction by a register; The address of the next instruction to execute after a RET is pushed on the stack.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the instruction offset")]
		public static void Call(Cpu cpu, uint rX)
		{
			var value = cpu.Registers[rX];
			cpu.Call(value);
		}

		[OpCode(OpCode.Callm, Comment = "Call the function offset from the current instruction by a memory address pointed to by a register; The address of the next instruction to execute after a RET is pushed on the stack.")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register pointing to the memory address containing the instruction offset")]
		public static void Callm(Cpu cpu, uint rX)
		{
			var vAddr = cpu.Registers[rX];
			var value = cpu.Read(vAddr);
			cpu.Call(value);
		}

		[OpCode(OpCode.Ret, Comment = "Returns control to the next instruction after the last call")]
		public static void Return(Cpu cpu)
		{
			cpu.Return();
		}

		[OpCode(OpCode.Alloc, Comment = "CreatePage memory")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Size required")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register to hold allocated address (0 if allocation was not successful)")]
		public static void Alloc(Cpu cpu, uint rX, uint rY)
		{
			cpu.Registers[rY] = cpu.Allocate(cpu.Registers[rX]);
		}

		[OpCode(OpCode.Acquire, Comment = "Acquire the operating system lock whose number is provided in the register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the lock number")]
		public static void AcquireLock(Cpu cpu, uint rX)
		{
			var lockNo = cpu.Registers[rX];
			cpu.AcquireLock(lockNo);
		}

		[OpCode(OpCode.Release, Comment = "Release the lock whose number is provided in the register; If the lock is not held by the current process, the instruction is a no-op")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the lock number")]
		public static void ReleaseLock(Cpu cpu, uint rX)
		{
			var lockNo = cpu.Registers[rX];
			cpu.ReleaseLock(lockNo);
		}

		[OpCode(OpCode.Signal, Comment = "Signal the operating system event whose number is provided in the register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the event number")]
		public static void SignalEvent(Cpu cpu, uint rX)
		{
			var eventNo = cpu.Registers[rX];
			cpu.SignalEvent(eventNo);
		}

		[OpCode(OpCode.Wait, Comment = "Wait for an operating system event to become signalled")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the event number")]
		public static void WaitEvent(Cpu cpu, uint rX)
		{
			var eventNo = cpu.Registers[rX];
			cpu.WaitEvent(eventNo);
		}

		[OpCode(OpCode.Sleep, Comment = "Sleep the number of clock cycles as indicated in r1. If the time to sleep is 0, the process sleeps infinitely")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the number of ticks to sleep")]
		public static void Sleep(Cpu cpu, uint rX)
		{
			var sleep = cpu.Registers[rX];
			cpu.Sleep(sleep);
		}

		[OpCode(OpCode.SetP, Comment = "Set the priority of the current process to the value in register")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the new priority")]
		public static void SetPriority(Cpu cpu, uint rX)
		{
			var priority = cpu.Registers[rX];
			cpu.AdjustPriority((byte) (priority & 0xFF));
		}

		[OpCode(OpCode.Exit, Comment = "Terminates the current process")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Published exit code")]
		public static void Exit(Cpu cpu, uint rX)
		{
			var exitCode = cpu.Registers[rX];
			cpu.Exit(exitCode);
		}

		[OpCode(OpCode.TermP, Comment = "Terminates another process")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register pointing to process to terminate")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Published exit code")]
		public static void TerminateProcess(Cpu cpu, uint rX, uint rY)
		{
			var pId = cpu.Registers[rX];
			var exitCode = cpu.Registers[rY];
			cpu.TerminateProcess(pId, exitCode);
		}

		[OpCode(OpCode.Free, Comment = "Free memory previously allocated, pointed to by a register ")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the address to free")]
		public static void FreeMemory(Cpu cpu, uint rX)
		{
			var offset = cpu.Registers[rX];
			cpu.Free(offset);
		}

		[OpCode(OpCode.Clear, Comment = "Zero-out memory pointed to by a register for X bytes")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the address to clear")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing the number of bytes to clear")]
		public static void MemoryClear(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rX];
		    var count = cpu.Registers[rY];

			cpu.MemoryClear(vAddr, count);
		}

		[OpCode(OpCode.MapShared, Comment = "Map a shared memory portion into the address space")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the address to clear")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing the number of bytes to clear")]
		public static void MapShared(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rX];
		    var count = cpu.Registers[rY];

            while (--count > 0)
            {
                cpu.Write(vAddr, 0);
                vAddr += 4;
            }
		}

		[OpCode(OpCode.Input, Comment = "Block process waiting for key press")]
		[Parameter("rX", Type = ParamType.Register, Comment = "Register containing the address to clear")]
		[Parameter("rY", Type = ParamType.Register, Comment = "Register containing the number of bytes to clear")]
		public static void Input(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rX];
		    var count = cpu.Registers[rY];

            while (--count > 0)
            {
                cpu.Write(vAddr, 0);
                vAddr += 4;
            }
		}
	}
}