using System;

namespace tinyOS
{
	public enum ParamType
	{
		None,
		Register,
		Constant
	}

	public class ParameterInfo
	{
		public string Name { get; set; }
		public ParamType Type { get; set; }
		public string Comment { get; set; }
	}
	
	public class OpCodeMetaInformation
	{
		public OpCode OpCode { get; set; }
		public string Comment { get; set; }
		public ParameterInfo[] Parameters { get; set; }
	}

	namespace OpCodeMeta
	{
		[AttributeUsage(AttributeTargets.Method)]
		public class OpCodeAttribute : Attribute
		{
			public OpCode OpCode { get; set; }
			public string Comment { get; set; }

			public OpCodeAttribute(OpCode opCode)
			{
				OpCode = opCode;
			}
		}

		[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
		public class ParameterAttribute : Attribute
		{
			public string Name { get; set; }
			public ParamType Type { get; set; }
			public string Comment { get; set; }

			public ParameterAttribute(string name)
			{
				Name = name;
			}
		}
	}
}