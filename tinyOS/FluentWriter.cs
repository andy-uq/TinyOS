using tinyOS;

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

		public IFluentDestinationSourceInstructionWriter Mov
		{
			get { return new FluentInstructionWriter(this, OpCode.Mov); }
		}

		public IFluentDestinationSourceInstructionWriter Add
		{
			get { return new FluentInstructionWriter(this, OpCode.Add); }
		}

		public IFluentDestinationSourceInstructionWriter Mul
		{
			get { return new FluentInstructionWriter(this, OpCode.Mul); }
		}

		public IFluentSourceInstructionWriter UnaryOp(OpCode opCode)
		{
			return new FluentInstructionWriter(this, opCode);
		}

		public IFluentDestinationSourceInstructionWriter Div
		{
			get { return new FluentInstructionWriter(this, OpCode.Div); }
		}

		public IFluentDestinationSourceInstructionWriter And
		{
			get { return new FluentInstructionWriter(this, OpCode.And); }
		}

		public IFluentDestinationSourceInstructionWriter Or
		{
			get { return new FluentInstructionWriter(this, OpCode.Or); }
		}

		public IFluentDestinationSourceInstructionWriter Xor
		{
			get { return new FluentInstructionWriter(this, OpCode.Xor); }
		}

		public IFluentDestinationSourceInstructionWriter Not
		{
			get { return new FluentInstructionWriter(this, OpCode.Not); }
		}

		public IFluentDestinationSourceInstructionWriter Neg
		{
			get { return new FluentInstructionWriter(this, OpCode.Neg); }
		}

		public IFluentDestinationSourceInstructionWriter Cmpi
		{
			get { return new FluentInstructionWriter(this, OpCode.Cmp); }
		}

		public IFluentSourceInstructionWriter Exit
		{
			get { return new FluentInstructionWriter(this, OpCode.Exit); }
		}

		public IFluentSourceInstructionWriter Push
		{
			get { return new FluentInstructionWriter(this, OpCode.Push); }
		}

		public IFluentSourceInstructionWriter Jmp
		{
			get { return new FluentInstructionWriter(this, OpCode.Jmp); }
		}

		public IFluentSourceInstructionWriter Jlt
		{
			get { return new FluentInstructionWriter(this, OpCode.Jlt); }
		}

		public IFluentSourceInstructionWriter Jgt
		{
			get { return new FluentInstructionWriter(this, OpCode.Jgt); }
		}

		public IFluentSourceInstructionWriter Je
		{
			get { return new FluentInstructionWriter(this, OpCode.Je); }
		}

		public IFluentSourceInstructionWriter Jne
		{
			get { return new FluentInstructionWriter(this, OpCode.Jne); }
		}

		public IFluentSourceInstructionWriter Print
		{
			get { return new FluentInstructionWriter(this, OpCode.Print); }
		}

		public IFluentDestinationInstructionWriter Incr
		{
			get { return new FluentInstructionWriter(this, OpCode.Incr); }
		}

		public IFluentDestinationInstructionWriter Pop
		{
			get { return new FluentInstructionWriter(this, OpCode.Pop); }
		}
	}


	public static class CodeStreamFluentExtensions
	{
		public static FluentWriter AsFluent(this CodeStream stream)
		{
			return new FluentWriter(stream);
		}
	}
}