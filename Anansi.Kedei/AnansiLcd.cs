namespace Anansi.Kedei
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Threading;
	using System.Linq;

	public class AnansiLcd : IDisposable
	{
		readonly LcdDisplay _display;
		const uint MaxX = 480;
		const uint MaxY = 320;
		const uint Base = 0x20;
		const uint SnsStart = 20;
		const uint CWidth = 12;
		const uint CHeight = 16;
		readonly List<LcdSensor> _sensors;
		int _lastId = -1;

		public AnansiLcd()
		{
			_display = new LcdDisplay();
			_sensors = new List<LcdSensor>();
		}

		private void DrawAreas()
		{
			_display.EmptyRectangle(0, 0, 180, MaxY,0xff,0x00);
			_display.EmptyRectangle(180, 0, MaxX, 120, 0xff, 0x00);
			_display.EmptyRectangle(180, 120, MaxX, MaxY, 0xff, 0x00);
			_display.DrawString(2, 2, Base, 0xff, "Sensors:");
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
				Name = MakeItLength(name, 3)
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
				s += "!";
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
			sns.Value = MakeItLength(value, 8);
			DisplaySensorValue(2, (uint)(SnsStart + sns.Id * (CHeight + 1)), sns.Name, sns.Value);
		}

		private void DisplaySensorValue(uint x, uint y, string name, string value)
		{
			_display.DrawString(x, y, Base, 0xff, name + ": " + value);
		}

		public void Dispose()
		{
			_display.Dispose();
		}
	}

	
}
