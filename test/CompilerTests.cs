using System;
using System.Linq;
using NUnit.Framework;
using tinyOS;

namespace ClassLibrary1
{
	[TestFixture]
	public class CompilerTests
	{
		const string _reallySimpleProgram = @"
			movi r1 $1
			exit r1
		";
 
		[Test]
		public void Compile()
		{
			var cpu = new Cpu(2048, 256);
			cpu.IdleProcess.Code.Append(cpu.Ram.Allocate(cpu.IdleProcess));

			var ms = cpu.GetMemoryStream(cpu.IdleProcess.Code);
			var writer = new CodeWriter(ms);
			Array.ForEach(IdleProcess.TerminatingIdle, writer.Write);
			writer.Close();

			var codeReader = new CodeReader(cpu.GetMemoryStream(cpu.IdleProcess.Code));
			var instructions = codeReader.Instructions;

			var actual = instructions.GetEnumerator();
			var expected = IdleProcess.TerminatingIdle.Cast<Instruction>().GetEnumerator();

			while ( actual.MoveNext() )
			{
				if ( expected.MoveNext() )
				{
					Assert.That(actual.Current.OpCode, Is.EqualTo(expected.Current.OpCode));
					Assert.That(actual.Current.Parameters, Is.EqualTo(expected.Current.Parameters));
					Assert.That(actual.Current.Comment, Is.EqualTo(expected.Current.Comment));
					continue;
				}

				Assert.That(actual.Current.OpCode, Is.EqualTo(OpCode.Noop));
			}

		}
	}
}