using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DxLibDLL;

namespace TakeUpJewel.GUI
{

	public static class ColorPallete
	{
		public static Color Red = Color.FromArgb(255, 0, 0);
		public static Color Orange = Color.FromArgb(255, 128, 0);
		public static Color Yellow = Color.FromArgb(255, 255, 0);
		public static Color LimeGreen = Color.FromArgb(0, 255, 0);
		public static Color Green = Color.FromArgb(0, 128, 64);
		public static Color Cyan = Color.FromArgb(0, 255, 255);
		public static Color Blue = Color.FromArgb(0, 0, 255);
		public static Color Purple = Color.FromArgb(128, 0, 255);
		public static Color Pink = Color.FromArgb(255, 0, 128);
		public static Color DarkRed = Color.FromArgb(164, 32, 0);
		public static Color White = Color.FromArgb(255, 255, 255);
		public static Color CreamSoda = Color.FromArgb(192, 255, 144);
		
		public static Color SkyBlue = Color.FromArgb(64, 172, 255);
		public static Color Windows98 = Color.FromArgb(0, 128, 128);
		public static Color LightPink = Color.FromArgb(255, 142, 142);
		public static Color LightOrange = Color.FromArgb(255, 192, 128);

		public static Color GetColorByIndex(int index)
		{
			switch (index)
			{
				case 0:
					return Red;
				case 1:
					return Orange;
				case 2:
					return Yellow;
				case 3:
					return LimeGreen;
				case 4:
					return Green;
				case 5:
					return Cyan;
				case 6:
					return Blue;
				case 7:
					return Purple;
				case 8:
					return Pink;
				case 9:
					return DarkRed;
				case 10:
					return White;
				case 11:
					return CreamSoda;
				case 12:
					return SkyBlue;
				case 13:
					return Windows98;
				case 14:
					return LightPink;
				case 15:
					return LightOrange;
				default:
					return Color.Black;

			}
		}



	}

	public class DxButton
	{
		/// <summary>
		/// 位置と大きさです。
		/// </summary>
		public Rectangle PointAndSize { get; set; }

		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }

		/// <summary>
		/// 表示するテキストです。
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// クリックされた時に実行するメソッドです。
		/// </summary>
		public Action<int, DxButton> ClickedAction { get; set; }

		/// <summary>
		/// マウスが押された時に実行するメソッドです。
		/// </summary>
		public Action<int, DxButton> MouseDownAction { get; set; }

		/// <summary>
		/// マウスが押されている間ずっと実行されるメソッドです。
		/// </summary>
		public Action<int, DxButton> MouseDownAction2 { get; set; }

		/// <summary>
		/// マウスが離された時に実行するメソッドです。
		/// </summary>
		public Action<int, DxButton> MouseUpAction { get; set; }

		Queue<Tuple<Action<int, DxButton>, int>> _taskqueue = new Queue<Tuple<Action<int, DxButton>, int>>();

		/// <summary>
		/// 四角形領域およびテキストを指定し、 DXButton の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="pointsize">位置と大きさ。</param>
		/// <param name="text">表示するテキスト。</param>
		public DxButton(Rectangle pointsize, string text, Color bc, Color fc)
		{
			PointAndSize = pointsize;
			Text = text;
			BackColor = bc;
			ForeColor = fc;

		}

		int _bmbtn;

		bool _isMouseDown;



