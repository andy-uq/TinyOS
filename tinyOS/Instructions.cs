using System;

namespace tinyOS
{
	public static class Instructions
	{
		 public static void Incr(Cpu cpu, uint rX)
		 {
		 	unchecked
			{
				cpu.Registers[rX]++;
			}
		 }

		 public static void Addi(Cpu cpu, uint rX, uint iValue)
		{
			unchecked
			{
				cpu.Registers[rX] += iValue;
			}
		}

		public static void Addr(Cpu cpu, uint rX, uint rY)
		{
			unchecked
			{
				cpu.Registers[rX] += cpu.Registers[rY];
			}
		}

		public static void Pushr(Cpu cpu, uint rX)
		{
			cpu.Push(cpu.Registers[rX]);
		}

		public static void Pushi(Cpu cpu, uint iValue)
		{
			cpu.Push(iValue);
		}

		public static void Popr(Cpu cpu, uint rX)
		{
			cpu.Registers[rX] = cpu.Pop();
		}

		public static void Popm(Cpu cpu, uint rX)
		{
			var bytes = BitConverter.GetBytes(cpu.Pop());
			
			var vAddr = cpu.Registers[rX];
			var ptr = cpu.Translate(vAddr);
			Array.Copy(bytes, 0, cpu.Ram, ptr, 4);
		}

		public static void Movi(Cpu cpu, uint rX, uint iValue)
		{
			cpu.Registers[rX] = iValue;
		}

		public static void Movr(Cpu cpu, uint rX, uint rY)
		{
			cpu.Registers[rX] = cpu.Registers[rY];
		}

		public static void Movmr(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];
			cpu.Registers[rX] = cpu.Read(vAddr);
		}

		public static void Movmm(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];
			var value = cpu.Read(vAddr);

			vAddr = cpu.Registers[rX];
			cpu.Write(vAddr, value);
		}

		public static void Movrm(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rY];
			cpu.Write(vAddr, cpu.Registers[rX]);
		}

		public static void Printr(Cpu cpu, uint rX)
		{
			cpu.Print(cpu.Registers[rX]);
		}

		public static void Printm(Cpu cpu, uint rX)
		{
			var vAddr = cpu.Registers[rX];
			var value = cpu.Read(vAddr);
			cpu.Print(value);
		}

		public static void Jmp(Cpu cpu, uint rX)
		{
			cpu.Jump(cpu.Registers[rX]);
		}

		public static void Jlt(Cpu cpu, uint rX)
		{
			if (cpu.Sf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		public static void Jgt(Cpu cpu, uint rX)
		{
			if (!cpu.Sf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		public static void Je(Cpu cpu, uint rX)
		{
			if (cpu.Zf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		public static void Jne(Cpu cpu, uint rX)
		{
			if (!cpu.Zf)
			{
				cpu.Jump(cpu.Registers[rX]);
			}
		}

		public static void Cmpi(Cpu cpu, uint rX, uint rValue)
		{
			var lValue = cpu.Registers[rX];

			cpu.Sf = (lValue < rValue);
			cpu.Zf = (lValue == rValue);
		}

		public static void Cmpr(Cpu cpu, uint rX, uint rY)
		{
			var lValue = cpu.Registers[rX];
			var rValue = cpu.Registers[rY];

			cpu.Sf = (lValue < rValue);
			cpu.Zf = (lValue == rValue);
		}

		public static void Call(Cpu cpu, uint rX)
		{
			var value = cpu.Registers[rX];
			cpu.Call(value);
		}

		public static void Callm(Cpu cpu, uint rX)
		{
			var vAddr = cpu.Registers[rX];
			var value = cpu.Read(vAddr);
			cpu.Call(value);
		}

		public static void Return(Cpu cpu)
		{
			cpu.Return();
		}

		public static void Alloc(Cpu cpu, uint rX, uint rY)
		{
			cpu.Registers[rY] = cpu.Allocate(cpu.Registers[rX]);
		}

		public static void AcquireLock(Cpu cpu, uint rX)
		{
			var lockNo = cpu.Registers[rX];
			cpu.AcquireLock(lockNo);
		}

		public static void ReleaseLock(Cpu cpu, uint rX)
		{
			var lockNo = cpu.Registers[rX];
			cpu.ReleaseLock(lockNo);
		}

		public static void Sleep(Cpu cpu, uint rX)
		{
			var sleep = cpu.Registers[rX];
			cpu.Sleep(sleep);
		}

		public static void SetPriority(Cpu cpu, uint rX)
		{
			var priority = cpu.Registers[rX];
			cpu.AdjustPriority((byte )(priority & 0xFF));
		}

		public static void Exit(Cpu cpu, uint rX)
		{
			var exitCode = cpu.Registers[rX];
			cpu.Exit(exitCode);
		}

		public static void TerminateProcess(Cpu cpu, uint rX)
		{
			var pId = cpu.Registers[rX];
			cpu.TerminateProcess(pId);
		}

		public static void FreeMemory(Cpu cpu, uint rX)
		{
			var offset = cpu.Registers[rX];
			cpu.Free(offset);
		}

		public static void MemoryClear(Cpu cpu, uint rX, uint rY)
		{
			var vAddr = cpu.Registers[rX];
			var addr = cpu.Translate(vAddr);
			var end = addr + cpu.Registers[rY];
			while ( addr < end )
				cpu.Ram[addr++] = 0;
		}
	}
}