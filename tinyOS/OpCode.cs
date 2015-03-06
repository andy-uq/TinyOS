namespace Andy.TinyOS
{
	public enum OpCode : byte
	{
		Noop,
		Incr,
		Add,
		Push,
		Mov,
		Output,
		Jmp,
		Cmp,
		Jlt,
		Jgt,
		Je,
		Call,
		Ret,
		Alloc,
		Acquire,		
		Release,
		Sleep,
		SetP,
		Exit,
		Free,
		Map,
		Signal,
		Wait,
		Input,
		Clear,
		TermP,
		Pop,
		Jne,
		Mul,
		Div,
		Not,
		Neg,
		And,
		Or,
		Xor,
	}
}