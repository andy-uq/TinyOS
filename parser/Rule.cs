using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// Describes a parsing rule, which maps roughly to a PEG grammar rule. 
	/// Unlike typical CFG forms (like BNF, or extended BNF) a PEG grammar
	/// rule is prioritized, unambiguous, and may have zero length 
	/// The function which a user calls is "Match" which performs the 
	/// actual matching and advancing of the parser index if successful.
	/// The different kinds of rules, provide overrides of the abstract 
	/// function InternalMatch to customize their own behavior.
	/// </summary>
	public abstract class Rule : IEquatable<Rule>
	{
		#region private fields

		/// <summary>
		/// A Rule can be named or unnamed. An unnamed Rule (IsUnnamed() == true)
		/// mean that the name field is null. The name is set by a call to static function
		/// </summary>
		private string Name { get; set; }

		#endregion

		#region protected fields

		protected Rule()
		{
			Rules = new List<Rule>();
		}

		/// <summary>
		/// Used to manage child rules. In many cases this is a list of one-element (e.g. StarRule, PlusRule).
		/// In some cases it has no-elements (e.g. NothingRule, AnythingRule). And in the cases of SeqRule, or ChoiceRule
		/// it will contain two or more rules.
		/// </summary>
		protected List<Rule> Rules { get; set; }

		#endregion

		#region public methods

		/// <summary>
		/// Used to combine unnamed sequence rules and unamed choice rules, into one single list.
		/// This is because long expressions like "a + b + c" would create a right-heavy binary tree 
		/// (e.g. (a + b) + c) where we really want a single list. 
		/// </summary>
		public virtual void FlattenRules()
		{
			foreach (Rule r in Rules)
				r.FlattenRules();
		}

		/// <summary>
		/// Used to add new sub-rules to SeqRule or ChildRule rules. 
		/// </summary>
		/// <param name="r"></param>
		protected void AddRule(Rule r)
		{
			// If this assertion fails, there is a good chance that it is because
			// you are referring to a rule that hasn't been initialized yet. 
			if (r == null)
				throw new ArgumentNullException("r");

			Rules.Add(r);
		}

		/// <summary>
		/// Returns a collection of all child rules
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Rule> GetRules()
		{
			return Rules;
		}

		#endregion

		#region overrides

		/// <summary>
		/// Returns a string representation of a parsing rule.
		/// myrule ::== (some_rule | some_other_rule)* + terminator;
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return IsUnnamed() ? RuleDefinition : Name;
		}

		#endregion

		#region properties

		/// <summary>
		/// Returns the name of the rule, or _unnamed_ if it is an unnamed rule.
		/// </summary>
		public string RuleName
		{
			get { return IsUnnamed() ? "_unnamed_" : Name; }
		}

		/// <summary>
		/// Outputs the rule in the form "MyRuleName ::== SomeRule + (OneRule | AnotherRule)"
		/// </summary>
		public string RuleNameAndDefinition
		{
			get { return RuleName + " ::== " + RuleDefinition; }
		}


		/// <summary>
		/// Outputs the rule name, or the definition if unnamed.
		/// </summary>
		public string RuleNameOrDefinition
		{
			get
			{
				if (IsUnnamed())
					return RuleDefinition;
				return
					RuleName;
			}
		}

		/// <summary>
		/// Returns a string that represents the defintion of parsing rule
		/// (some_rule | some_other_rule)* + terminator
		/// </summary>
		public abstract string RuleDefinition { get; }

		/// <summary>
		/// Returns a string representing the kind of rule (Star, Choice, etc.)
		/// </summary>
		public abstract RuleType RuleType { get; }

		#endregion

		#region operator overloads

		/// <summary>
		/// Represent the choice operator. 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rule operator |(Rule a, Rule b)
		{
			return new ChoiceRule(a, b);
		}

		/// <summary>
		/// Represents the sequence operator.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Rule operator +(Rule a, Rule b)
		{
			return new SeqRule(a, b);
		}

		// Note: string a + string b is already to return a concatenated string, 
		// so it is not included here.

		#endregion

		/// <summary>
		/// This is the work-horse function. It attempts to match a rule
		/// by calling the rule's specialized "InternalMatch()" function. 
		/// If the match is successful, a node is created and added to the 
		/// the parser tree managed by ParserState. 
		/// If the match fails, then no node is created (technically 
		/// the node was already created, and just gets deleted) and the 
		/// ParserState index is returned to its original location.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public virtual bool Match(ParserState p)
		{
			int oldIndex = p.Index;

			if (p.CreateNodes && !(this is SkipRule))
			{
				var node = new ParseNode(this, p.Peek(), p.Text, oldIndex);
				p.Push(node);
				if (InternalMatch(p))
				{
					node.Complete(p.Index);
					p.Pop();
					return true;
				}

				p.Index = oldIndex;
				p.Pop();
				return false;
			}

			if (InternalMatch(p))
			{
				return true;
			}

			p.Index = oldIndex;
			return false;
		}

		/// <summary>
		/// Each specialized rule type, provides its own definition of InternalMatch 
		/// to do what is expected. An override of this function should not take 
		/// care of creating parse tree nodes, or restoring the text pointer, because
		/// that is done automatically by the Match function.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		protected abstract bool InternalMatch(ParserState p);

		/// <summary>
		/// Used to assign names. Usually just called by AssignRuleNames,
		/// which reflects over an object's field names to set the rule names.        
		/// </summary>
		/// <param name="s"></param>
		public void SetName(string s)
		{
			Name = s;
		}

		/// <summary>
		/// Returns true if the name field is null.
		/// </summary>
		/// <returns></returns>
		public bool IsUnnamed()
		{
			return Name == null;
		}

		/// <summary>
		/// Returns all child rules.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Rule> GetChildren()
		{
			return Rules;
		}

		public bool Equals(Rule other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.RuleNameOrDefinition, RuleNameOrDefinition);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Rule)) return false;
			return Equals((Rule) obj);
		}

		public override int GetHashCode()
		{
			return RuleNameOrDefinition.GetHashCode();
		}

		public static bool operator ==(Rule left, Rule right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Rule left, Rule right)
		{
			return !Equals(left, right);
		}
	}

	/// <summary>
	/// Matches two or more sub-rules, returning true if any are successful.    
	/// </summary>
	public class ChoiceRule : Rule
	{
		public ChoiceRule(Rule a, Rule b)
		{
			AddRule(a);
			AddRule(b);
		}

		public override string RuleDefinition
		{
			get
			{
				string r = "(";
				int i = 0;
				foreach (Rule rule in GetRules())
				{
					if (i++ > 0)
						r += " | ";
					r += rule.ToString();
				}
				return r + ")";
			}
		}

		public override RuleType RuleType
		{
			get { return RuleType.Choice; }
		}

		public override void FlattenRules()
		{
			base.FlattenRules();

			while (true)
			{
				var tmp = new List<Rule>();
				foreach (Rule r in GetRules())
				{
					if (r is ChoiceRule && r.IsUnnamed())
					{
						tmp.AddRange(r.GetRules());
					}
					else
					{
						tmp.Add(r);
					}
				}

				if (Rules.Count == tmp.Count)
				{
					return;
				}

				Rules = tmp;
			}
		}

		protected override bool InternalMatch(ParserState p)
		{
			return Rules.Any(r => r.Match(p));
		}
	}

	/// <summary>
	/// Matches a sequence of rules, one by one.
	/// </summary>
	public class SeqRule : Rule
	{
		public SeqRule(Rule a, Rule b)
		{
			AddRule(a);
			AddRule(b);
		}

		public SeqRule(IEnumerable<Rule> xs)
		{
			Rules.AddRange(xs);

			if (Rules.Count < 2)
				throw new InvalidOperationException("Expecting two or more rules in a sequence");
		}

		public override RuleType RuleType
		{
			get { return RuleType.Sequence; }
		}

		public override string RuleDefinition
		{
			get
			{
				string r = "(";
				for (int i = 0; i < Rules.Count; ++i)
				{
					if (i > 0)
						r += " + ";
					r += Rules[i].ToString();
				}
				return r + ")";
			}
		}

		public override void FlattenRules()
		{
			base.FlattenRules();

			while (true)
			{
				var tmp = new List<Rule>();
				foreach (Rule r in Rules)
				{
					if (r is SeqRule && r.IsUnnamed())
					{
						tmp.AddRange(r.GetRules());
					}
					else
					{
						tmp.Add(r);
					}
				}

				if (Rules.Count == tmp.Count)
					return;

				Rules = tmp;
			}
		}

		protected override bool InternalMatch(ParserState p)
		{
			return Rules.All(r => r.Match(p));
		}
	}

	/// <summary>
	/// Matches a single character and advances the index by one
	/// </summary>
	public class AnythingRule : Rule
	{
		public static readonly AnythingRule Instance = new AnythingRule();

		private AnythingRule()
		{
		}

		public override RuleType RuleType
		{
			get { return RuleType.Anything; }
		}

		public override string RuleDefinition
		{
			get { return "."; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if (p.AtEndOfInput())
				return false;

			p.Index++;
			return true;
		}
	}

	/// <summary>
	/// Always returns true, but unlike AnythingRule does not advance 
	/// the index by one
	/// </summary>
	public class NothingRule : Rule
	{
		public static readonly NothingRule Instance = new NothingRule();

		public override RuleType RuleType
		{
			get { return RuleType.Nothing; }
		}

		public override string RuleDefinition
		{
			get { return "#"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			return true;
		}
	}

	/// <summary>
	/// Matches a sub-rule 0 or more times.
	/// </summary>
	public class StarRule : Rule
	{
		public StarRule(Rule x)
		{
			AddRule(x);
		}

		public override RuleType RuleType
		{
			get { return RuleType.Star; }
		}

		public override string RuleDefinition
		{
			get { return Rules[0] + "*"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if (Rules.Count != 1)
				throw new InvalidOperationException(string.Format("Invalid number of child rules (Expected 1, Actual: {0})", Rules.Count));

			Rule r = Rules[0];
			while (true)
			{
				int old = p.Index;
				if (!r.Match(p))
					return true;

				// Avoid infinite loops.
				if (p.Index <= old)
					throw new Exception("Failed to advance parser input pointer, while parsing rule '" +
					                    r.RuleNameOrDefinition + "'. This means the grammar is invalid, maybe because of nested star rules.");
			}
		}
	}

	/// <summary>
	/// Matches a sub-rule 1 or more times.
	/// </summary>
	public class PlusRule : Rule
	{
		public PlusRule(Rule x)
		{
			AddRule(x);
		}

		public override RuleType RuleType
		{
			get { return RuleType.Plus; }
		}

		public override string RuleDefinition
		{
			get { return Rules[0] + "+"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if ( Rules.Count != 1 )
				throw new InvalidOperationException(string.Format("Invalid number of child rules (Expected 1, Actual: {0})", Rules.Count));

			Rule r = Rules[0];
			if (!r.Match(p))
				return false;
			while (true)
			{
				int old = p.Index;
				if (!r.Match(p))
					return true;

				// Avoid infinite loops.
				if (p.Index <= old)
					throw new Exception("Failed to advance parser input pointer, while parsing rule '" +
					                    r.RuleNameOrDefinition + "'. This means the grammar is invalid, maybe because of nested star rules.");
			}
		}
	}

	/// <summary>
	/// Returns true if the sub-rule is not successful, or false otherwise.
	/// </summary>
	public class NotRule : Rule
	{
		public NotRule(Rule x)
		{
			AddRule(x);
		}

		public override RuleType RuleType
		{
			get { return RuleType.Not; }
		}

		public override string RuleDefinition
		{
			get { return Rules[0] + "^"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if ( Rules.Count != 1 )
				throw new InvalidOperationException(string.Format("Invalid number of child rules (Expected 1, Actual: {0})", Rules.Count));

			Rule r = Rules[0];
			if (r.Match(p))
				return false;
			return true;
		}
	}

	/// <summary>
	/// Returns true, whether or not the sub-rule is not successful.
	/// </summary>
	public class OptRule : Rule
	{
		public OptRule(Rule x)
		{
			AddRule(x);
		}

		public override RuleType RuleType
		{
			get { return RuleType.Opt; }
		}

		public override string RuleDefinition
		{
			get { return Rules[0] + "?"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			Rule r = Rules[0];
			r.Match(p);
			return true;
		}
	}

	/// <summary>
	/// Evaluates a function at run-time, to perform the matching for it.
	/// Used to allow circular references in the grammar.
	/// </summary>
	public class RecursiveRule : Rule
	{
		private readonly Func<Rule> _func;

		public RecursiveRule(Func<Rule> f)
		{
			_func = f;
		}

		public override RuleType RuleType
		{
			get { return RuleType.Recursive; }
		}

		public override string RuleDefinition
		{
			get { return "recursive"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			Rule r = _func();
			return r.Match(p);
		}
	}

	/// <summary>
	/// Matches a sequence of characters in the input text
	/// </summary>
	public class CharSeqRule : Rule
	{
		private readonly string _value;

		public CharSeqRule(string s)
		{
			if (string.IsNullOrEmpty(s))
				throw new ArgumentException("Parameter must not be empty or null", "s");

			_value = s;
		}

		public override RuleType RuleType
		{
			get { return RuleType.CharSequence; }
		}

		public override string RuleDefinition
		{
			get { return "[" + _value + "]"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			int n = _value.Length;
			if (p.Index + n > p.Text.Length)
				return false;
			for (int i = 0; i < n; ++i)
			{
				if (p.Text[p.Index++] != _value[i])
					return false;
			}
			return true;
		}
	}

	/// <summary>
	/// Matches a single character against a set of characters in the input text.
	/// </summary>
	public class CharSetRule : Rule
	{
		private readonly string _value;

		public CharSetRule(string s)
		{
			_value = s;
		}

		public override RuleType RuleType
		{
			get { return RuleType.CharSet; }
		}

		public override string RuleDefinition
		{
			get { return "[" + _value + "]"; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if (p.AtEndOfInput())
				return false;

			char c = p.Text[p.Index++];
			return _value.Contains(c);
		}
	}

	/// <summary>
	/// Matches a single character in the input text against a range of valid characters
	/// </summary>
	public class CharRangeRule : Rule
	{
		private readonly char _first;
		private readonly char _last;

		public CharRangeRule(char from, char to)
		{
			_first = from;
			_last = to;
		}

		public override RuleType RuleType
		{
			get { return RuleType.CharRange; }
		}

		public override string RuleDefinition
		{
			get { return string.Format("[{0}..{1}]", _first, _last); }
		}

		protected override bool InternalMatch(ParserState p)
		{
			if (p.AtEndOfInput())
				return false;

			char c = p.Text[p.Index++];
			return (c >= _first && c <= _last);
		}
	}

	/// <summary>
	/// Throws an exception if parsing fails, instead of returning false
	/// </summary>
	public class NoFailRule : Rule
	{
		public NoFailRule(Rule x)
		{
			AddRule(x);
		}

		public override RuleType RuleType
		{
			get { return RuleType.NoFail; }
		}

		public override string RuleDefinition
		{
			get { return Rules[0].RuleDefinition; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			Rule rule = Rules[0];
			if (rule.Match(p))
			{
				return true;
			}

			throw new ParsingException(p, rule, p.Peek());
		}
	}

	/// <summary>
	/// Used to prevent creation of parse node in the AST tree. For 
	/// example whitespace. 
	/// </summary>
	public class SkipRule : Rule
	{
		public SkipRule(Rule x)
		{
			AddRule(x);
		}

		public override string RuleDefinition
		{
			get { return "skip(" + Rules[0].RuleDefinition + ")"; }
		}

		public override RuleType RuleType
		{
			get { return RuleType.Skip; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			// Tell the parser state to stop creating nodes. 
			bool store = p.CreateNodes;
			p.CreateNodes = false;

			bool result;
			try
			{
				result = Rules[0].Match(p);
			}
			finally
			{
				p.CreateNodes = store;
			}
			return result;
		}
	}

	/// <summary>
	/// Used to prevent creation of child parse nodes in the AST tree. For 
	/// example identifiers. Similar to SkipRule.
	/// </summary>
	public class LeafRule : Rule
	{
		public LeafRule(Rule x)
		{
			AddRule(x);
		}

		public override string RuleDefinition
		{
			get { return Rules[0].RuleDefinition; }
		}

		public override RuleType RuleType
		{
			get { return RuleType.Leaf; }
		}

		protected override bool InternalMatch(ParserState p)
		{
			// Tell the parser state to stop creating nodes. 
			bool store = p.CreateNodes;
			p.CreateNodes = false;

			bool result;
			try
			{
				result = Rules[0].Match(p);
			}
			finally
			{
				p.CreateNodes = store;
			}
			return result;
		}
	}

	/// <summary>
	/// A rule that returns true if at the end of input.
	/// </summary>
	public class EndOfInputRule : Rule
	{
		public override string RuleDefinition
		{
			get { return "_EOF_"; }
		}

		public override RuleType RuleType
		{
			get { return RuleType.Eof; }
		}

		public override bool Match(ParserState p)
		{
			return p.AtEndOfInput();
		}

		protected override bool InternalMatch(ParserState p)
		{
			throw new Exception("Error: EndOfInput.InternalMatch() should never be called");
		}
	}
}