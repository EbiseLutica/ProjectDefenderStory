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
using TakeUpJewel;
using TakeUpJewel.Entities;

namespace MapEditor
{

	public partial class ObjectSelectForm : Form
	{
		public readonly string[] Items;

		public ObjectSelectForm()
		{
			InitializeComponent();

			foreach (var s in Enum.GetNames(typeof(PlayerForm)))
			{
				PlayerFormSelector.Items.Add(s);
			}

			Items = (from a in GameEngine.EntityRegister.Items
				where a.EntityId != -1
				orderby a.EntityId
				select $"{a.EntityId:0000} {a.EntityName}").ToArray();

			listBox1.Items.AddRange(Items);


			listBox1.SelectedIndex = 0;

			PlayerFormSelector.SelectedIndex = 0;
			
		}

		public RequestFlag Request = RequestFlag.None;
		

		public string Path
		{
			get
			{
				return toolStripButton2.Text;
			}
			private set
			{
				toolStripButton2.Text = value;
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
			:this()
		{
			if (filename != null)
				pictureBox1.Image = new Bitmap(filename);
			Task.Factory.StartNew(()=>
			{
				while (true)
				{
					if (!IsHandleCreated)
						continue;
					Invoke(new MethodInvoker(() =>
					{
						pictureBox1.Refresh();
					}));
					_gradient = (_gradient + 10) % 360;
					Thread.Sleep(16);
						
				}
			});
		}

		public int Chipno;

		public Random Rnd = new Random(100);

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

			}
		}
		int _tmpx = -1, _tmpy = -1;

		public ChipPack Chippack = new ChipPack(1, 1, 0);

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			_tmpx = e.X / 16;
			_tmpy = e.Y / 16;
			Chipno = e.Y / 16 * 16 + e.X / 16;
			Refresh();
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			var ex = e.X;
			var ey = e.Y;
			if (ex / 16 > 15)
				ex = 15 * 16;
			if (ey / 16 > 3)
				ey = 3 * 16;
			int w = Math.Abs(ex / 16 - _tmpx) + 1, h = Math.Abs(ey / 16 - _tmpy) + 1;
			
			var c = new byte[w * h];
			int ox = _tmpx, oy = _tmpy;
			if (ex / 16 < _tmpx)
				ox = ex / 16;
			if (ey / 16 < _tmpy)
				oy = ey / 16;
			for (var x = ox; x < ox + w; x++)
				for (var y = oy; y < oy + h; y++)
					c[(y - oy) * w + (x - ox)] = (byte)(y * 16 + x);
			Chippack = new ChipPack(w, h, c);
			_tmpx = _tmpy = -1;
		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			if (_tmpx == -1 || _tmpy == -1)
				return;
			int w = Math.Abs(e.X / 16 - _tmpx) + 1, h = Math.Abs(e.Y / 16 - _tmpy) + 1;
			var c = new byte[w * h];
			int ox = _tmpx, oy = _tmpy;
			if (e.X / 16 < _tmpx)
				ox = e.X / 16;
			if (e.Y / 16 < _tmpy)
				oy = e.Y / 16;
			for (var x = ox; x < ox + w; x++)
				for (var y = oy; y < oy + h; y++)
					c[(y - oy) * w + (x - ox)] = (byte)(y * 16 + x);
			Chippack = new ChipPack(w, h, c);
		}


