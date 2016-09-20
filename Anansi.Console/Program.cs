namespace Anansi.Console
{
	using System;
	using Anansi.Kedei;

	class MainClass
	{
		public static void Main(string[] args)
		{
			using (var display = new LcdDisplay())
			{
				Console.WriteLine("Hello world!");
			}
		}
	}
}
