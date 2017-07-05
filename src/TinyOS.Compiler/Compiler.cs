using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Andy.TinyOS.Parser;

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

		private static CompilerContext CompileExpression(CompilerContext context, params Tuple<string, Action<FluentWriter>>[] codeWriters)
		{
			return CompileExpression(context, codeWriters.Select(x => new Tuple<string, Action<FluentWriter>, OpCode>(x.Item1, x.Item2, OpCode.Noop)).ToArray());
		}

		private static CompilerContext CompileExpression(CompilerContext context, params Tuple<string, Action<FluentWriter>, OpCode>[] codeWriters)
		{
			context.Compile(context.Node[0]);
			var code = context.AsFluent();
			var expressions = codeWriters.ToDictionary(k => k.Item1, v => new { writer = v.Item2, jumpCode = v.Item3 });

			foreach (var right in context.Node[1])
			{
				code.Push.R(Register.A);

				var @operator = right[0];
				context.Compile(right[1]);

				code.Pop.R(Register.B);

				var expr = expressions[@operator.Value];
				expr.writer(code);

				context.JumpExpression = expr.jumpCode;
			}

			return context;
		}

		private CompilerContext IntLiteral(CompilerContext context)
		{
			context.AsFluent()
				.Mov.RU(Register.A, uint.Parse(context.Node.ToString()));

			return context;
		}

		private CompilerContext Identifier(CompilerContext context)
		{
			var name = context.Node.Value;
			var symbol = context.SymbolTable[name];

			context.AsFluent()
				.Mov.RU(Register.A, symbol.Address)
				.Add.RR(Register.A, Register.H)
				.Mov.RM(Register.A, MemoryAddress.A);

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
						.Not.RR(Register.A, Register.A);
					break;
				case "-":
					context.AsFluent()
						.Neg.RR(Register.A, Register.A);
					break;
				default:
					throw new InvalidOperationException("Cannot determine unary operator: " + context.Node.Value );
			}

			return context;
		}

		private CompilerContext AddExpression(CompilerContext context)
		{
			return CompileExpression(context,
				new Tuple<string, Action<FluentWriter>>("+", code => code.Add.RR(Register.A, Register.B)),
				new Tuple<string, Action<FluentWriter>>("-", code => code.Neg.RR(Register.A, Register.A).Add.RR(Register.A, Register.B))
			);
		}

		private CompilerContext Factor(CompilerContext context)
		{
			return CompileExpression(context,
				new Tuple<string, Action<FluentWriter>>("*", code => code.Mul.RR(Register.A, Register.B)),
				new Tuple<string, Action<FluentWriter>>("/", code => code.Div.RR(Register.B, Register.A))
			);
		}

		private CompilerContext Expression(CompilerContext context)
		{
			return CompileExpression(context,
				new Tuple<string, Action<FluentWriter>>("&", code => code.And.RR(Register.A, Register.B)),
				new Tuple<string, Action<FluentWriter>>("|", code => code.Or.RR(Register.A, Register.B)),
				new Tuple<string, Action<FluentWriter>>("^", code => code.Xor.RR(Register.A, Register.B))
			);
		}

		private CompilerContext AssignmentExpression(CompilerContext context)
		{
			var identifier = context.Node[0];
			foreach (var right in context.Node[1])
			{
				context.Compile(right[1]);
			}

			var name = identifier.Value;
			var symbol = context.SymbolTable[name];

			context.AsFluent()
				.Mov.RU(Register.B, symbol.Address, $"Load variable {name} into rB")
				.Add.RR(Register.B, Register.H)
				.Mov.MR(MemoryAddress.B, Register.A);

			return context;
		}

		private CompilerContext RelationalExpression(CompilerContext context)
		{
			return CompileExpression(context,
				new Tuple<string, Action<FluentWriter>, OpCode>("<", code => code.Cmpi.RR(Register.A, Register.B), OpCode.Jlt),
				new Tuple<string, Action<FluentWriter>, OpCode>("<=", code => code.Cmpi.RR(Register.A, Register.B), OpCode.Jlt),
				new Tuple<string, Action<FluentWriter>, OpCode>(">", code => code.Cmpi.RR(Register.B, Register.A), OpCode.Jlt),
				new Tuple<string, Action<FluentWriter>, OpCode>(">=", code => code.Cmpi.RR(Register.B, Register.A), OpCode.Jlt),
				new Tuple<string, Action<FluentWriter>, OpCode>("==", code => code.Cmpi.RR(Register.A, Register.B), OpCode.Jne)
			);
		}

		private CompilerContext IfStatement(CompilerContext context)
		{
			var ifFalse = Enumerable.Empty<Instruction>();

			var expression = context.Node.GetNamedChild("if_condition").GetNamedChild("relational_expression");
			context.Compile(expression);
			var jumpCode = context.JumpExpression;

			var block = context.Node.GetNamedChild("block");
			var child = context.Push(block);
			CodeStream ifTrue = child.Compiler(child);

			var elseBlock = context.Node.GetNamedChild("else_block");
			if (elseBlock != null)
			{
				block = elseBlock.GetNamedChild("block");
				child = context.Push(block);
				ifFalse = child.Compiler(child);
				ifTrue.AsFluent()
					.Jmp.I(ifFalse.Count() + 1);
			}

			context.Code.AsFluent()
				.UnaryOp(jumpCode).I(ifTrue.Count() + 1);

			context.Code += ifTrue;
			context.Code += ifFalse;
			
			return context;
		}
		
		private CompilerContext Statement(CompilerContext context)
		{
			context.Code.Add(new Instruction(OpCode.Noop, context.Node.ToString()));
			foreach ( var expression in context.Node )
			{
				var child = context.Push(expression);
				context.Code += child.Compiler(child);
			}

			return context;
		}

		private CompilerContext WhileStatement(CompilerContext context)
		{
			var expression = context.Node.GetNamedChild("while_condition").GetNamedChild("relational_expression");

			var conditionPtr = context.Code.Position;
			context.Compile(expression);
			
			var block = context.Node.GetNamedChild("block");
			var child = context.Push(block);
			
			var blockCode = child.Compiler(child);
			context.Code.AsFluent()
				.Je.I(blockCode.Count() + 2);

			context.Code += blockCode;
			context.Code.AsFluent()
				.Jmp.I(conditionPtr - context.Code.Position);
			
			return context;
		}
		
		public IEnumerable<Instruction> Compile()
		{
			var context = new CompilerContext
			{
				Code = new CodeStream(),
				Compiler = Compile,
				Node = _root,
				Parent = null,
				SymbolTable = new SymbolTable(),
			};

			foreach ( var node in context.Node )
				context.Compile(node);

			context.AsFluent()
				.Exit.R(Register.A);

			return context.Code;
		}

		private CodeStream Compile(CompilerContext context)
		{
			if ( context == null )
				throw new ArgumentNullException(nameof(context));
			
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