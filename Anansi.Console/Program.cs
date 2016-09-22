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
				Console.WriteLine("LCD Init");
				display.Init();
				Console.WriteLine("LCD Clear");
				display.Clear(0x00);
				Console.WriteLine("LCD Reactangle");
				display.Rectangle(10, 10, 30, 40, 0xFF);
				Console.WriteLine("LCD Rectangle Empty");
				display.EmptyRectangle(10, 50, 30, 70, 0xFF, 0xAD);
				Console.WriteLine("LCD Load Font");
				display.LoadFont("font.bmp", 12, 16, 96);
				Console.WriteLine("LCD Display symbol");
				display.DrawSymbol(10, 70, (char)0x20, 0xFF);
			}
		}
	}
}
