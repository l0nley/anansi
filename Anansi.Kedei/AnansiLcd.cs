namespace Anansi.Kedei
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Threading;

	public class AnansiLcd : IDisposable
	{
		readonly LcdDisplay _display;
		const uint MaxX = 480;
		const uint MaxY = 320;
		const uint Base = 20;
		readonly List<LcdSensor> _sensors;
		int _lastId = 0;

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
				_display.LoadFont("font.bmp", 12, 16, 96);
				DrawAreas();
			});
		}

		public int RegisterSensor(string name)
		{
			var sensor = new LcdSensor
			{
				Id = Interlocked.Increment(ref _lastId),
				Name = name
			};
    		_sensors.Add(sensor);
			return sensor.Id;
		}

		public Task ChangeSensorValue(int id, string value)
		{
			return Task.Run(() => ChangeSensorValueImpl(id, value));
		}

		private void ChangeSensorValueImpl(int id, string value)
		{
			
		}

		public void Dispose()
		{
			_display.Dispose();
		}
	}

	
}