		int _gradient = 255;

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			var dat = (int)(Math.Sin(_gradient / 180.0 * Math.PI) * 127 + 127);
			foreach (var chip in Chippack.Chips)
				e.Graphics.DrawRectangle(new Pen(Color.FromArgb(dat, dat, dat), 1), chip % 16 * 16, chip / 16 * 16, 16, 16);
			//Console.Write(dat + " ");
		}
		public ScreenFlag Sf = ScreenFlag.Fore;

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (Sf == ScreenFlag.Fore)
			{
				Sf = ScreenFlag.Back;
				toolStripButton1.Text = "裏";
			}
			else
			{
				Sf = ScreenFlag.Fore;
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
				pictureBox1.Image = new Bitmap(filename);
			Request = RequestFlag.ChangeMpt;
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
			Request = RequestFlag.Resize;
		}

		int[] _zoom = { 10, 20, 50, 70, 100 };

		public int Pzoom = 4;

		public int Zoom => _zoom[Pzoom];

		//拡大
		private void button2_Click(object sender, EventArgs e)
		{
			Pzoom++;
			if (Pzoom > 4)
				Pzoom = 4;
		}

		//縮小
		private void button3_Click(object sender, EventArgs e)
		{
			Pzoom--;
			if (Pzoom < 0)
				Pzoom = 0;
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
			Request = RequestFlag.CreateNew;
		}

		private void 開くOToolStripButton_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.OpenCitMap;
		}

		private void 上書き保存SToolStripButton_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SaveCitMap;
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

		private int _entityId;

		public int EntityId => _entityId;

		private void toolStripButton15_Click(object sender, EventArgs e)
		{
			Tool = ToolFlag.SpVisible;
		}

		private void citmapを保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SaveCitMap;
		}

		private void spdataを保存ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SaveSpdata;
		}

		private void マップを開くToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.OpenCitMap;
		}

		private void spdataを開くToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.OpenSpdata;
		}

		private void 上書き保存SToolStripButton_ButtonClick(object sender, EventArgs e)
		{
			Request = RequestFlag.SaveCitMap;
		}


		//↑
		private void button4_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SwapT;
		}

		//↓
		private void button5_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SwapB;
		}

		//←
		private void button6_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SwapL;
		}

		//→
		private void button7_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.SwapR;
		}

		private void 実行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.TestPlay;
		}

		private void チートプレイToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Request = RequestFlag.CheatPlay;
		}

		private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
		{
			実行ToolStripMenuItem.PerformClick();
		}

		private void propertyGrid1_Click(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			listBox1.BeginUpdate();
			listBox1.Items.Clear();
			listBox1.Items.AddRange(Items.Where(str => textBox1.Text.Contains(str)).ToArray());
			listBox1.EndUpdate();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			_entityId = int.Parse(listBox1.SelectedItem.ToString().Substring(0, 4));
		}

		private void toolStripButton16_Click(object sender, EventArgs e)
		{
			var cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.Cancel)
				return;
			RequestedColor = cd.Color;
			Request = RequestFlag.ChangeColor;
		}

	}

	public enum RequestFlag
	{
		None, Resize, ChangeMpt, CreateNew, OpenCitMap, OpenSpdata, SaveCitMap, SaveSpdata, ChangeColor, SwapL, SwapR, SwapT, SwapB, TestPlay, CheatPlay
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
		readonly char[] _permitChars = {'0','1','2','3','4','5','6','7','8','9', '0', '-', '.', };

		protected override void WndProc(ref Message m)
		{
			const int wmChar = 0x0102;
			const int wmPaste = 0x0302;

			switch (m.Msg)
			{
				case wmChar:
					if ((_permitChars != null) && (_permitChars.Length > 0))
					{
						var e = new KeyPressEventArgs((char)(m.WParam.ToInt32()));
						OnChar(e);

						if (e.Handled)
							return;
					}
					break;
				case wmPaste:
					if ((_permitChars != null) && (_permitChars.Length > 0))
					{
						OnPaste(new EventArgs());
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

			if (!HasPermitChars(e.KeyChar, _permitChars))
			{
				e.Handled = true;
			}
		}

		protected virtual void OnPaste(EventArgs e)
		{
			var stString = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();

			if (stString != null)
			{
				SelectedText = GetPermitedString(stString, _permitChars);
			}
		}

		private static bool HasPermitChars(char chTarget, char[] chPermits)
		{
			foreach (var ch in chPermits)
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
			var stReturn = string.Empty;

			foreach (var chTarget in stTarget)
			{
				if (HasPermitChars(chTarget, chPermits))
				{
					stReturn += chTarget;
				}
			}

			return stReturn;
		}

	}



}
