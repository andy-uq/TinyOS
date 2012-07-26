using System;
using System.Linq;
using Andy.TinyOS;
using Andy.TinyOS.OpCodeMeta;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class OperationMetaDataTests
	{
		[Test]
		public void HasMetaForEveryOpCode()
		{
			foreach (var opcode in Enum.GetValues(typeof(OpCode)).Cast<OpCode>())
			{
				if ( opcode.ToString().EndsWith("X") )
					continue;

				Assert.That(OpCodeMetaInformation.Lookup.Keys, Contains.Item(opcode));
			}
		}

		[Test]
		public void EveryOpCodeHasComment()
		{
			foreach (var opcode in Enum.GetValues(typeof(OpCode)).Cast<OpCode>())
			{
				if ( opcode.ToString().EndsWith("X") )
					continue;

				var meta = OpCodeMetaInformation.Lookup[opcode];
				Assert.That(meta.Comment, Is.Not.Null.Or.Empty);
			}
		}

	}
}