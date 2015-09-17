using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DxLibDLL;

namespace DefenderStory.GUI
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

	public class DXButton
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
		public Action<int, DXButton> ClickedAction { get; set; }

		/// <summary>
		/// マウスが押された時に実行するメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseDownAction { get; set; }

		/// <summary>
		/// マウスが押されている間ずっと実行されるメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseDownAction2 { get; set; }

		/// <summary>
		/// マウスが離された時に実行するメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseUpAction { get; set; }

		Queue<Tuple<Action<int, DXButton>, int>> taskqueue = new Queue<Tuple<Action<int, DXButton>, int>>();

		/// <summary>
		/// 四角形領域およびテキストを指定し、 DXButton の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="pointsize">位置と大きさ。</param>
		/// <param name="text">表示するテキスト。</param>
		public DXButton(Rectangle pointsize, string text, Color bc, Color fc)
		{
			PointAndSize = pointsize;
			Text = text;
			BackColor = bc;
			ForeColor = fc;

		}

		int bmbtn = 0;

		bool isMouseDown = false;



		/// <summary>
		/// 描画とともに、イベントを処理します。
		/// </summary>
		public virtual void Draw()
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}


			DX.DrawBox(PointAndSize.X + Convert.ToByte(isMouseDown), PointAndSize.Y + Convert.ToByte(isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(isMouseDown), DX.GetColor(Math.Abs(BackColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(BackColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(BackColor.B - 255 * Convert.ToByte(isMouseDown))), 1);
			DX.DrawBox(PointAndSize.X + Convert.ToByte(isMouseDown), PointAndSize.Y + Convert.ToByte(isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(isMouseDown), DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(isMouseDown))), 0);
			DX.DrawString(PointAndSize.X + PointAndSize.Width / 2 - DX.GetDrawStringWidth(this.Text, this.Text.Length) / 2, PointAndSize.Y + PointAndSize.Height / 2 - DX.GetFontSize() / 2, this.Text, DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(isMouseDown))));

			int mx, my, mbtn;


			DX.GetMousePoint(out mx, out my);
			mbtn = DX.GetMouseInput();
			bool contain = PointAndSize.Contains(mx, my);


			if (bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ClickedAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(ClickedAction, bmbtn));

				//Console.WriteLine("debug_isclicked");
			}
			if (bmbtn == 0 && mbtn > 0 && !isMouseDown && contain)
			{

				isMouseDown = true;
				if (MouseDownAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseDownAction, mbtn));
			}

			if (mbtn > 0 && contain && isMouseDown)
			{
				if (MouseDownAction2 != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseDownAction2, mbtn));
			}

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (bmbtn > 0 && mbtn == 0 && isMouseDown)
			{
				isMouseDown = false;
				if (MouseUpAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseUpAction, bmbtn));
				//Console.WriteLine("debug_ismouseup");
			}

			if (taskqueue.Count > 0)
			{
				Tuple<Action<int, DXButton>, int> temp = taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			bmbtn = mbtn;
		}

	}

	public class DXTrackBar
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
		public Action<int, DXTrackBar> ValueChangedAction { get; set; }

		Queue<Tuple<Action<int, DXTrackBar>, int>> taskqueue = new Queue<Tuple<Action<int, DXTrackBar>, int>>();

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
		public DXTrackBar(Rectangle pointsize, Color bc, Color fc, int mv)
		{
			PointAndSize = pointsize;
			BackColor = bc;
			ForeColor = fc;
			MaxValue = mv;
		}

		int bmbtn = 0;

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
			bool contain = PointAndSize.Contains(mx, my);

			if (mbtn == 1 && contain)
			{
				this.IsValueChanging = true;
				Value = (mx - PointAndSize.X) * (MaxValue / (PointAndSize.Width - PointAndSize.X));
			}

			if (bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ValueChangedAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXTrackBar>, int>(ValueChangedAction, this.Value));
				this.IsValueChanging = false;
				//Console.WriteLine("debug_isclicked");
			}

			int dx = PointAndSize.X + Value * (PointAndSize.Width - PointAndSize.X) / MaxValue;
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, (uint)BackColor.ToArgb(), 1);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, (uint)ForeColor.ToArgb(), 0);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, dx, PointAndSize.Y + PointAndSize.Height, (uint)ForeColor.ToArgb(), 1);

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (taskqueue.Count > 0)
			{
				Tuple<Action<int, DXTrackBar>, int> temp = taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			bmbtn = mbtn;
		}

	}

	public class DXLabel
	{
		/// <summary>
		/// 現在のラベルの位置です。
		/// </summary>
		public PointF Location { get; set; }

		public float spdx, spdy;

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
		public DXLabel(PointF l, string txt, Color bc, Color fc)
			: this(l, txt, bc, fc, 0, 0) { }

		public DXLabel(PointF l, string txt, Color bc, Color fc, float speedx, float speedy)
		{
			Location = l;
			Text = txt;
			BackColor = bc;
			ForeColor = fc;
			spdx = speedx;
			spdy = speedy;
		}

		public DXLabel(float x, float y, string txt, Color bc, Color fc)
			: this(new PointF(x, y), txt, bc, fc)
		{
		}



		public void Draw()
		{
			//if (BackColor != Color.Transparent)
			//	DX.DrawBox((int)Location.X, (int)Location.Y, (int)Location.X + DX.GetDrawStringWidth(Text, Text.Length), (int)Location.Y + DX.GetFontSize() * , BackColor.ToArgb(), 1);
			string[] hoge = Text.Split('\n');
			int height = DX.GetFontSize();
			for (int i = 0; i < hoge.Length; i++)
			{
				DX.DrawString((int)Location.X, (int)Location.Y + i * height, hoge[i], (uint)ForeColor.ToArgb());
			}
		}
	}

	public class DXOrb
	{
		public Point location;

		public SizeF nowSize;
		public Color color;

		public float speed;

		public DXOrb(Point loc, SizeF size, Color col, float spd)
		{
			location = loc;
			nowSize = size;
			color = col;
			speed = spd;
		}

		public void Update()
		{
			nowSize.Width /= speed;
			nowSize.Height /= speed;
			if (nowSize.Width < 1)
				nowSize.Width = 0;
			if (nowSize.Height < 1)
				nowSize.Height = 0;
			if (nowSize.Width != 0.0 && nowSize.Height != 0.0)

				DX.DrawOval(location.X, location.Y, (int)nowSize.Width, (int)nowSize.Height, (uint)color.ToArgb(), 0, 2);
		}

	}

	public class NumFont
	{
		public static ushort[] numfont = new ushort[]
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
			foreach (char c in number)
			{
				if (c == '\n')
				{
					ty += 6;	//LF
					tx = x;		//CR
					continue;
				}
				if (!char.IsNumber(c))
					throw new ArgumentException("描画する文字列は、0～9の数字だけでなければなりません。");
				DrawNumFont(tx, ty, col, numfont[int.Parse(c.ToString())]);
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