		/// <summary>
		/// 描画とともに、イベントを処理します。
		/// </summary>
		public virtual void Draw()
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}


			DX.DrawBox(PointAndSize.X + Convert.ToByte(_isMouseDown), PointAndSize.Y + Convert.ToByte(_isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(_isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(_isMouseDown), DX.GetColor(Math.Abs(BackColor.R - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(BackColor.G - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(BackColor.B - 255 * Convert.ToByte(_isMouseDown))), 1);
			DX.DrawBox(PointAndSize.X + Convert.ToByte(_isMouseDown), PointAndSize.Y + Convert.ToByte(_isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(_isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(_isMouseDown), DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(_isMouseDown))), 0);
			DX.DrawString(PointAndSize.X + PointAndSize.Width / 2 - DX.GetDrawStringWidth(Text, Text.Length) / 2, PointAndSize.Y + PointAndSize.Height / 2 - DX.GetFontSize() / 2, Text, DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(_isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(_isMouseDown))));

			int mx, my, mbtn;


			DX.GetMousePoint(out mx, out my);
			mbtn = DX.GetMouseInput();
			var contain = PointAndSize.Contains(mx, my);


			if (_bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ClickedAction != null)
					_taskqueue.Enqueue(Tuple.Create<Action<int, DxButton>, int>(ClickedAction, _bmbtn));

				//Console.WriteLine("debug_isclicked");
			}
			if (_bmbtn == 0 && mbtn > 0 && !_isMouseDown && contain)
			{

				_isMouseDown = true;
				if (MouseDownAction != null)
					_taskqueue.Enqueue(Tuple.Create<Action<int, DxButton>, int>(MouseDownAction, mbtn));
			}

			if (mbtn > 0 && contain && _isMouseDown)
			{
				if (MouseDownAction2 != null)
					_taskqueue.Enqueue(Tuple.Create<Action<int, DxButton>, int>(MouseDownAction2, mbtn));
			}

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (_bmbtn > 0 && mbtn == 0 && _isMouseDown)
			{
				_isMouseDown = false;
				if (MouseUpAction != null)
					_taskqueue.Enqueue(Tuple.Create<Action<int, DxButton>, int>(MouseUpAction, _bmbtn));
				//Console.WriteLine("debug_ismouseup");
			}

			if (_taskqueue.Count > 0)
			{
				var temp = _taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			_bmbtn = mbtn;
		}

	}

	public class DxTrackBar
	{
		/// <summary>
		/// 位置と大きさです。
		/// </summary>
		Rectangle PointAndSize { get; set; }

		/// <summary>
		/// 背景色です。
		/// </summary>
		public Color BackColor { get; set; }
		/// <summary>
		/// 前景色です。
		/// </summary>
		public Color ForeColor { get; set; }

		/// <summary>
		/// 値です。
		/// </summary>
		public int Value { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsValueChanging { get; private set; }

		/// <summary>
		/// 値の変更が終わった瞬間のイベントです。
		/// </summary>
		public Action<int, DxTrackBar> ValueChangedAction { get; set; }

		Queue<Tuple<Action<int, DxTrackBar>, int>> _taskqueue = new Queue<Tuple<Action<int, DxTrackBar>, int>>();

		/// <summary>
		/// 最大値です。
		/// </summary>
		public int MaxValue { get; set; }

		/// <summary>
		/// 矩形領域、色、最大値を指定し、 DXLib で動作するトラックバーの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="pointsize">位置と大きさ。</param>
		/// <param name="text">表示するテキスト。</param>
		/// <param name="bc">背景色。</param>
		/// <param name="fc">前景色。</param>
		/// <param name="mv">最大値。</param>
		public DxTrackBar(Rectangle pointsize, Color bc, Color fc, int mv)
		{
			PointAndSize = pointsize;
			BackColor = bc;
			ForeColor = fc;
			MaxValue = mv;
		}

		int _bmbtn;

		//bool isMouseDown = false;



		/// <summary>
		/// 描画とともに、イベントを処理します。
		/// </summary>
		public void Draw()
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}




			int mx, my, mbtn;




			DX.GetMousePoint(out mx, out my);
			mbtn = DX.GetMouseInput();
			var contain = PointAndSize.Contains(mx, my);

			if (mbtn == 1 && contain)
			{
				IsValueChanging = true;
				Value = (mx - PointAndSize.X) * (MaxValue / (PointAndSize.Width - PointAndSize.X));
			}

			if (_bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ValueChangedAction != null)
					_taskqueue.Enqueue(Tuple.Create<Action<int, DxTrackBar>, int>(ValueChangedAction, Value));
				IsValueChanging = false;
				//Console.WriteLine("debug_isclicked");
			}

			var dx = PointAndSize.X + Value * (PointAndSize.Width - PointAndSize.X) / MaxValue;
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, (uint)BackColor.ToArgb(), 1);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, (uint)ForeColor.ToArgb(), 0);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, dx, PointAndSize.Y + PointAndSize.Height, (uint)ForeColor.ToArgb(), 1);

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (_taskqueue.Count > 0)
			{
				var temp = _taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			_bmbtn = mbtn;
		}

	}

	public class DxLabel
	{
		/// <summary>
		/// 現在のラベルの位置です。
		/// </summary>
		public PointF Location { get; set; }

		public float Spdx, Spdy;

		/// <summary>
		/// ラベルで表示する文字列です。
		/// </summary>
		public string Text { get; set; }

		public Color ForeColor { get; set; }
		public Color BackColor { get; set; }

		/// <summary>
		/// 位置、テキスト、色を指定して、DXLib 上の新しい DXLabel を初期化します。
		/// </summary>
		/// <param name="l"></param>
		/// <param name="txt"></param>
		public DxLabel(PointF l, string txt, Color bc, Color fc)
			: this(l, txt, bc, fc, 0, 0) { }

		public DxLabel(PointF l, string txt, Color bc, Color fc, float speedx, float speedy)
		{
			Location = l;
			Text = txt;
			BackColor = bc;
			ForeColor = fc;
			Spdx = speedx;
			Spdy = speedy;
		}

		public DxLabel(float x, float y, string txt, Color bc, Color fc)
			: this(new PointF(x, y), txt, bc, fc)
		{
		}



		public void Draw()
		{
			//if (BackColor != Color.Transparent)
			//	DX.DrawBox((int)Location.X, (int)Location.Y, (int)Location.X + DX.GetDrawStringWidth(Text, Text.Length), (int)Location.Y + DX.GetFontSize() * , BackColor.ToArgb(), 1);
			var hoge = Text.Split('\n');
			var height = DX.GetFontSize();
			for (var i = 0; i < hoge.Length; i++)
			{
				DX.DrawString((int)Location.X, (int)Location.Y + i * height, hoge[i], (uint)ForeColor.ToArgb());
			}
		}
	}

	public class DxOrb
	{
		public Point Location;

		public SizeF NowSize;
		public Color Color;

		public float Speed;

		public DxOrb(Point loc, SizeF size, Color col, float spd)
		{
			Location = loc;
			NowSize = size;
			Color = col;
			Speed = spd;
		}

		public void Update()
		{
			NowSize.Width /= Speed;
			NowSize.Height /= Speed;
			if (NowSize.Width < 1)
				NowSize.Width = 0;
			if (NowSize.Height < 1)
				NowSize.Height = 0;
			if (NowSize.Width != 0.0 && NowSize.Height != 0.0)

				DX.DrawOval(Location.X, Location.Y, (int)NowSize.Width, (int)NowSize.Height, (uint)Color.ToArgb(), 0, 2);
		}

	}

	public class NumFont
	{
		public static ushort[] Numfont = new ushort[]
				{
					0x7b6f,	//0
					0x6497,	//1
					0x73e7,	//2
					0x73cf,	//3
					0x5bc9,	//4
					0x79cf,	//5
					0x79ef,	//6
					0x7b49,	//7
					0x7bef,	//8
					0x7bcf,	//9
				};

		public static void DrawNumFontString(int x, int y, uint col, string number)
		{
			int tx = x, ty = y;
			foreach (var c in number)
			{
				if (c == '\n')
				{
					ty += 6;	//LF
					tx = x;		//CR
					continue;
				}
				if (!char.IsNumber(c))
					throw new ArgumentException("描画する文字列は、0～9の数字だけでなければなりません。");
				DrawNumFont(tx, ty, col, Numfont[int.Parse(c.ToString())]);
				tx += 4;
			}
		}

		public static void DrawNumFont(int x, int y, uint col, int font)
		{
			//粗いけどこれ以外に方法が思いつかなかった(´･_･`)
			DX.DrawPixel(x, y, ((font & 16384) == 16384) ? col : 0);	// 0, 0
			DX.DrawPixel(x + 1, y, ((font & 8192) == 8192) ? col : 0);	// 1, 0
			DX.DrawPixel(x + 2, y, ((font & 4096) == 4096) ? col : 0);	// 2, 0
			DX.DrawPixel(x, y + 1, ((font & 2048) == 2048) ? col : 0);	// 0, 1
			DX.DrawPixel(x + 1, y + 1, ((font & 1024) == 1024) ? col : 0);	// 1, 1
			DX.DrawPixel(x + 2, y + 1, ((font & 512) == 512) ? col : 0);	// 2, 1
			DX.DrawPixel(x, y + 2, ((font & 256) == 256) ? col : 0);	// 0, 2
			DX.DrawPixel(x + 1, y + 2, ((font & 128) == 128) ? col : 0);	// 1, 2
			DX.DrawPixel(x + 2, y + 2, ((font & 64) == 64) ? col : 0);	// 2, 2
			DX.DrawPixel(x, y + 3, ((font & 32) == 32) ? col : 0);	// 0, 3
			DX.DrawPixel(x + 1, y + 3, ((font & 16) == 16) ? col : 0);	// 1, 3
			DX.DrawPixel(x + 2, y + 3, ((font & 8) == 8) ? col : 0);	// 2, 3
			DX.DrawPixel(x, y + 4, ((font & 4) == 4) ? col : 0);	// 0, 4
			DX.DrawPixel(x + 1, y + 4, ((font & 2) == 2) ? col : 0);	// 1, 4
			DX.DrawPixel(x + 2, y + 4, ((font & 1) == 1) ? col : 0);	// 2, 4
		}

	}

}
