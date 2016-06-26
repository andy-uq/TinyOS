namespace Andy.TinyOS
{

	public interface IFluentSourceInstructionWriter
	{
		FluentWriter I(int value);
		FluentWriter U(uint value);
		FluentWriter R(Register register);
		FluentWriter M(MemoryAddress memoryAddress);
	}

	public interface IFluentDestinationInstructionWriter
	{
		FluentWriter R(Register register);
		FluentWriter M(MemoryAddress memoryAddress);
	}

	public interface IFluentDestinationSourceInstructionWriter
	{
		FluentWriter RI(Register destination, int value);
		FluentWriter RU(Register destination, uint value, string comment = null);
		FluentWriter RR(Register destination, Register source);
		FluentWriter RM(Register destination, MemoryAddress sourceAddress);

		FluentWriter MI(MemoryAddress destinationAddress, int value);
		FluentWriter MU(MemoryAddress destinationAddress, uint value);
		FluentWriter MR(MemoryAddress destinationAddress, Register source);
		FluentWriter MM(MemoryAddress destinationAddress, MemoryAddress sourceAddress);
	}

	public class FluentWriter
	{
		private readonly CodeStream _codeStream;

		private class FluentInstructionWriter : IFluentSourceInstructionWriter, IFluentDestinationInstructionWriter, IFluentDestinationSourceInstructionWriter
		{
			private readonly FluentWriter _parent;
			private readonly OpCode _opCode;

			public FluentInstructionWriter(FluentWriter parent, OpCode opCode)
			{
				_parent = parent;
				_opCode = opCode;
			}

			public FluentWriter I(int value)
			{
				var instruction = new Instruction(_opCode).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter U(uint value)
			{
				var instruction = new Instruction(_opCode).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			FluentWriter IFluentSourceInstructionWriter.R(Register register)
			{
				var instruction = new Instruction(_opCode).Source(register);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			FluentWriter IFluentDestinationInstructionWriter.M(MemoryAddress memoryAddress)
			{
				var instruction = new Instruction(_opCode).Destination(memoryAddress);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			FluentWriter IFluentDestinationInstructionWriter.R(Register register)
			{
				var instruction = new Instruction(_opCode).Destination(register);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			FluentWriter IFluentSourceInstructionWriter.M(MemoryAddress memoryAddress)
			{
				var instruction = new Instruction(_opCode).Destination(memoryAddress);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter RI(Register destination, int value)
			{
				var instruction = new Instruction(_opCode).Destination(destination).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter RU(Register destination, uint value, string comment)
			{
				var instruction = new Instruction(_opCode, comment).Destination(destination).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter RR(Register destination, Register source)
			{
				var instruction = new Instruction(_opCode).Destination(destination).Source(source);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter RM(Register destination, MemoryAddress sourceAddress)
			{
				var instruction = new Instruction(_opCode).Destination(destination).Source(sourceAddress);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter MI(MemoryAddress destinationAddress, int value)
			{
				var instruction = new Instruction(_opCode).Destination(destinationAddress).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter MU(MemoryAddress destinationAddress, uint value)
			{
				var instruction = new Instruction(_opCode).Destination(destinationAddress).Source(value);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter MR(MemoryAddress destinationAddress, Register source)
			{
				var instruction = new Instruction(_opCode).Destination(destinationAddress).Source(source);
				_parent._codeStream.Add(instruction);
				return _parent;
			}

			public FluentWriter MM(MemoryAddress destinationAddress, MemoryAddress sourceAddress)
			{
				var instruction = new Instruction(_opCode).Destination(destinationAddress).Source(sourceAddress);
				_parent._codeStream.Add(instruction);
				return _parent;
			}
		}

		public FluentWriter(CodeStream codeStream)
		{
			_codeStream = codeStream;
		}

		public IFluentDestinationSourceInstructionWriter Mov => new FluentInstructionWriter(this, OpCode.Mov);

		public IFluentDestinationSourceInstructionWriter Add => new FluentInstructionWriter(this, OpCode.Add);

		public IFluentDestinationSourceInstructionWriter Mul => new FluentInstructionWriter(this, OpCode.Mul);

		public IFluentSourceInstructionWriter UnaryOp(OpCode opCode)
		{
			return new FluentInstructionWriter(this, opCode);
		}

		public IFluentDestinationSourceInstructionWriter Div => new FluentInstructionWriter(this, OpCode.Div);

		public IFluentDestinationSourceInstructionWriter And => new FluentInstructionWriter(this, OpCode.And);

		public IFluentDestinationSourceInstructionWriter Or => new FluentInstructionWriter(this, OpCode.Or);

		public IFluentDestinationSourceInstructionWriter Xor => new FluentInstructionWriter(this, OpCode.Xor);

		public IFluentDestinationSourceInstructionWriter Not => new FluentInstructionWriter(this, OpCode.Not);

		public IFluentDestinationSourceInstructionWriter Neg => new FluentInstructionWriter(this, OpCode.Neg);

		public IFluentDestinationSourceInstructionWriter Cmpi => new FluentInstructionWriter(this, OpCode.Cmp);

		public IFluentSourceInstructionWriter Exit => new FluentInstructionWriter(this, OpCode.Exit);

		public IFluentSourceInstructionWriter Push => new FluentInstructionWriter(this, OpCode.Push);

		public IFluentSourceInstructionWriter Jmp => new FluentInstructionWriter(this, OpCode.Jmp);

		public IFluentSourceInstructionWriter Jlt => new FluentInstructionWriter(this, OpCode.Jlt);

		public IFluentSourceInstructionWriter Jgt => new FluentInstructionWriter(this, OpCode.Jgt);

		public IFluentSourceInstructionWriter Je => new FluentInstructionWriter(this, OpCode.Je);

		public IFluentSourceInstructionWriter Jne => new FluentInstructionWriter(this, OpCode.Jne);

		public IFluentDestinationSourceInstructionWriter Output => new FluentInstructionWriter(this, OpCode.Output);

		public IFluentDestinationInstructionWriter Incr => new FluentInstructionWriter(this, OpCode.Incr);

		public IFluentDestinationInstructionWriter Pop => new FluentInstructionWriter(this, OpCode.Pop);
	}


	public static class CodeStreamFluentExtensions
	{
		public static FluentWriter AsFluent(this CodeStream stream)
		{
			return new FluentWriter(stream);
		}
	}
}