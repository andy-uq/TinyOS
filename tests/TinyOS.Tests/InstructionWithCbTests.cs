using System;
using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class InstructionTests
	{
		private Cpu _cpu;
		private int _heapOffset;
		private int _sharedOffset;
		private readonly byte[] _ram = new byte[1024];

		private uint A => _cpu.Registers[Register.A];
		private uint B => _cpu.Registers[Register.B];
		private uint C => _cpu.Registers[Register.C];
		private uint H => _cpu.Registers[Register.H];
		private uint Sp => _cpu.Sp;

		public InstructionTests()
		{
			const uint pId = 10;

			var ram = new Ram(_ram, 16);
			_sharedOffset  = (int) ram.AllocateShared(256);

			_cpu = new Cpu(ram);
			_cpu.InputDevice = new InputDevice();

			var p = CreateProcess(pId);
			_cpu.CurrentProcess = p;
			_cpu.CurrentProcess.Registers[Register.H] = p.GlobalData.Offset;

			var heap = _cpu.Ram.Allocate(_cpu.IdleProcess);
			_heapOffset = (int) _cpu.Ram.ToPhysicalAddress(_cpu.IdleProcess.Id, heap.VirtualAddress);
			_cpu.Ram.Free(heap);
		}

		private ProcessContextBlock CreateProcess(uint pId)
		{
			var p = new ProcessContextBlock {Id = pId};
			p.Stack.Append(_cpu.Ram.Allocate(p));
			p.Code.Append(_cpu.Ram.Allocate(p));
			p.GlobalData.Append(_cpu.Ram.Allocate(p));
			return p;
		}

		private void Invoke(Instruction instruction)
		{
			_cpu.Execute(instruction);
		}

		private void Invoke(Action<Cpu, byte, uint> instruction, uint source)
		{
			instruction(_cpu, (byte) OpCodeFlag.Constant << 2, source);
		}

		private void Invoke(Action<Cpu, byte, uint> instruction, Register destination)
		{
			instruction(_cpu, (byte)OpCodeFlag.Register | (byte)OpCodeFlag.Register << 2, destination);
		}

		private void Invoke(Action<Cpu, byte, uint> instruction, MemoryAddress destination)
		{
			instruction(_cpu, (byte)OpCodeFlag.MemoryAddress | (byte)OpCodeFlag.MemoryAddress << 2, destination);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, Register destination, uint value)
		{
			instruction(_cpu, (byte) OpCodeFlag.Register | (byte) OpCodeFlag.Constant << 2, destination, value);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, Register destination, Register source)
		{
			instruction(_cpu, (byte) OpCodeFlag.Register | (byte) OpCodeFlag.Register << 2, destination, source);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, MemoryAddress destination, uint source)
		{
			instruction(_cpu, (byte) OpCodeFlag.MemoryAddress | (byte) OpCodeFlag.Constant << 2, destination, source);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, MemoryAddress destination, MemoryAddress source)
		{
			instruction(_cpu, (byte) OpCodeFlag.MemoryAddress | (byte) OpCodeFlag.MemoryAddress << 2, destination, source);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, MemoryAddress destination, Register source)
		{
			instruction(_cpu, (byte) OpCodeFlag.MemoryAddress | (byte) OpCodeFlag.Register << 2, destination, source);
		}

		private void Invoke(Action<Cpu, byte, uint, uint> instruction, Register destination, MemoryAddress source)
		{
			instruction(_cpu, (byte) OpCodeFlag.Register | (byte) OpCodeFlag.MemoryAddress << 2, destination, source);
		}

		private static uint AsUint(int value)
		{
			unchecked
			{
				return (uint) value;
			}
		}

		[Fact]
		public void Add()
		{
			Invoke(InstructionsWithControlByte.Add, Register.A, 100);
			Invoke(InstructionsWithControlByte.Add, Register.B, 50);
			A.Should().Be(100);

			Invoke(InstructionsWithControlByte.Add, Register.A, Register.B);
			A.Should().Be(150);
		}

		[Fact]
		public void WithDynamic()
		{
			Invoke(new Instruction(OpCode.Mov).Destination(Register.A).Source(100));
			Invoke(new Instruction(OpCode.Mov).Destination(Register.B).Source(50));
			A.Should().Be(100);

			Invoke(new Instruction(OpCode.Add).Destination(Register.A).Source(Register.B));
			A.Should().Be(150);
		}

		[Fact]
		public void Incri()
		{
			var globalData = _cpu.CurrentProcess.GlobalData.Offset;
			H.Should().Be(globalData);

			Invoke(InstructionsWithControlByte.Incr, Register.A);
			Invoke(InstructionsWithControlByte.Incr, MemoryAddress.H);
			A.Should().Be(1);
			H.Should().Be(globalData);
			_cpu.Read(H).Should().Be(1);
		}

		[Fact]
		public void Mov()
		{
			Invoke(InstructionsWithControlByte.Mov, Register.A, 100);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.H, Register.A);
			A.Should().Be(100);
			_cpu.Read(H).Should().Be(100);
		}

		[Fact]
		public void Push()
		{
			Invoke(InstructionsWithControlByte.Push, 1000);
			Sp.Should().Be(4);
			_cpu.Peek().Should().Be(1000);
		}

		[Fact]
		public void Pop()
		{
			Invoke(InstructionsWithControlByte.Push, 1000);
			Invoke(InstructionsWithControlByte.Pop, MemoryAddress.H);
			_cpu.Read(H).Should().Be(1000);
		}

		[Fact]
		public void Cmpi()
		{
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			Invoke(InstructionsWithControlByte.Cmp, Register.A, 20);

			_cpu.Sf.Should().BeTrue();
			_cpu.Zf.Should().BeFalse();

			Invoke(InstructionsWithControlByte.Mov, Register.A, 30);
			Invoke(InstructionsWithControlByte.Cmp, Register.A, 20);
			_cpu.Sf.Should().BeFalse();
			_cpu.Zf.Should().BeFalse();

			Invoke(InstructionsWithControlByte.Cmp, Register.A, 30);
			_cpu.Zf.Should().BeTrue();
		}

		[Fact]
		public void Jmp()
		{
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Jmp, Register.A);
			_cpu.CurrentProcess.Ip++;

			_cpu.Ip.Should().Be(11);

			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Jmp, AsUint(-11));
			_cpu.CurrentProcess.Ip++;
			
			_cpu.Ip.Should().Be(1);
		}

		[Fact]
		public void Je()
		{
			Invoke(InstructionsWithControlByte.Mov, Register.C, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Cmp, Register.A, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Je, Register.C);
			_cpu.CurrentProcess.Ip++;
			
			_cpu.Ip.Should().Be(13);

			Invoke(InstructionsWithControlByte.Mov, Register.C, AsUint(-13));
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Je, Register.C);
			_cpu.CurrentProcess.Ip++;
			
			_cpu.Ip.Should().Be(1);
		}

		[Fact]
		public void Jlt()
		{
			Invoke(InstructionsWithControlByte.Mov, Register.C, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Cmp, Register.A, 20);
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Jlt, Register.C);
			_cpu.CurrentProcess.Ip++;

			_cpu.Ip.Should().Be(13);

			Invoke(InstructionsWithControlByte.Mov, Register.C, AsUint(-13));
			_cpu.CurrentProcess.Ip++;
			Invoke(InstructionsWithControlByte.Jlt, Register.C);
			_cpu.CurrentProcess.Ip++;

			_cpu.Ip.Should().Be(1);
		}

		[Fact]
		public void Alloc()
		{
            int count = _cpu.CurrentProcess.PageTable.Count();
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			Invoke(InstructionsWithControlByte.Alloc, Register.B, Register.A);
			
			_cpu.CurrentProcess.PageTable.Count().Should().Be(count + 1);
		}

		[Fact]
		public void Map()
		{
            int count = _cpu.CurrentProcess.PageTable.Count();
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			Invoke(InstructionsWithControlByte.Map, Register.B, Register.A);
			
			Assert.That(B, Is.Not.EqualTo(0));
			_cpu.CurrentProcess.PageTable.Count().Should().Be(count + 1);

			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, 8088);
			Invoke(InstructionsWithControlByte.Mov, Register.C, MemoryAddress.B);
			var value = BitConverter.ToUInt32(_ram, _sharedOffset);
			value.Should().Be(8088);
			C.Should().Be(8088);
		}

		[Fact]
		public void MapIsShared()
		{
			var pA = _cpu.CurrentProcess;
			_cpu.CurrentProcess = new ProcessContextBlock();

			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			Invoke(InstructionsWithControlByte.Map, Register.B, Register.A);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, 8088);

			_cpu.CurrentProcess = pA;
			Invoke(InstructionsWithControlByte.Mov, Register.A, 10);
			Invoke(InstructionsWithControlByte.Map, Register.B, Register.A);
			Invoke(InstructionsWithControlByte.Mov, Register.C, MemoryAddress.B);

			C.Should().Be(8088);
		}

		[Fact]
		public void WaitEvent()
		{
			var pA = _cpu.CurrentProcess;

			Invoke(InstructionsWithControlByte.WaitEvent, 1);

			_cpu.CurrentProcess.Should().BeNull();
			Assert.That(_cpu.DeviceReadQueue.Where(x => x.DeviceId == DeviceId.Event1).Select(x => x.Process), Contains.Item(pA));
		}

		[Fact]
		public void Sleep()
		{
			IdleProcess.Initialise(_cpu);

			var pA = _cpu.CurrentProcess;
			Invoke(InstructionsWithControlByte.Sleep, 10);

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(_cpu.IdleProcess));

			for ( int i = 0; i < 10; i++ )
				_cpu.Tick();

			Assert.That(_cpu.CurrentProcess, Is.SameAs(pA));
		}

		[Fact]
		public void Input()
		{
			IdleProcess.Initialise(_cpu);

			var pA = _cpu.CurrentProcess;
			Invoke(InstructionsWithControlByte.Input, Register.C, (uint )DeviceId.Terminal);

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(_cpu.IdleProcess));
			
			_cpu.InputDevice.Push(99);
			for ( int i = 0; i < 2; i++ )
				_cpu.Tick();

			Assert.That(_cpu.CurrentProcess, Is.SameAs(pA));
			C.Should().Be(99);
		}

		[Fact]
		public void Printr()
		{
			IdleProcess.Initialise(_cpu);
			var pA = _cpu.CurrentProcess;

			var printStack = new Stack<uint>();
			_cpu.OutputMethod = printStack.Push;

			Invoke(InstructionsWithControlByte.Mov, Register.A, (uint)DeviceId.Terminal);
			Invoke(InstructionsWithControlByte.Mov, Register.B, 10);
			Invoke(InstructionsWithControlByte.Output, Register.A, Register.B);

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(_cpu.IdleProcess));

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(pA));

			printStack.Peek().Should().Be(10);
		}

		[Fact]
		public void Printc()
		{
			IdleProcess.Initialise(_cpu);
			var pA = _cpu.CurrentProcess;

			var printStack = new Stack<uint>();
			_cpu.OutputMethod = printStack.Push;

			Invoke(InstructionsWithControlByte.Mov, Register.A, (uint)DeviceId.Terminal);
			Invoke(InstructionsWithControlByte.Output, Register.A, 10);

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(_cpu.IdleProcess));

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(pA));

			printStack.Peek().Should().Be(10);
		}

		[Fact]
		public void Printm()
		{
			IdleProcess.Initialise(_cpu);
			var pA = _cpu.CurrentProcess;
			
			var printStack = new Stack<uint>();
			_cpu.OutputMethod = printStack.Push;

			Invoke(InstructionsWithControlByte.Alloc, Register.B, 4);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, 99);
			Invoke(InstructionsWithControlByte.Mov, Register.A, (uint)DeviceId.Terminal);
			Invoke(InstructionsWithControlByte.Output, Register.A, MemoryAddress.B);
			
			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(_cpu.IdleProcess));

			_cpu.Tick();
			Assert.That(_cpu.CurrentProcess, Is.SameAs(pA));

			printStack.Peek().Should().Be(99);
		}


		[Fact]
		public void SignalEvent()
		{
			var pA = _cpu.CurrentProcess;

			Invoke(InstructionsWithControlByte.WaitEvent, 1);

			var pB = new ProcessContextBlock();
			_cpu.CurrentProcess = pB;
			Invoke(InstructionsWithControlByte.SignalEvent, 1);

			_cpu.CurrentProcess.Should().Be(pB);
			Assert.That(_cpu.ReadyQueue, Contains.Item(pA));
			Assert.That(_cpu.DeviceReadQueue, Is.Empty);
		}

		[Fact]
		public void Movrm()
		{
			Invoke(InstructionsWithControlByte.Alloc, Register.B, 10);
			Invoke(InstructionsWithControlByte.Mov, Register.A, 88);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, Register.A);

			var value = BitConverter.ToUInt32(_ram, _heapOffset);
			value.Should().Be(88);
		}

		[Fact]
		public void Movmr()
		{
			Invoke(InstructionsWithControlByte.Alloc, Register.B, 10);
			Invoke(InstructionsWithControlByte.Mov, Register.A, 88);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, Register.A);
			Invoke(InstructionsWithControlByte.Mov, Register.C, MemoryAddress.B);
			
			C.Should().Be(88);
		}

		[Fact]
		public void MemoryClear()
		{
			Invoke(InstructionsWithControlByte.Alloc, Register.B, 10);
			Invoke(InstructionsWithControlByte.Push, Register.B);

			Invoke(InstructionsWithControlByte.Mov, Register.A, 0x12345678);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, Register.A);
			var value = (ulong )BitConverter.ToUInt32(_ram, _heapOffset);
			value.Should().Be(0x12345678L);

			Invoke(InstructionsWithControlByte.Add, Register.B, 4);
			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.B, Register.A);
			
			value = BitConverter.ToUInt64(_ram, _heapOffset);
			value.Should().Be(0x1234567812345678);
			
			Invoke(InstructionsWithControlByte.Pop, Register.B);

			Invoke(InstructionsWithControlByte.Push, Register.B);
			Invoke(InstructionsWithControlByte.Add, Register.B, 1);
			Invoke(InstructionsWithControlByte.MemoryClear, Register.B, 4);
			Invoke(InstructionsWithControlByte.Pop, Register.B);

			value = BitConverter.ToUInt64(_ram, _heapOffset);
			Console.WriteLine(value.ToString("x8"));
			value.Should().Be(0x1234560000000078);

			Invoke(InstructionsWithControlByte.FreeMemory, Register.B);
		}

		[Fact]
		public void Movmm()
		{
			Invoke(InstructionsWithControlByte.Alloc, Register.B, 4);
			Invoke(InstructionsWithControlByte.Alloc, Register.C, 4);

			Array.Copy(BitConverter.GetBytes(88), 0, _ram, _heapOffset, 4);

			Invoke(InstructionsWithControlByte.Mov, MemoryAddress.C, MemoryAddress.B);

			var value = BitConverter.ToUInt32(_ram, (int) (_heapOffset + _cpu.Ram.FrameSize));
			value.Should().Be(88);
		}

		[Fact]
		public void Free()
		{
			Invoke(InstructionsWithControlByte.Alloc, Register.B, 4);
			Assert.That(B, Is.Not.EqualTo(0));
			Invoke(InstructionsWithControlByte.FreeMemory, Register.B);
		}
	}
}