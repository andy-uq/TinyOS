using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Andy.TinyOS.Parser;
using tinyOS;

namespace Andy.TinyOS.Compiler
{
	public class Compiler
	{
		private readonly Dictionary<Rule, Func<ParseNode, IEnumerable<Instruction>>> _codeGenerator;
		private readonly ParseNode _root;
		private AndyStructuralGrammer _grammar;

		public Compiler(AndyStructuralGrammer grammer, ParserState parserState)
		{
			_grammar = grammer;
			_codeGenerator = grammer.GetRules<AndyStructuralGrammer>(throwOnMissing: false)
				.ToDictionary(k => k, GetCodeGenerator);

			_root = parserState.GetRoot();
		}

		public static Func<ParseNode, IEnumerable<Instruction>> GetCodeGenerator(Rule rule)
		{
			var name = rule.RuleName;
			var method = typeof(Compiler).GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.NonPublic);
			if (method == null)
				return null;

			var p = Expression.Parameter(typeof(ParseNode));
			var lambda = Expression.Lambda<Func<ParseNode, IEnumerable<Instruction>>>(
				Expression.Call(method, p),
				p
			);

			return lambda.Compile();
		}

		private IEnumerable<Instruction> Term(ParseNode node)
		{
			return new Instruction[]
			{
				new Instruction {OpCode = OpCode.Movi, Parameters = new uint[] {0, uint.Parse(node.ToString())}}
			};
		}

		private IEnumerable<Instruction> Factor(ParseNode node)
		{
			var factorOperator = node.SingleOrDefault(x => x.GetRule() == _grammar.multiply_operator);
			if ( factorOperator == null )
				return null;

			return new Instruction[]
			{
				new Instruction {OpCode = OpCode.Movi, Parameters = new uint[] {0, uint.Parse(node.ToString())}}
			};
		}

		public IEnumerable<Instruction> Compile()
		{
			return _root.SelectMany(Compile).Concat(new[] { 
				new Instruction
				{
					OpCode = OpCode.Exit,
					Parameters = new uint[] {0}
				}
			});
		}

		public IEnumerable<Instruction> Compile(ParseNode node)
		{
			var rule = node.GetRule();
			var codeGenerator = _codeGenerator[rule];
			if (codeGenerator != null)
				return codeGenerator(node);

			return node.SelectMany(Compile);
		}

		private IEnumerable<Instruction> Subtract(ParseNode node)
		{
			return new[]
			{
				new Instruction() {OpCode = OpCode.Movr, Parameters = new uint[] {1, 0}},
				new Instruction() {OpCode = OpCode.Addr, Parameters = new uint[] {0, 1}}
			};
		}

		private IEnumerable<Instruction> Add(ParseNode node)
		{
			return new[]
			{
				new Instruction() {OpCode = OpCode.Movr, Parameters = new uint[] {1, 0}},
				new Instruction() {OpCode = OpCode.Addr, Parameters = new uint[] {0, 1}}
			};
		}

		private IEnumerable<Instruction> Literal(ParseNode node)
		{
			var value = GetValue(node);
			return new[]
			{
				new Instruction
				{
					OpCode = OpCode.Movi,
					Parameters = new uint[] {0, value}
				},
			};
		}

		private uint GetValue(ParseNode node)
		{
			return uint.Parse(node.ToString());
		}
	}
}