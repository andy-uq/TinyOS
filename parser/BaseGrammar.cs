﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// Provides a core set of parse rule operators. These are functions that construct
	/// parse rules from other parse rules. Usually a grammar derives from this class
	/// and exposes its rules as a set of public fields of type "Rule". A set of static
	/// functions (e.g. GetRuleFields(), GetRules()) are exposed in this class to be used 
	/// for iterating over the public Rule fields in a grammar, using reflection. 
	/// </summary>
	public class BaseGrammar
	{
		/// Rule operators are functions that take rules as arguments and produce new rules

		#region Rule operators

		/// <summary>
		/// Creates a new rule that attempts to match Rule x, but will always return true.
		/// It is equivalent to the expression "x | Nothing()" and is represented by the unary operator "?".
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Opt(Rule x)
		{
			return new OptRule(x);
		}

		/// <summary>
		/// Creates a rule that attempts to match Rule x as many times as possible
		/// and will always returns true. It is represented by the unary operator "*".
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Star(Rule x)
		{
			return new StarRule(x);
		}

		/// <summary>
		/// Like the Star operation, will attempt to match a Rule x as many times as possible
		/// except that it will return false if it does not match at least once.
		/// It is represented by the unary operator "+".
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Plus(Rule x)
		{
			return new PlusRule(x);
		}

		/// <summary>
		/// Creates a rule, that returns true if Rule x returns false, or returns false
		/// otherwise. It will never advance the parser index. It is represented by the 
		/// unsary operator "^".
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Not(Rule x)
		{
			return Skip(new NotRule(x));
		}

		/// <summary>
		/// Creates a rule that does not advanced the parser index, and will always
		/// return true. It is represented by the symbol "#".
		/// </summary>
		/// <returns></returns>
		protected static Rule Nothing()
		{
			return NothingRule.Instance;
		}

		/// <summary>
		/// Creates a rule that matches single characters, up to and including a 
		/// termination rule. 
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Until(Rule x)
		{
			return Star(AnythingBut(x));
		}

		/// <summary>
		/// Reads everything up to and including a specific rule.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule UntilPast(Rule x)
		{
			return Until(x) + x;
		}

		/// <summary>
		/// This creates a rule that evaluates a function at run-time, to come 
		/// up with the actual behavior. It is used to allow mutually recursive (i.e. circular) 
		/// rule references,otherwise the initialization of rules could enter into an infinite loop.
		/// </summary>
		/// <param name="f">A function that returns a rule, given no arguments</param>
		/// <returns></returns>
		protected static Rule Recursive(Func<Rule> f)
		{
			return new RecursiveRule(f);
		}

		/// <summary>
		/// Creates a rule that matches a particular sequence of characters.
		/// No node is created for this kind of rule.. 
		/// </summary>
		/// <param name="s">The characters that are matched by the rule.</param>
		/// <returns></returns>
		protected static Rule CharSeq(String s)
		{
			return new CharSeqRule(s);
		}

		/// <summary>
		/// Creates a rule that matches a single character, which belongs to 
		/// a set of characters.
		/// </summary>
		/// <param name="s">A string containing the characters to be contained in the string.</param>
		/// <returns></returns>
		protected static Rule CharSet(String s)
		{
			return new CharSetRule(s);
		}

		/// <summary>
		/// Creates a rule that matches a single character, which falls into a range of characters.
		/// </summary>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		protected static Rule CharRange(char begin, char end)
		{
			return new CharRangeRule(begin, end);
		}

		/// <summary>
		/// Creates a rule that matches any single character. Unlike Nothing() it always advances the 
		/// input. No node is created for this kind of rule.
		/// </summary>
		/// <returns></returns>
		protected static Rule Anything()
		{
			return AnythingRule.Instance;
		}

		/// <summary>
		/// Creates a rule that matches any single character, as long as the Rule x
		/// is not matched.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule AnythingBut(Rule x)
		{
			return Not(x) + Anything();
		}

		/// <summary>
		/// Creates a rule wrapper that will throw an exception if the underlying rule
		/// fails.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule NoFail(Rule x)
		{
			return new NoFailRule(x);
		}

		/// <summary>
		/// Creates a rule, that wraps all items in a sequence with a "NoFail" rule.
		/// This means that if any rule in the sequence fails, an exception will be thrown.
		/// Useful for identifying precisely where errors occur.
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		protected static Rule NoFailSeq(Rule rule)
		{
			if (!(rule is SeqRule))
				throw new Exception("A NoFailSeq operator can only be applied to a sequence");

			var seq = rule as SeqRule;

			// Normally this is done at the last minute, but we want all sub-rules 
			// to be wrapped in "NoFail" 
			seq.FlattenRules();

			IEnumerable<Rule> list = seq.GetChildren().Select(NoFail);
			var result = new SeqRule(list);
			return result;
		}

		/// <summary>
		/// Parses a rule, but does not create a parse node.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Skip(Rule x)
		{
			return new SkipRule(x);
		}

		/// <summary>
		/// Parses a rule and creates a parse node, but prevents any children 
		/// from being created.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Leaf(Rule x)
		{
			return new LeafRule(x);
		}

		/// <summary>
		/// Parses a rule 0 or more times but does not create a parse node.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		protected static Rule Eat(Rule x)
		{
			return Skip(Star(x));
		}

		/// <summary>
		/// Returns a rule that indicates we are the end of input.
		/// </summary>
		/// <returns></returns>
		protected static Rule EndOfInput()
		{
			return new EndOfInputRule();
		}

		#endregion

		/// Functions for operating on grammars

		#region grammar function

		/// <summary>
		/// Given a grammar object, it will set the name of all fields of type Rule 
		/// to have the name of the object.
		/// </summary>
		private void AssignRuleNames<T>(bool throwOnMissing = true)
		{
			foreach ( FieldInfo f in GetRuleFields<T>() )
			{
				Object v = f.GetValue(this);
				var r = v as Rule;
				if (r == null)
				{
					if (throwOnMissing)
					{
						throw new Exception("uninitialized rule " + f.Name);
					}
				}
				else
				{
					r.SetName(f.Name);
				}
			}
		}

		/// <summary>
		/// Returns all of the fields in a grammar that are of type Rule using reflection.
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public IEnumerable<FieldInfo> GetRuleFields<T>()
		{
			Type type = typeof (T);
			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			return type.GetFields(bindingFlags).Where(f => f.FieldType == typeof (Rule));
		}

		/// <summary>
		/// Returns all of the rules in a grammar, using reflection to iterate over the public fields
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Rule> GetRules<T>(bool throwOnMissing)
		{
			foreach (FieldInfo fi in GetRuleFields<T>())
			{
				var rule = fi.GetValue(this) as Rule;
				if (rule == null)
				{
					if ( throwOnMissing == false )
						continue;

					throw new Exception("Uninitialized rule: " + fi.Name);
				}

				yield return rule;
			}
		}

		/// <summary>
		/// Flattens all exposed rules in a grammar.
		/// </summary>
		/// <param name="g"></param>
		private void FlattenRules<T>(bool throwOnMissing)
		{
			foreach (Rule r in GetRules<T>(throwOnMissing))
				r.FlattenRules();
		}

		/// <summary>
		/// Assigns names to the rules, based on their member field names
		/// and then flattens all unnamed sequence and choice rules.
		/// </summary>
		/// <typeparam name="T">The grammar type being initialized</typeparam>
		public void InitializeRules<T>(bool throwOnMissing = true)
		{
			AssignRuleNames<T>(throwOnMissing);
			FlattenRules<T>(throwOnMissing);
		}

		#endregion
	}
}