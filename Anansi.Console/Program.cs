namespace Anansi.Console
{
	using System;
	using Anansi.Kedei;
	using System.Threading.Tasks;

	class MainClass
	{
		public static void Main(string[] args)
		{
			using (var display = new AnansiLcd())
			{
				Task.Run(() => display.Init()).Wait();
			}
		}
	}
}
