using tinyOS;

namespace Andy.TinyOS
{
	public class FluentWriter
	{
		private readonly CodeStream _codeStream;

		public FluentWriter(CodeStream codeStream)
		{
			_codeStream = codeStream;
		}

		/// <summary>
		/// Assign a register to a constant value
		/// </summary>
		/// <param name="destination">Register receiving value</param>
		/// <param name="value">Value to store in register</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Movi(Register destination, uint value, string comment = null)
		{
			_codeStream.Add(OpCode.Movi, comment, destination, value);
			return this;
		}

		/// <summary>
		/// Assign a register the same value as another register
		/// </summary>
		/// <param name="destination">Register receiving value</param>
		/// <param name="source">Register containing value</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Movr(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Movr, comment, destination, source);
			return this;
		}

		/// <summary>
		/// Invert the value of a register
		/// </summary>
		/// <param name="destination">Register to bit-flip</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Not(Register destination, string comment = null)
		{
			_codeStream.Add(OpCode.Not, comment, destination);
			return this;
		}

		/// <summary>
		/// Negate the value in a register
		/// </summary>
		/// <param name="destination">Register to negate</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Neg(Register destination, string comment = null)
		{
			_codeStream.Add(OpCode.Neg, comment, destination);
			return this;
		}

		/// <summary>
		/// Terminates the current process
		/// </summary>
		/// <param name="exitValue">Published exit code</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Exit(Register exitValue, string comment = null)
		{
			_codeStream.Add(OpCode.Exit, comment, exitValue);
			return this;
		}

		/// <summary>
		/// Push the value of a register onto the stack
		/// </summary>
		/// <param name="source">Register to push</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Pushr(Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Pushr, comment, source);
			return this;
		}

		/// <summary>
		/// Pop a value off the stack and into a
		/// </summary>
		/// <param name="destination">Register to receive the value</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Popr(Register destination, string comment = null)
		{
			_codeStream.Add(OpCode.Popr, comment, destination);
			return this;
		}

		/// <summary>Add the values of two registers
		/// </summary>
		/// <param name="destination">Register containing value of first operand and destination</param>
		/// <param name="source">Register containing second operand</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Addr(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Addr, comment, destination, source);
			return this;
		}

		/// <summary>
		/// Multiply two values together
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Mul(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Mul, comment, destination, source);
			return this;
		}

		/// <summary>
		/// Bitwise XOR two values together
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Xor(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Xor, comment, destination, source);
			return this;
		}

		/// <summary>
		/// Bitwise OR two values together
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Or(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Or, comment, destination, source);
			return this;
		}

		/// <summary>
		/// Bitwise AND two values together
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="source"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter And(Register destination, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.And, comment, destination, source);
			return this;
		}

		public FluentWriter Div(Register numerator, Register denominator)
		{
			throw new System.NotImplementedException();
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