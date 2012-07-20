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
		/// Assign a register a value from memory
		/// </summary>
		/// <param name="destination">Register being assigned</param>
		/// <param name="sourceAddr">Register containing source address</param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Movmr(Register destination, Register sourceAddr, string comment = null)
		{
			_codeStream.Add(OpCode.Movmr, comment, destination, sourceAddr);
			return this;
		}

		public FluentWriter Movrm(Register destinationAddr, Register source, string comment = null)
		{
			_codeStream.Add(OpCode.Movrm, comment, destinationAddr, source);
			return this;
		}

		/// <summary>
		/// Assign a register to a constant value
		/// </summary>
		/// <param name="destination">Register receiving value</param>
		/// <param name="value">Value to store in register</param>
		/// <param name="comment">Instruction comment</param>
		public FluentWriter Movi(Register destination, int value, string comment = null)
		{
			_codeStream.Add(OpCode.Movi, comment, destination, unchecked((uint)value));
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

		public FluentWriter Div(Register numerator, Register denominator, string comment = null)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Print the value in a register
		/// </summary>
		/// <param name="register">Register to print</param>
		/// <param name="comment"></param>
		public FluentWriter Printr(Register register, string comment = null)
		{
			_codeStream.Add(OpCode.Printr, comment, register);
			return this;
		}

		/// <summary>
		/// Jump the IP register the value indicated by the supplied register
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Jmp(Register offset, string comment = null)
		{
			_codeStream.Add(OpCode.Jmp, comment, offset);
			return this;
		}

		/// <summary>
		/// Jump the IP register the value indicated by the supplied register if the SF flag is set
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Jlt(Register offset, string comment = null)
		{
			_codeStream.Add(OpCode.Jlt, comment, offset);
			return this;
		}

		/// <summary>
		/// Increase the value in the supplied register by 1
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Incr(Register destination, string comment = null)
		{
			_codeStream.Add(OpCode.Incr, comment, destination);
			return this;
		}

		/// <summary>
		/// Compare a register and a constant value. Set ZF if values are equal, SF if register &lt; value
		/// </summary>
		/// <param name="register"></param>
		/// <param name="value"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public FluentWriter Cmpi(Register register, uint value, string comment = null)
		{
			_codeStream.Add(OpCode.Cmpi, comment, register, value);
			return this;
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