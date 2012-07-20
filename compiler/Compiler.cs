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
		private readonly Dictionary<Rule, Func<CompilerContext, CompilerContext>> _codeGenerator;
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

		public Func<CompilerContext, CompilerContext> GetCodeGenerator(Rule rule)
		{
			var name = rule.RuleName.Replace("_", "");
			var method = typeof(Compiler).GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);
			if (method == null)
				return null;

			var p = System.Linq.Expressions.Expression.Parameter(typeof(CompilerContext));
			var instance = System.Linq.Expressions.Expression.Constant(this);
			var lambda = System.Linq.Expressions.Expression.Lambda<Func<CompilerContext, CompilerContext>>(
				System.Linq.Expressions.Expression.Call(instance, method, p),
				p
			);

			return lambda.Compile();
		}

		private static CompilerContext CompileExpression(CompilerContext context, params Tuple<char, Action<FluentWriter>>[] codeWriters)
		{
			context.Compile(context.Node[0]);
			var code = context.AsFluent();
			var expressions = codeWriters.ToDictionary(k => k.Item1, v => v.Item2);

			foreach (var right in context.Node[1])
			{
				code.Pushr(Register.A);

				var @operator = right[0];
				context.Compile(right[1]);

				code.Popr(Register.B);

				expressions[@operator.Value[0]](code);
			}

			return context;
		}

		private CompilerContext IntLiteral(CompilerContext context)
		{
			context.AsFluent()
				.Movi(Register.A, uint.Parse(context.Node.ToString()));

			return context;
		}

		private CompilerContext Term(CompilerContext context)
		{
			var rootNode = context.Node[0];
			if ( rootNode.Is(_grammar.unary_expression) )
			{
				context.Compile(rootNode[1]);
				context.Compile(rootNode[0]);
			}
			else
			{
				context.Compile(rootNode);
			}

			return context;
		}

		private CompilerContext UnaryOperator(CompilerContext context)
		{
			switch ( context.Node.Value )
			{
				case "!":
					context.AsFluent()
						.Not(Register.A);
					break;
				case "-":
					context.AsFluent()
						.Neg(Register.A);
					break;
				default:
					throw new InvalidOperationException("Cannot determine unary operator: " + context.Node.Value );
			}

			return context;
		}

		private CompilerContext AddExpression(CompilerContext context)
		{
			return CompileExpression(context, 
				new Tuple<char, Action<FluentWriter>>('+', code => code.Addr(Register.A, Register.B)),
				new Tuple<char, Action<FluentWriter>>('-', code => code.Neg(Register.A).Addr(Register.A, Register.B))
			);
		}

		private CompilerContext Factor(CompilerContext context)
		{
			return CompileExpression(context, 
				new Tuple<char, Action<FluentWriter>>('*', code => code.Mul(Register.A, Register.B)),
				new Tuple<char, Action<FluentWriter>>('/', code => code.Div(Register.B, Register.A))
			);
		}

		private CompilerContext Expression(CompilerContext context)
		{
			return CompileExpression(context,
				new Tuple<char, Action<FluentWriter>>('&', code => code.And(Register.A, Register.B)),
				new Tuple<char, Action<FluentWriter>>('|', code => code.Or(Register.A, Register.B)),
				new Tuple<char, Action<FluentWriter>>('^', code => code.Xor(Register.A, Register.B))
			);
		}
		
		public IEnumerable<Instruction> Compile()
		{
			var context = new CompilerContext
			{
				Code = new CodeStream(),
				Compiler = Compile,
				Node = _root,
				Parent = null,
			};

			foreach ( var node in context.Node )
				context.Compile(node);

			context.AsFluent()
				.Exit(Register.A);

			return context.Code;
		}

		private CodeStream Compile(CompilerContext context)
		{
			if ( context == null )
				throw new ArgumentNullException("context");
			
			var rule = context.Node.GetRule();
			if ( !rule.IsUnnamed() )
			{
				Func<CompilerContext, CompilerContext> codeGenerator;
				if (!_codeGenerator.TryGetValue(rule, out codeGenerator))
					throw new InvalidOperationException("Cannot find code generator: " + rule.RuleName );

				if (codeGenerator != null)
				{
					codeGenerator(context);
					return context.Code;
				}
			}

			foreach ( var node in context.Node )
				context.Compile(node);

			return context.Code;
		}
	}
}