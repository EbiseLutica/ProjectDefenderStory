using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
	public partial class ObjectSelectForm : Form
	{
		public ObjectSelectForm()
		{
			InitializeComponent();
			listBox1.SelectedIndex = 0;
		}

		public RequestFlag request = RequestFlag.None;

		public string Path
		{
			get
			{
				return this.toolStripButton2.Text;
			}
			private set
			{
				this.toolStripButton2.Text = value;
			}
		}

		public bool ForeVisible
		{
			get
			{
				return toolStripButton9.Checked;
			}
			set
			{
				toolStripButton9.Checked = value;
			}
		}

		public bool BackVisible
		{
			get
			{
				return toolStripButton8.Checked;
			}
			set
			{
				toolStripButton8.Checked = value;
			}
		}

		public string StatusMessage
		{
			get
			{
				return statusStrip1.Items[0].Text;
			}
			set
			{
				statusStrip1.Items[0].Text = value;
			}
		}

		public ObjectSelectForm(string filename)
		{
			InitializeComponent();
			if (filename != null)
				this.pictureBox1.Image = new Bitmap(filename);
			Task.Factory.StartNew(new Action(()=>
				{
					while (true)
					{
						if (!this.IsHandleCreated)
							continue;
						this.Invoke(new MethodInvoker(() =>
							{
								pictureBox1.Refresh();
							}));
						gradient = (gradient + 10) % 360;
						Thread.Sleep(16);
						
					}
				}));
		}

		public int chipno = 0;

		public Random rnd = new Random(100);

		ToolFlag _tool = ToolFlag.Pen;

		public ToolFlag Tool
		{
			get
			{
				return _tool;
			}
			set
			{
				_tool = value;
				toolStripButton5.Checked = value == ToolFlag.Pen;
				toolStripButton6.Checked = value == ToolFlag.Line;
				toolStripButton4.Checked = value == ToolFlag.Select;
				toolStripButton7.Checked = value == ToolFlag.Fill;
				toolStripButton13.Checked = value == ToolFlag.SpPut;
				toolStripButton14.Checked = value == ToolFlag.SpSel;
				toolStripButton12.Checked = value == ToolFlag.SpDel;
				toolStripButton15.Checked = value == ToolFlag.SpVisible;

			}
		}
		int tmpx = -1, tmpy = -1;

		public ChipPack chippack = new ChipPack(1, 1, 0);

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			tmpx = e.X / 16;
			tmpy = e.Y / 16;
			chipno = e.Y / 16 * 16 + e.X / 16;
			this.Refresh();
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			int w = Math.Abs(e.X / 16 - tmpx) + 1, h = Math.Abs(e.Y / 16 - tmpy) + 1;
			byte[] c = new byte[w * h];
			int ox = tmpx, oy = tmpy;
			if (e.X / 16 < tmpx)
				ox = e.X / 16;
			if (e.Y / 16 < tmpy)
				oy = e.Y / 16;
			for (int x = ox; x < ox + w; x++)
				for (int y = oy; y < oy + h; y++)
					c[(y - oy) * w + (x - ox)] = (byte)(y * 16 + x);
			chippack = new ChipPack(w, h, c);
			tmpx = tmpy = -1;
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			if (tmpx == -1 || tmpy == -1)
				return;
			int w = Math.Abs(e.X / 16 - tmpx) + 1, h = Math.Abs(e.Y / 16 - tmpy) + 1;
			byte[] c = new byte[w * h];
			int ox = tmpx, oy = tmpy;
			if (e.X / 16 < tmpx)
				ox = e.X / 16;
			if (e.Y / 16 < tmpy)
				oy = e.Y / 16;
			for (int x = ox; x < ox + w; x++)
				for (int y = oy; y < oy + h; y++)
					c[(y - oy) * w + (x - ox)] = (byte)(y * 16 + x);
			chippack = new ChipPack(w, h, c);
		}


		int gradient = 255;

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			int dat = (int)(Math.Sin(gradient / 180.0 * Math.PI) * 127 + 127);
			foreach (byte chip in chippack.chips)
				e.Graphics.DrawRectangle(new Pen(Color.FromArgb(dat, dat, dat), 1), chip % 16 * 16, chip / 16 * 16, 16, 16);
			//Console.Write(dat + " ");
		}
		public ScreenFlag sf = ScreenFlag.Fore;

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (sf == ScreenFlag.Fore)
			{
				sf = ScreenFlag.Back;
				toolStripButton1.Text = "裏";
			}
			else
			{
				sf = ScreenFlag.Fore;
				toolStripButton1.Text = "表";
			}
		}

		private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
		{

		}

		

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			var filename = "Resources\\Graphics\\" + toolStripButton2.Text + ".png";
			if (System.IO.File.Exists(filename))
				this.pictureBox1.Image = new Bitmap(filename);
			request = RequestFlag.ChangeMpt;
		}

		//選択
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.Select;
		}

		//ペン
		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.Pen;
		}

		//線
		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.Line;
		}

		//塗りつぶし
		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.Fill;
		}

		private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		public Size MapSize
		{
			get
			{
				return new Size(int.Parse(numericTextBox1.Text), int.Parse(numericTextBox2.Text));
			}
			set
			{
				numericTextBox1.Text = value.Width.ToString();
				numericTextBox2.Text = value.Height.ToString();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			request = RequestFlag.Resize;
		}

		int[] zoom = new[] { 10, 20, 50, 70, 100 };

		public int pzoom = 4;

		public int Zoom
		{
			get
			{
				return zoom[pzoom];
			}
		}

		//拡大
		private void button2_Click(object sender, EventArgs e)
		{
			pzoom++;
			if (pzoom > 4)
				pzoom = 4;
		}

		//縮小
		private void button3_Click(object sender, EventArgs e)
		{
			pzoom--;
			if (pzoom < 0)
				pzoom = 0;
		}

		public bool GridVisible
		{
			get
			{
				return toolStripButton11.Checked;
			}
			set
			{
				toolStripButton11.Checked = value;
			}
		}

		public Color RequestedColor
		{
			get
			{
				return toolStripButton16.BackColor;
			}
			set
			{
				toolStripButton16.BackColor = value;
			}
		}

		private void toolStripButton11_Click(object sender, EventArgs e)
		{

		}

		private void 新規作成NToolStripButton_Click(object sender, EventArgs e)
		{
			request = RequestFlag.CreateNew;
		}

		private void 開くOToolStripButton_Click(object sender, EventArgs e)
		{
			request = RequestFlag.OpenFile;
		}

		private void 上書き保存SToolStripButton_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SaveCitMap;
		}

		private void toolStripButton13_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.SpPut;
		}

		private void toolStripButton14_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.SpSel;
		}

		private void toolStripButton12_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.SpDel;
		}

		public int SpriteID
		{
			get
			{
				return this.listBox1.SelectedIndex;
			}
			set
			{
				this.listBox1.SelectedIndex = value;
			}
		}

		private void toolStripButton15_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.SpVisible;
		}

		private void citmapを保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SaveCitMap;
		}

		private void spdataを保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SaveSpdata;
		}

		private void マップを開くToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void spdataを開くToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void 上書き保存SToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			request = RequestFlag.SaveCitMap;
		}

		private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
		{

		}

		//↑
		private void button4_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SwapT;
		}

		//↓
		private void button5_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SwapB;
		}

		//←
		private void button6_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SwapL;
		}

		//→
		private void button7_Click(object sender, EventArgs e)
		{
			request = RequestFlag.SwapR;
		}

		private void toolStripButton16_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
				return;
			RequestedColor = cd.Color;
			request = RequestFlag.ChangeColor;
		}

	}

	public enum RequestFlag
	{
		None, Resize, ChangeMpt, CreateNew, OpenFile, SaveCitMap, SaveSpdata, ChangeColor, SwapL, SwapR, SwapT, SwapB
	}


	public enum ToolFlag
	{
		Pen, Line, Select, Fill, SpPut, SpSel, SpDel, SpVisible
	}

	public enum ScreenFlag
	{
		Fore, Back
	}

	public class NumericTextBox : TextBox
	{
		public NumericTextBox()
		{
		}

		private char[] permitChars = new []{'0','1','2','3','4','5','6','7','8','9', '0', '-', '.', };

		protected override void WndProc(ref Message m)
		{
			const int WM_CHAR = 0x0102;
			const int WM_PASTE = 0x0302;

			switch (m.Msg)
			{
				case WM_CHAR:
					if ((this.permitChars != null) && (this.permitChars.Length > 0))
					{
						KeyPressEventArgs e = new KeyPressEventArgs((char)(m.WParam.ToInt32()));
						this.OnChar(e);

						if (e.Handled)
							return;
					}
					break;
				case WM_PASTE:
					if ((this.permitChars != null) && (this.permitChars.Length > 0))
					{
						this.OnPaste(new System.EventArgs());
						return;
					}
					break;
			}
			base.WndProc(ref m);
		}


		protected virtual void OnChar(KeyPressEventArgs e)
		{
			if (char.IsControl(e.KeyChar))
			{
				return;
			}

			if (!HasPermitChars(e.KeyChar, this.permitChars))
			{
				e.Handled = true;
			}
		}

		protected virtual void OnPaste(System.EventArgs e)
		{
			string stString = Clipboard.GetDataObject().GetData(System.Windows.Forms.DataFormats.Text).ToString();

			if (stString != null)
			{
				this.SelectedText = GetPermitedString(stString, this.permitChars);
			}
		}

		private static bool HasPermitChars(char chTarget, char[] chPermits)
		{
			foreach (char ch in chPermits)
			{
				if (chTarget == ch)
				{
					return true;
				}
			}

			return false;
		}

		private static string GetPermitedString(string stTarget, char[] chPermits)
		{
			string stReturn = string.Empty;

			foreach (char chTarget in stTarget)
			{
				if (HasPermitChars(chTarget, chPermits))
				{
					stReturn += chTarget;
				}
			}

			return stReturn;
		}

	}

	public class Sprite
	{
		public int PosX { get; set; }
		public int PosY { get; set; }
		public int SpriteID { get; set; }
		public bool Visible { get; set; }
	}


}
