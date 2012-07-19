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
		private readonly AndyStructuralGrammar _grammar;

		public Compiler(AndyStructuralGrammar grammar, ParserState parserState)
		{
			_grammar = grammar;

			var rules = grammar.GetRules<AndyStructuralGrammar>(throwOnMissing: false);
			rules = rules.Concat(grammar.GetRules<AndyBaseGrammar>(throwOnMissing: false));

			_codeGenerator = rules
				.Distinct()
				.ToDictionary(k => k, GetCodeGenerator);

			_root = parserState.GetRoot();
		}

		public Func<ParseNode, IEnumerable<Instruction>> GetCodeGenerator(Rule rule)
		{
			var name = rule.RuleName.Replace("_", "");
			var method = typeof(Compiler).GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);
			if (method == null)
				return null;

			var p = System.Linq.Expressions.Expression.Parameter(typeof(ParseNode));
			var instance = System.Linq.Expressions.Expression.Constant(this);
			var lambda = System.Linq.Expressions.Expression.Lambda<Func<ParseNode, IEnumerable<Instruction>>>(
				System.Linq.Expressions.Expression.Call(instance, method, p),
				p
			);

			return lambda.Compile();
		}

		private IEnumerable<Instruction> IntLiteral(ParseNode node)
		{
			return new[]
			{
				new Instruction {OpCode = OpCode.Movi, Parameters = new uint[] {0, uint.Parse(node.ToString())}}
			};
		}

		private IEnumerable<Instruction> Term(ParseNode node)
		{
			if ( node[0].Is(_grammar.unary_expression ))
			{
				node = node[0];
				return Compile(node[1]).Concat(Compile(node[0]));
			}

			return Compile(node[0]);
		}

		private IEnumerable<Instruction> UnaryOperator(ParseNode node)
		{
			Instruction unaryCode;

			switch (node.Value)
			{
				case "!":
					unaryCode = new Instruction {OpCode = OpCode.Not, Parameters = new uint[] {0}};
					break;
				case "-":
					unaryCode = new Instruction {OpCode = OpCode.Neg, Parameters = new uint[] {0}};
					break;
				default:
					throw new InvalidOperationException("Cannot determine unary operator: " + node.Value);
			}

			return new[] {unaryCode};
		}

		private IEnumerable<Instruction> Expression(ParseNode node)
		{
			var code = Compile(node[0]);

			foreach ( var right in node[1] )
			{
				code = code.Concat(new[] {new Instruction {OpCode = OpCode.Pushr, Parameters = new uint[] { 0 }},});
				var @operator = right[0];
				code = code.Concat(Compile(right[1]));

				Instruction logicalCode;
				switch (@operator.Value)
				{
					case "&":
						logicalCode = new Instruction {OpCode = OpCode.And, Parameters = new uint[] {0, 1}};
						break;
					case "|":
						logicalCode = new Instruction {OpCode = OpCode.Or, Parameters = new uint[] {0, 1}};
						break;
					case "^":
						logicalCode = new Instruction {OpCode = OpCode.Xor, Parameters = new uint[] {0, 1}};
						break;
					default:
						throw new InvalidOperationException("Cannot determine logical operator: " + @operator.Value);
				}

				code = code.Concat(new[]
				                   	{
										new Instruction {OpCode = OpCode.Popr, Parameters = new uint[] { 1 }},
				                   		logicalCode
				                   	});
			}

			return code;
		}

		private IEnumerable<Instruction> AddExpression(ParseNode node)
		{
			var code = Compile(node[0]);

			foreach ( var right in node[1] )
			{
				code = code.Concat(new[] {new Instruction {OpCode = OpCode.Pushr, Parameters = new uint[] {0}},});
				var @operator = right[0];
				code = code.Concat(Compile(right[1]));

				Instruction[] logicalCode;
				switch (@operator.Value)
				{
					case "+":
						logicalCode = new[] {new Instruction {OpCode = OpCode.Addr, Parameters = new uint[] {0, 1}}};
						break;
					case "-":
						logicalCode = new[]
						              	{
						              		new Instruction {OpCode = OpCode.Neg, Parameters = new uint[] {0}},
						              		new Instruction {OpCode = OpCode.Addr, Parameters = new uint[] {0, 1}}
						              	};
						break;
					default:
						throw new InvalidOperationException("Cannot determine addition operator: " + @operator.Value);
				}

				code = code.Concat(new[]
				                   	{
				                   		new Instruction {OpCode = OpCode.Popr, Parameters = new uint[] {1}},
				                   	}).Concat(logicalCode);
			}

			return code;
		}

		private IEnumerable<Instruction> Factor(ParseNode node)
		{
			var code = Compile(node[0]);

			foreach ( var right in node[1] )
			{
				code = code.Concat(new[] {new Instruction {OpCode = OpCode.Pushr, Parameters = new uint[] {0}},});
				var @operator = right[0];
				code = code.Concat(Compile(right[1]));

				Instruction[] logicalCode;
				switch (@operator.Value)
				{
					case "*":
						logicalCode = new[] {new Instruction {OpCode = OpCode.Mul, Parameters = new uint[] {0, 1}}};
						break;
					case "/":
						logicalCode = new[]
						              	{
						              		new Instruction {OpCode = OpCode.Div, Parameters = new uint[] {0,1}},
						              	};
						break;
					default:
						throw new InvalidOperationException("Cannot determine addition operator: " + @operator.Value);
				}

				code = code.Concat(new[]
				                   	{
				                   		new Instruction {OpCode = OpCode.Popr, Parameters = new uint[] {1}},
				                   	}).Concat(logicalCode);
			}

			return code;
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

		private IEnumerable<Instruction> Compile(IEnumerable<ParseNode> nodeSet)
		{
			return nodeSet.SelectMany(Compile);
		}

		private IEnumerable<Instruction> Compile(ParseNode node)
		{
			if ( node == null )
				throw new ArgumentNullException("node");

			var rule = node.GetRule();
			if ( !rule.IsUnnamed() )
			{
				Func<ParseNode, IEnumerable<Instruction>> codeGenerator;
				if (!_codeGenerator.TryGetValue(rule, out codeGenerator))
					throw new InvalidOperationException("Cannot find code generator: " + rule.RuleName );

				if (codeGenerator != null)
				{
					var code = codeGenerator(node);
					if ( code != null )
						return code;
				}
			}

			return node.SelectMany(Compile);
		}
	}
}