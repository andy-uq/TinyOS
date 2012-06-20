using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace tinyOS
{
	namespace OpCodeMeta
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

		public static class OpCodeMetaInformationBuilder
		{
			public static Action<Cpu, uint[]> BuildOperation(OpCodeMetaInformation metaInformation)
			{
				var expression = BuildExpression(metaInformation);
				return expression.Compile();
			}

			private static Expression<Action<Cpu, uint[]>> BuildExpression(OpCodeMetaInformation metaInformation)
			{
				var pCpu = Expression.Parameter(typeof (Cpu), "cpu");
				var pValues = Expression.Parameter(typeof (uint[]), "values");
				var actionParams = new[]
				{
					pCpu,
					pValues
				};

				var pParams = new List<Expression>() {pCpu};
				pParams.AddRange(metaInformation.Parameters.Select((x, i) => Expression.ArrayIndex(pValues, Expression.Constant(i))));

				Expression body = Expression.Call(metaInformation.Method, pParams);
				return Expression.Lambda<Action<Cpu, uint[]>>(body, actionParams);
			}

			public static IEnumerable<OpCodeMetaInformation> GetMetaInformation()
			{
				var instructionType = typeof (Instructions);
				return
					from method in instructionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
					let mParameters = method.GetParameters()
					let attributes = method.GetCustomAttributes(false).Cast<Attribute>()
					select new
					{
						method,
						descriptor = attributes.OfType<OpCodeAttribute>().SingleOrDefault(),
						parameters = attributes.OfType<ParameterAttribute>().OrderBy(x => ParameterOrder(method.Name, mParameters, x)),
					}
					into meta
					where meta.descriptor != null
					select new OpCodeMetaInformation
					{
						OpCode = meta.descriptor.OpCode,
						Comment = meta.descriptor.Comment,
						Method = meta.method,
						Parameters = meta.parameters
							.Select(p => new ParameterInfo {Comment = p.Comment, Name = p.Name, Type = p.Type})
							.ToArray()
					};
			}

			private static int ParameterOrder(string name, System.Reflection.ParameterInfo[] mParameters, ParameterAttribute parameter)
			{
				var methodParameter = mParameters.SingleOrDefault(x => x.Name == parameter.Name);
				if (methodParameter == null)
					throw new InvalidOperationException("Cannot find method parameter " + parameter.Name + " on " + name);

				return Array.IndexOf(mParameters, methodParameter);
			}
		}
	}
}