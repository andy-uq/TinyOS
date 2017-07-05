using System;
using System.Linq;
using Andy.TinyOS;
using FluentAssertions;
using Xunit;

namespace ClassLibrary1
{
	public class CompilerTests
	{
		const string _reallySimpleProgram = @"
			movi r1 $1
			exit r1
		";
 
		[Fact]
		public void Compile()
		{
			var cpu = new Cpu(2048, 256);
			cpu.IdleProcess.Code.Append(cpu.Ram.Allocate(cpu.IdleProcess));

			var ms = cpu.GetMemoryStream(cpu.IdleProcess.Code);
			var writer = new CodeWriter(ms);
			Array.ForEach(IdleProcess.TerminatingIdle, writer.Write);
			writer.Close();

			var codeReader = new CodeReader(cpu.GetMemoryStream(cpu.IdleProcess.Code));
			
			var actual = codeReader.GetEnumerator();
			var expected = IdleProcess.TerminatingIdle.Cast<Instruction>().GetEnumerator();

			while ( actual.MoveNext() )
			{
				if ( expected.MoveNext() )
				{
					actual.Current.OpCode.Should().Be(expected.Current.OpCode);
					actual.Current.Parameters.Should().ContainInOrder(expected.Current.Parameters);
					actual.Current.Comment.Should().Be(expected.Current.Comment);
					continue;
				}

				actual.Current.OpCode.Should().Be(OpCode.Noop);
			}

		}
	}
}