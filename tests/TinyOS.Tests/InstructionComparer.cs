using System.Collections.Generic;
using System.Linq;
using Andy.TinyOS;

namespace ClassLibrary1
{
	public class InstructionComparer : IEqualityComparer<Instruction>
	{
		public bool Equals(Instruction x, Instruction y)
		{
			if ( x.OpCode == y.OpCode 
				&& x.OpCodeMask == y.OpCodeMask 
				&& x.Parameters.Length == y.Parameters.Length )
			{
				return !x.Parameters.Where((t, i) => t != y.Parameters[i]).Any();
			}

			return false;
		}

		public int GetHashCode(Instruction obj)
		{
			throw new System.NotImplementedException();
		}
	}
}