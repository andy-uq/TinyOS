using System.Collections.Generic;
using System.IO;

namespace tinyOS
{
	public class ProcessContextBlock
	{
		private Page _code;
		private Page _globalData;
		private Page _stack;

		public ProcessContextBlock()
		{
			Registers = new uint[10];
			PageTable = new PageTable();
			Locks = new HashSet<Lock>();
		}

		public uint Id { get; set; }

		public uint[] Registers { get; set; }
		public uint Ip { get; set; }
		public bool Sf { get; set; }
		public bool Zf { get; set; }

		public byte Priority { get; set; }

		public HashSet<Lock> Locks { get; set; }
		public PageTable PageTable { get; set; }
		public uint ExitCode { get; set; }
		public int Quanta { get; set; }

		public Page Stack
		{
			get { return _stack; }
			set
			{
				_stack = value;
				if (value != null)
				{
					PageTable.Allocate(value);
				}
			}
		}

		public Page Code
		{
			get { return _code; }
			set
			{
				_code = value;
				if ( value != null )
				{
					PageTable.Allocate(value);
				}
			}
		}

		public Page GlobalData
		{
			get { return _globalData; }
			set
			{
				_globalData = value;
				if ( value != null )
				{
					PageTable.Allocate(value);
				}
			}
		}
	}

	public static class ProcessControlBlockExtensions
	{
		public static void Compile(this ProcessContextBlock pcb, string source)
		{
			var writer = new CodeWriter(pcb.Code.Data);
			using (var reader = new StringReader(source))
			{
				var parser = new TextParser();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line))
						continue;

					var instruction = parser.Parse(line);
					writer.Write(instruction);
				}
			}
			writer.Close();
		}
	}
}