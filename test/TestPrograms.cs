namespace ClassLibrary1
{
	public static class TestPrograms
	{
		public const string ReallySimpleProgram = @"
			mov r1 $1 ; Put 1 into r1
			exit r1 ; Exit with r1
		";

		public const string P1 = @"
			mov r1 $1
			acquire r1
			wait r1
			release r1
			exit r1
		";

		public const string P2 = @"
			mov r1 $1
			signal r1
			acquire r1
			release r1
			exit r1
		";
	}
}