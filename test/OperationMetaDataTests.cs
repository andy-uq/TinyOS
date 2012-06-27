using System;
using System.Linq;
using NUnit.Framework;
using tinyOS;
using tinyOS.OpCodeMeta;

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
				Assert.That(OpCodeMetaInformation.Lookup.Keys, Contains.Item(opcode));
			}
		}

	}
}