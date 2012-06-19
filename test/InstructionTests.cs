using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	public static class Register
	{
		public const uint A = 1;
		public const uint B = 2;
		public const uint C = 3;
		public const uint Sp = 4;
	}

	[TestFixture]
	public class InstructionTests
	{
		private Cpu _cpu;
		private int _heapOffset;

		private uint A { get { return _cpu.Registers[1]; } }
		private uint B { get { return _cpu.Registers[2]; } }
		private uint C { get { return _cpu.Registers[3]; } }
		private uint Sp { get { return _cpu.Sp; } }

		[SetUp]
		public void SetUp()
		{
			uint pId = 10;

			_cpu = new Cpu(1024);
			_cpu.CurrentProcess = new ProcessContextBlock { Id = pId, Stack = _cpu.MemoryManager.Allocate(pId, 128), Code = _cpu.MemoryManager.Allocate(pId, 128) };
			var pageInfo = _cpu.CurrentProcess.PageTable.Last();
			_heapOffset = (int )(pageInfo.PhysicalOffset + pageInfo.Size);
		}

		[Test]
		public void Addi()
		{
			Instructions.Addi(_cpu, Register.A, 100);
			Assert.That(A, Is.EqualTo(100));

			Instructions.Addi(_cpu, Register.A, 50);
			Assert.That(A, Is.EqualTo(150));
		}

		[Test]
		public void Incri()
		{
			Instructions.Incr(_cpu, Register.A);
			Assert.That(A, Is.EqualTo(1));
		}

		[Test]
		public void Movi()
		{
			Instructions.Movi(_cpu, Register.A, 1000);
			Assert.That(A, Is.EqualTo(1000));
		}

		[Test]
		public void Movr()
		{
			Instructions.Movi(_cpu, Register.A, 1000);
			Instructions.Movr(_cpu, Register.B, Register.A);
			Assert.That(B, Is.EqualTo(1000));
		}

		[Test]
		public void Addr()
		{
			Instructions.Movi(_cpu, Register.A, 1000);
			Instructions.Movi(_cpu, Register.B, 1);
			Instructions.Addr(_cpu, Register.A, Register.B);
			Assert.That(A, Is.EqualTo(1001));
		}

		[Test]
		public void Pushr()
		{
			Instructions.Movi(_cpu, Register.A, 1000);
			Instructions.Pushr(_cpu, Register.A);
			Assert.That(Sp, Is.EqualTo(4));
			Assert.That(_cpu.Peek(), Is.EqualTo(1000));
		}

		[Test]
		public void Pushi()
		{
			Instructions.Pushi(_cpu, 101);
			Assert.That(Sp, Is.EqualTo(4));
			Assert.That(_cpu.Peek(), Is.EqualTo(101));
		}

		[Test]
		public void Pop()
		{
			Instructions.Pushi(_cpu, 101);
			Assert.That(Sp, Is.EqualTo(4));
			Instructions.Popr(_cpu, Register.A);
			Assert.That(A, Is.EqualTo(101));
		}

		[Test]
		public void Cmpi()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Cmpi(_cpu, Register.A, 20);
			Assert.That(_cpu.Sf, Is.True);
			Assert.That(_cpu.Zf, Is.False);

			Instructions.Movi(_cpu, Register.A, 30);
			Instructions.Cmpi(_cpu, Register.A, 20);
			Assert.That(_cpu.Sf, Is.False);
			Assert.That(_cpu.Zf, Is.False);

			Instructions.Movi(_cpu, Register.A, 40);
			Instructions.Cmpi(_cpu, Register.A, 40);
			Assert.That(_cpu.Zf, Is.True);
		}

		[Test]
		public void Jmp()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Jmp(_cpu, Register.A);
			Assert.That(_cpu.Ip, Is.EqualTo(10));

			Instructions.Movi(_cpu, Register.A, AsUint(-10));
			Instructions.Jmp(_cpu, Register.A);
			Assert.That(_cpu.Ip, Is.EqualTo(0));
		}

		[Test]
		public void Je()
		{
			Instructions.Movi(_cpu, Register.C, AsUint(10));
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Cmpi(_cpu, Register.A, 10);
			Instructions.Je(_cpu, Register.C);
			Assert.That(_cpu.Ip, Is.EqualTo(10));

			Instructions.Movi(_cpu, Register.C, AsUint(-10));
			Instructions.Je(_cpu, Register.C);
			Assert.That(_cpu.Ip, Is.EqualTo(0));
		}

		[Test]
		public void Jlt()
		{
			Instructions.Movi(_cpu, Register.C, AsUint(10));
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Cmpi(_cpu, Register.A, 20);
			Instructions.Jlt(_cpu, Register.C);
			Assert.That(_cpu.Ip, Is.EqualTo(10));

			Instructions.Movi(_cpu, Register.C, AsUint(-10));
			Instructions.Jlt(_cpu, Register.C);
			Assert.That(_cpu.Ip, Is.EqualTo(0));
		}

		private static uint AsUint(int value)
		{
			unchecked
			{
				return (uint) value;
			}
		}

		[Test]
		public void Cmpr()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Movi(_cpu, Register.B, 20);
			Instructions.Cmpr(_cpu, Register.A, Register.B);
			Assert.That(_cpu.Sf, Is.True);
			Assert.That(_cpu.Zf, Is.False);

			Instructions.Movi(_cpu, Register.A, 30);
			Instructions.Cmpr(_cpu, Register.A, Register.B);
			Assert.That(_cpu.Sf, Is.False);
			Assert.That(_cpu.Zf, Is.False);

			Instructions.Movi(_cpu, Register.B, 30);
			Instructions.Cmpr(_cpu, Register.A, Register.B);
			Assert.That(_cpu.Zf, Is.True);
		}

		[Test]
		public void Alloc()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			Assert.That(B, Is.EqualTo(_heapOffset));
			Assert.That(_cpu.CurrentProcess.PageTable.Count(), Is.EqualTo(3));
		}

		[Test]
		public void Movrm()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			
			Instructions.Movi(_cpu, Register.A, 88);
			Instructions.Movrm(_cpu, Register.B, Register.A);

			var value = BitConverter.ToUInt32(_cpu.Ram, _heapOffset);
			Assert.That(value, Is.EqualTo(88));
		}

		[Test]
		public void MemoryClear()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			
			Instructions.Movi(_cpu, Register.A, 0x12345678);
			Instructions.Pushr(_cpu, Register.B);
			Instructions.Movrm(_cpu, Register.B, Register.A);
			Instructions.Addi(_cpu, Register.B, 4);
			Instructions.Movrm(_cpu, Register.B, Register.A);
			Instructions.Addi(_cpu, Register.B, 4);
			Instructions.Movrm(_cpu, Register.B, Register.A);
			Instructions.Popr(_cpu, Register.B);
			
			var value = BitConverter.ToUInt64(_cpu.Ram, _heapOffset);
			Assert.That(value, Is.EqualTo(0x1234567812345678));

			Instructions.Pushr(_cpu, Register.B);
			Instructions.Addi(_cpu, Register.B, 1);
			Instructions.Movi(_cpu, Register.A, 4);
			Instructions.MemoryClear(_cpu, Register.B, Register.A);
			Instructions.Popr(_cpu, Register.B);

			value = BitConverter.ToUInt64(_cpu.Ram, _heapOffset);
			Console.WriteLine(value.ToString("x8"));
			Assert.That(value, Is.EqualTo(0x1234560000000078));

			Instructions.FreeMemory(_cpu, Register.B);
		}

		[Test]
		public void Movmr()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			
			Array.Copy(BitConverter.GetBytes(88), 0, _cpu.Ram, _heapOffset, 4);

			Instructions.Movmr(_cpu, Register.A, Register.B);
			
			Assert.That(A, Is.EqualTo(88));
		}

		[Test]
		public void Movmm()
		{
			Instructions.Movi(_cpu, Register.A, 4);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			Instructions.Alloc(_cpu, Register.A, Register.C);
			
			Assert.That(B, Is.EqualTo(_heapOffset + 0));
			Assert.That(C, Is.EqualTo(_heapOffset + 4));

			Array.Copy(BitConverter.GetBytes(88), 0, _cpu.Ram, _heapOffset, 4);

			Instructions.Movmm(_cpu, Register.C, Register.B);

			var value = BitConverter.ToUInt32(_cpu.Ram, _heapOffset + 4);
			Assert.That(value, Is.EqualTo(88));
		}

		[Test]
		public void Free()
		{
			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			Assert.That(B, Is.Not.EqualTo(0));
			Instructions.FreeMemory(_cpu, Register.B);
		}

		[Test]
		public void Printr()
		{
			var printStack = new Stack<uint>();
			_cpu.PrintMethod = printStack.Push;

			Instructions.Movi(_cpu, Register.A, 10);
			Instructions.Printr(_cpu, Register.A);
			Assert.That(printStack.Peek(), Is.EqualTo(10));
		}

		[Test]
		public void Printm()
		{
			var printStack = new Stack<uint>();
			_cpu.PrintMethod = printStack.Push;

			Instructions.Movi(_cpu, Register.A, 4);
			Instructions.Alloc(_cpu, Register.A, Register.B);
			Instructions.Movi(_cpu, Register.A, 99);
			Instructions.Movrm(_cpu, Register.B, Register.A);
			Instructions.Printm(_cpu, Register.B);
			Assert.That(printStack.Peek(), Is.EqualTo(99));
		}
	}
}