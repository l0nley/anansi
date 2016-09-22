namespace Anansi.Kedei
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Linq;
	using System.Net.NetworkInformation;

	public class AnansiLcd : IDisposable
	{
		readonly LcdDisplay _display;
		const uint MaxX = 480;
		const uint MaxY = 320;
		const uint Base = 0x20;
		const uint SnsStart = 20;
		const uint CWidth = 12;
		const uint CHeight = 16;
		const int SnsNameLength = 3;
		const int SnsValueLength = 9;
		const uint SysStart = 20;
		const uint Black = 0x00;
		const uint White = 0xff;
		const uint Magenta = 0xfa;
		const uint Red = 0xaa;
		const uint Green = 0xab;
		readonly List<LcdSensor> _sensors;
		bool disposed = false;
		int _lastId = -1;

		public AnansiLcd()
		{
			_display = new LcdDisplay();
			_sensors = new List<LcdSensor>();
		}

		private void DrawAreas()
		{
			_display.EmptyRectangle(0, 0, 180, MaxY, White, Black);
			_display.EmptyRectangle(180, 0, MaxX, 120, White, Black);
			_display.EmptyRectangle(180, 120, MaxX, MaxY, White, Black);
			_display.DrawString(2, 2, Base, White, "Sensors:");
			_display.DrawString(185, 2, Base, White, "System:");
		}


		public Task Init()
		{
			return Task.Run(() =>
			{
				_display.Init();
				_display.Clear(Black);
				_display.LoadFont("font.bmp", CWidth, CHeight, 96);
				DrawAreas();
			});
		}

		public int RegisterSensor(string name)
		{
			var sensor = new LcdSensor
			{
				Id = Interlocked.Increment(ref _lastId),
				Name = MakeItLength(name, SnsNameLength)
			};
			_sensors.Add(sensor);
			return sensor.Id;
		}

		public Task SetNetworkState(bool network, bool internet, NetworkInterface[] interfaces)
		{
			return Task.Run(() =>
			{
				_display.DrawString(185, SysStart, Base, White, "NET: ");
				_display.DrawString(185 + CWidth * 5, SysStart, Base, (network ? Green : Red), (network ? "ON  " : "OFF "));
				_display.DrawString(185 + CWidth * 9, SysStart, Base, White, "INET: ");
				_display.DrawString(185 + CWidth * 15, SysStart, Base, (internet ? Green : Red), (internet ? "ON " : "OFF"));
				var curh = SysStart + (CHeight + 1);
				for (var i = 0; i < 2; i++)
				{
					var s = "No interface";
					if (interfaces.Length > i)
					{
						var iface = interfaces[i];
						s = MakeItLength(iface.Id,5) + " " + GetOperationalStatus(iface.OperationalStatus) + " " +GetIp(iface);
					}
					_display.DrawString(185, curh, Base, White, MakeItLength(s,29));
					curh += CHeight + 1;
				}
			});
		}

		private string GetIp(NetworkInterface iface)
		{
			var addr = iface.GetIPProperties().UnicastAddresses.FirstOrDefault(_ => _.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
			var s = "No IP";
			if (addr != null)
			{
				s = "192.168.244.255";
			}
			return MakeItLength(s, 15);
		}

		private string GetOperationalStatus(OperationalStatus status)
		{
			switch (status)
			{
				case OperationalStatus.Up:
					return "U";
				case OperationalStatus.Down:
					return "D";
				default:
					return "U";
			}
		}

		private string MakeItLength(string s, uint length)
		{
			s = s.Trim();
			if (s.Length > length)
			{
				return s.Substring(0, (int)length);
			}
			if (s.Length == length)
			{
				return s;
			}
			while (s.Length < length)
			{
				s += " ";
			}
			return s;
		}

		public Task ChangeSensorValue(int id, string value)
		{
			return Task.Run(() => ChangeSensorValueImpl(id, value));
		}

		private void ChangeSensorValueImpl(int id, string value)
		{
			var sns = _sensors.FirstOrDefault(_ => _.Id == id);
			if (sns == null)
			{
				return;
			}
			sns.Value = MakeItLength(value, SnsValueLength);
			DisplaySensorValue(2, (uint)(SnsStart + sns.Id * (CHeight + 1)), sns.Name, sns.Value);
		}

		private void DisplaySensorValue(uint x, uint y, string name, string value)
		{
			_display.DrawString(x, y, Base, 0xff, name + ": " + value);
		}

		public void Dispose()
		{
			if (disposed == false)
			{
				_display.Dispose();
			}
		}
	}


}
