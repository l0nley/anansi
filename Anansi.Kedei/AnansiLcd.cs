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
			_display.Rectangle(0, 0, MaxX, MaxY, 0xff);
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
