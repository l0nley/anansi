namespace Anansi.Kedei
{
	using System;
	using System.Runtime.InteropServices;
	using System.Drawing;
	using System.Collections.Generic;

	public class LcdDisplay : IDisposable
	{
		[DllImport("libkedei.so")]
		static extern void lcd_open();
		[DllImport("libkedei.so")]
		static extern void lcd_init();
		[DllImport("libkedei.so")]
		static extern void lcd_close();
		[DllImport("libkedei.so")]
		static extern void lcd_clear(uint clr);
		[DllImport("libkedei.so")]
		static extern void lcd_rectangle(uint x, uint y, uint ex, uint ey, uint clr);
		[DllImport("libkedei.so")]
		static extern void lcd_rectangle_empty(uint x, uint y, uint ex, uint ey, uint clr1, uint clr2);
		[DllImport("libkedei.so")]
		static extern void lcd_reset();
		[DllImport("libkedei.so")]
		static extern void lcd_load_chars(uint cwidth, uint cheight, uint ccout, [In]uint[] matrix, uint length);
		[DllImport("libkedei.so")]
		static extern void lcd_draw_symbol(uint x, uint y, uint sym, uint color);

		public void Init()
		{
			lcd_open();
			lcd_init();
		}

		public void Rectangle(uint x, uint y, uint ex, uint ey, uint color)
		{
			lcd_rectangle(x, y, ex, ey, color);
		}

		public void Reset() {
			lcd_reset();
		}

		public void DrawSymbol(uint x, uint y, uint symbol, uint color)
		{
			lcd_draw_symbol(x, y, symbol, color);
		}

		public void LoadFont(string fileName, uint cwidth, uint cheight, uint ccount) {
			var matrix = new List<uint>();
			using (var image = new Bitmap(fileName))
			{
				for (var y = 0; y < image.Height; y++)
				{
					for (var x = 0; x < image.Width; x++)
					{
						matrix.Add((uint)image.GetPixel(x, y).ToArgb());
					}
				}
			}
			var matrixArray = matrix.ToArray();
			lcd_load_chars(cwidth, cheight, ccount, matrixArray, (uint)matrixArray.Length);
		}

		public void EmptyRectangle(uint x, uint y, uint ex, uint ey, uint borderColor, uint background)
		{
			lcd_rectangle_empty(x, y, ex, ey, borderColor, background);
		}

		public void Clear(uint color)
		{
			lcd_clear(color);
		}

		public void Dispose()
		{
			lcd_close();
		}
	}
}
