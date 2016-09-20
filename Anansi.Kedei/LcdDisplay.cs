namespace Anansi.Kedei
{
	using System;
	using System.Runtime.InteropServices;

	public class LcdDisplay : IDisposable
	{
		[DllImport("libkedei.so")]
		static extern void lcd_open();
		[DllImport("libkedei.so")]
		static extern void lcd_init();
		[DllImport("libkedei.so")]
		static extern void lcd_close();
		[DllImport("libkedei.so")]
		static extern void lcd_clear(int clr);
		[DllImport("libkedei.so")]
		static extern void lcd_rectangle(int x, int y, int ex, int ey, int clr);
		[DllImport("libkedei.so")]
		static extern void lcd_rectangle_empty(int x, int y, int ex, int ey, int clr1, int clr2);

		public void Init()
		{
			lcd_open();
			lcd_init();
		}

		public void Rectangle(int x, int y, int ex, int ey, int color)
		{
			lcd_rectangle(x, y, ex, ey, color);
		}

		public void EmptyRectangle(int x, int y, int ex, int ey, int borderColor, int background)
		{
			lcd_rectangle_empty(x, y, ex, ey, borderColor, background);
		}

		public void Clear(int color)
		{
			lcd_clear(color);
		}

		public void Dispose()
		{
			lcd_close();
		}
	}
}
