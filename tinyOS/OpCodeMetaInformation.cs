using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Andy.TinyOS
{
	namespace OpCodeMeta
	{
		public enum ParamType
		{
			None,
			Register,
			Constant,
			Source,
			Destination
		}

		public class ParameterInfo
		{
			public string Name { get; set; }
			public ParamType Type { get; set; }
			public string Comment { get; set; }
		}

		public class OpCodeMetaInformation
		{
			private static readonly Dictionary<OpCode, OpCodeMetaInformation> _opcodeMetaInformation = OpCodeMetaInformationBuilder.GetMetaInformation().ToDictionary(x => x.OpCode);

			public static IEnumerable<OpCodeMetaInformation> Opcodes
			{
				get { return _opcodeMetaInformation.Values; }
			}

			public static IDictionary<OpCode, OpCodeMetaInformation> Lookup
			{
				get { return _opcodeMetaInformation; }
			}

			public OpCode OpCode { get; set; }
			public string Comment { get; set; }
			public ParameterInfo[] Parameters { get; set; }

			public MethodInfo Method { get; set; }
		}

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