namespace Anansi.Kedei
{
	using System;
	using System.Threading.Tasks;

	public class AnansiLcd : IDisposable
	{
		readonly LcdDisplay _display;
		const uint MaxX = 480;
		const uint MaxY = 320;

		public AnansiLcd()
		{
			_display = new LcdDisplay();
		}

		private void DrawAreas()
		{
			_display.EmptyRectangle(0, 0, 180, MaxY,0x00,0xff);
			_display.EmptyRectangle(180, 0, MaxX, 120, 0x00, 0xff);
			_display.EmptyRectangle(180, 120, MaxX, MaxY, 0x00, 0xff);
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

		public void Dispose()
		{
			_display.Dispose();
		}
	}
}
