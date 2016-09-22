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
		readonly List<LcdSensor> _sensors;
		bool disposed = false;
		int _lastId = -1;
		readonly Timer _timer;

		public AnansiLcd()
		{
			_display = new LcdDisplay();
			_sensors = new List<LcdSensor>();
			_timer = new Timer(HandleTimerCallback, null, 200, 1000);
		}

		private void DrawAreas()
		{
			_display.EmptyRectangle(0, 0, 180, MaxY,0xff,0x00);
			_display.EmptyRectangle(180, 0, MaxX, 120, 0xff, 0x00);
			_display.EmptyRectangle(180, 120, MaxX, MaxY, 0xff, 0x00);
			_display.DrawString(2, 2, Base, 0xff, "Sensors:");
		}

		private void HandleTimerCallback(object state)
		{
			var networkAvailiable = NetworkInterface.GetIsNetworkAvailable();
			var interfaces = NetworkInterface.GetAllNetworkInterfaces();
			Task.Run(() => _display.DrawString(185, 2, Base, 0xff, "System:"));
			Task.Run(() => _display.DrawString(185, CHeight + 4, Base, 0xff, "NET: "));
			Task.Run(() => _display.DrawString(CWidth * 5, CHeight + 4, Base, ((uint)(networkAvailiable ? 0x00FF00 : 0xFF0000)), (networkAvailiable ? "ON " : "OFF")));
			var curh = CHeight * 2 + 4 + 1;
			for (var i = 0; i < 5; i++)
			{
				string s;
				if (interfaces.Length > i)
				{
					var iface = interfaces[i];
					s = MakeItLength(iface.Id, 4) + " " + MakeItLength(iface.NetworkInterfaceType.ToString(), 5);
				}
				else {
					s = MakeItLength(string.Empty, 20) + "!";
				}
				Task.Run(() => _display.DrawString(185, curh, Base, 0xff, s));
				curh += CHeight + 1;
			}
			                    
		}

		public Task Init()
		{
			return Task.Run(() =>
			{
				_display.Init();
				_display.Clear(0x00);
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
				_timer.Dispose();
				_display.Dispose();
			}
		}
	}

	
}
