namespace Anansi.Console
{
	using System;
	using Anansi.Kedei;
	using System.Threading.Tasks;
	using System.Net.NetworkInformation;
	using System.Linq;

	class MainClass
	{
		public static void Main(string[] args)
		{
			using (var display = new AnansiLcd())
			{
				Task.Run(() => display.Init()).Wait();
				var id = display.RegisterSensor("TEMP");
				Task.Run(() => display.ChangeSensorValue(id, "128")).Wait();
				var id2 = display.RegisterSensor("SMOKE");
				Task.Run(() => display.ChangeSensorValue(id2,"asdsads")).Wait();
				Task.Run(() => display.SetNetworkState(true, false, NetworkInterface.GetAllNetworkInterfaces().Where(_=>_.Name!="lo").ToArray())).Wait();
			}
		}
	}
}
