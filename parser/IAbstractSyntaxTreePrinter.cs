namespace Andy.TinyOS.Parser
{
	/// <summary>
	/// A parser will print to any class that implements IAstPrinter.
	/// This is effectively a pretty printer.
	/// </summary>
	public interface IAbstractSyntaxTreePrinter
	{
		void Print(ParseNode node);
	}
}