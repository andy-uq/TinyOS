using System;
using System.Linq;
using System.Xml.Linq;

namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// A pretty printer designed for the structural C++ grammar
	/// </summary>
	public class CppStructuralOutputAsXml
		: AbstractSyntaxTreeTextPrinter
	{
		public XDocument AsXml()
		{
			var xml = ToString();
			return XDocument.Parse(xml);
		}

		private void PrintSimpleNode(ParseNode node)
		{
			string name = node.IsUnnamed() ? "" : " name='" + node.RuleName + "'";
			string value = string.Format("<{0}{1}>{2}</{0}>", node.RuleType, name, Escape(node));
			WriteLine(value);
		}

		private string Escape(ParseNode node)
		{
			var value = node.ToString();
			return value
				.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace("<", "&gt;");
		}

		protected override void Add(ParseNode node, int depth)
		{
			Indent = new String(' ', depth*2);
			if (node.IsUnnamed())
			{
				if (node.Count == 0)
					WriteLine(Escape(node));

				foreach (var child in node)
					Add(child, depth);

				return;
			}

			switch (node.RuleName)
			{
				case "pp_directive":
					PrintSimpleNode(node);
					return;

				case "comment_set":
					PrintSimpleNode(node);
					return;
			}

			string name = node.IsUnnamed() ? "" : string.Format(" name='{0}'", node.RuleName);
			string beginTag = string.Format("<{0}{1}>", node.RuleType, name);
			WriteLine(beginTag);

			if (node.Count == 0)
			{
				WriteLine(Escape(node));
			}
			else
			{
				foreach (ParseNode tmp in node)
					Add(tmp, depth + 1);
			}

			WriteLine(string.Format("</{0}>", node.RuleType));
		}

		public string TypeDeclToString(ParseNode node)
		{
			string r = "";

			Assert(node.RuleName == "type_decl");
			Assert(node.RuleType == RuleType.Sequence);
			Assert(node.Count == 2);
			Assert(node[1].RuleType == RuleType.Choice);
			Assert(node[1].Count == 1);

			switch (node[1][0].RuleName)
			{
				case "class_decl":
				case "struct_decl":
				case "union_decl":
				case "enum_decl":
					Assert(node[1][0].Count >= 3);
					Assert(node[1][0][1].RuleType == RuleType.Opt);

					r += node[1][0] + " ";
					ParseNode optIdent = node[1][0][1];
					if (optIdent.Count > 0)
					{
						Assert(optIdent[0].RuleName == "identifier");
						r += optIdent[0].ToString();
					}
					else
					{
						r += "_anon_";
					}
					break;

				default:
					throw new Exception("Unrecognized type declaration");
			}

			return r;
		}

		public void OutputDeclarationContent(ParseNode node)
		{
			Assert(node.RuleName == "declaration_content");
			string s = node.Aggregate("content: ", OutputChild);

			WriteLine(s);
		}

		private string OutputChild(string output, ParseNode child)
		{
			Assert(child.Count == 2);
			Assert(child[0].RuleName == "node");
			Assert(child[0].RuleType == RuleType.Choice);
			Assert(child[0].Count == 1);

			ParseNode tmp = child[0][0];
			switch (tmp.RuleName)
			{
				case "bracketed_group":
					output += "[...] ";
					break;
				case "paran_group":
					output += "(...) ";
					break;
				case "brace_group":
					output += "{...} ";
					break;
				case "type_decl":
					output += TypeDeclToString(tmp);
					break;
				case "typedef_decl":
					output += "typdef ";
					break;
				case "literal":
				case "symbol":
				case "label":
				case "identifier":
					output += tmp + " ";
					break;
			}

			return output;
		}

		public void OutputPrefixComment(ParseNode node, string indent)
		{
			Assert(node.RuleName == "comment_set");
			string r = "";

			foreach (ParseNode x in node.GetHierarchy())
				if (x.RuleName == "comment")
					r += x.ToString();

			if (r.Length > 0)
				WriteLine("prefix comment: " + r);
		}

		public void OutputSuffixComment(ParseNode node, string indent)
		{
		}

		public void OutputDeclaration(ParseNode node, string indent)
		{
			if (node.RuleName == "declaration")
			{
				Assert(node.Count == 1);
				node = node[0];
				Assert(node.Count >= 2);
				Assert(node[0].RuleName == "comment_set");
				OutputPrefixComment(node[0], indent);

				switch (node[1].RuleName)
				{
					case "pp_directive":
						WriteLine("pp_directive: " + node[1]);
						break;

					case "declaration_content":
						OutputSuffixComment(node[3], indent);
						OutputDeclarationContent(node[1]);
						break;

					case "semicolon":
						WriteLine("empty declaration");
						break;

					default:
						throw new Exception("Unrecognized kind of declaration");
				}
			}

			foreach (ParseNode child in node)
				OutputDeclaration(child, indent + "  ");
		}
	}
}