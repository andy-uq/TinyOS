using System.Collections.Generic;

namespace Andy.TinyOS.Compiler
{
	public class SymbolTable
	{
		private uint _offset;
		private readonly Dictionary<string, Symbol> _symbols;

		public SymbolTable()
		{
			_symbols = new Dictionary<string, Symbol>();
		}

		public Symbol this[string name]
		{
			get
			{
				Symbol symbol;
				if (_symbols.TryGetValue(name, out symbol))
					return symbol;

				symbol = new Symbol {Name = name, Address = (_offset++) * 4 };
				_symbols.Add(name, symbol);

				return symbol;
			}
		}
	}
}