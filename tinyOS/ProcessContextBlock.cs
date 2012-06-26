using System.Collections.Generic;
using System.IO;

namespace tinyOS
{
	public class ProcessContextBlock
	{
		private PageInfo _code;
		private PageInfo _globalData;
		private PageInfo _stack;

		public ProcessContextBlock()
		{
			Registers = new uint[10];
			Locks = new HashSet<Lock>();

		    Stack = new PageInfo();
            Code = new PageInfo();
            GlobalData = new PageInfo();

		    PageTable = new PageTable {Code, GlobalData, Stack,};
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

	    public PageInfo Stack { get; set; }
	    public PageInfo Code { get; set; }
	    public PageInfo GlobalData { get; set; }
	}

	public static class ProcessControlBlockExtensions
	{
		public static Stream Compile(this ProcessContextBlock pcb, string source)
		{
            var stream = new MemoryStream();
			var writer = new CodeWriter(stream);
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
		    return stream;
		}
	}
}