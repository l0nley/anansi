namespace Anansi.Kedei
{
	using System;
	using System.Runtime.InteropServices;
	using System.Drawing;
	using System.Collections.Generic;
	using System.IO;

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
		[DllImport("libkedei.so")]
		static extern void lcd_reset();
		[DllImport("libkedei.so")]
		static extern void lcd_load_chars(int cwidth, int cheight, int ccout, ref IntPtr matrix);
		[DllImport("libkedei.so")]
		static extern void lcd_draw_symbol(int x, int y, int sym, int color);

		readonly List<IntPtr> pointers;

		public LcdDisplay()
		{
			pointers = new List<IntPtr>();
		}

		public void Init()
		{
			lcd_open();
			lcd_init();
		}

		public void Rectangle(int x, int y, int ex, int ey, int color)
		{
			lcd_rectangle(x, y, ex, ey, color);
		}

		public void Reset() {
			lcd_reset();
		}

		public void DrawSymbol(int x, int y, char symbol, int color)
		{
			lcd_draw_symbol(x, y, symbol, color);
		}

		public void LoadFont(string fileName, int cwidth, int cheight, int ccount) {
			var matrix = new List<int>();
			using (var image = new Bitmap(fileName))
			{
				for (var i = 0; i < image.Height; i++)
				{
					for (var j = 0; j < image.Width; j++)
					{
						Console.WriteLine(i.ToString() + ":" + j.ToString());
						matrix.Add(image.GetPixel(i, j).ToArgb());
					}
				}
			}
			var matrixArray = matrix.ToArray();
			var buffer = Marshal.AllocCoTaskMem(sizeof(uint) * matrixArray.Length);
			Marshal.Copy(matrixArray, 0, buffer, matrixArray.Length);
			pointers.Add(buffer);
			lcd_load_chars(cwidth, cheight, ccount, ref buffer);
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
			foreach (var ptr in pointers)
			{
				try
				{
					Marshal.FreeCoTaskMem(ptr);
				}
				catch 
				{
				}
			}
			pointers.Clear();
			lcd_close();
		}
	}
}
