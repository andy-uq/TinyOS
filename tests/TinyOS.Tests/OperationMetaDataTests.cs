using System;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.OpCodeMeta;
using Xunit;

namespace ClassLibrary1
{
	public class OperationMetaDataTests
	{
		[Fact]
		public void HasMetaForEveryOpCode()
		{
			foreach (var opcode in Enum.GetValues(typeof(OpCode)).Cast<OpCode>())
			{
				if ( opcode.ToString().EndsWith("X") )
					continue;

				Assert.That(OpCodeMetaInformation.Lookup.Keys, Contains.Item(opcode));
			}
		}

		[Fact]
		public void EveryOpCodeHasComment()
		{
			foreach (var opcode in Enum.GetValues(typeof(OpCode)).Cast<OpCode>())
			{
				if ( opcode.ToString().EndsWith("X") )
					continue;

				var meta = OpCodeMetaInformation.Lookup[opcode];
				Xunit.Assert.NotEmpty(meta.Comment);
			}
		}

	}
}