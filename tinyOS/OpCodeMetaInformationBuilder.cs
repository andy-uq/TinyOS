using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using tinyOS;

namespace Andy.TinyOS.OpCodeMeta
{
	public static class OpCodeMetaInformationBuilder
	{
		public static Action<Cpu, byte, uint[]> BuildOperation(OpCodeMetaInformation metaInformation)
		{
			var expression = BuildExpression(metaInformation);
			return expression.Compile();
		}

		private static Expression<Action<Cpu, byte, uint[]>> BuildExpression(OpCodeMetaInformation metaInformation)
		{
			var pCpu = Expression.Parameter(typeof (Cpu), "cpu");
			var pControlFlag = Expression.Parameter(typeof (byte), "controlFlag");
			var pValues = Expression.Parameter(typeof (uint[]), "values");
			var actionParams = new[]
			{
				pCpu,
				pControlFlag,
				pValues
			};

			var pParams = new List<Expression> {pCpu, pControlFlag};
			pParams.AddRange(
				metaInformation
					.Parameters
					.Select((x, i) => Expression.ArrayIndex(pValues, Expression.Constant(i)))
				);

			Expression body = Expression.Call(metaInformation.Method, pParams);
			return Expression.Lambda<Action<Cpu, byte, uint[]>>(body, actionParams);
		}

		public static IEnumerable<OpCodeMetaInformation> GetMetaInformation()
		{
			var instructionType = typeof (InstructionsWithControlByte);
			return
				from method in instructionType.GetMethods(BindingFlags.Public | BindingFlags.Static)
				let mParameters = method.GetParameters()
				let attributes = method.GetCustomAttributes(false).Cast<Attribute>()
				let parameters = attributes.OfType<ParameterAttribute>()
				select new
				{
					method,
					descriptor = attributes.OfType<OpCodeAttribute>().SingleOrDefault(),
					parameters = parameters.Any() 
						? parameters.OrderBy(x => ParameterOrder(method.Name, mParameters, x))
						: mParameters.Select(AutoParameter).Where(x => x != null),
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

		private static ParameterAttribute AutoParameter(System.Reflection.ParameterInfo p)
		{
			if (p.Name == "source")
				return new ParameterAttribute("Source") { Comment = "Source value (Constant | Register | Memory Address)", Type = ParamType.Source };

			if (p.Name == "destination")
				return new ParameterAttribute("Destination") { Comment = "Destination value (Register | Memory Address)", Type = ParamType.Destination };

			return null;
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